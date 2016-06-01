using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CH.IIS.AlwaysRunning
{
    partial class MainService : ServiceBase
    {

        public MainService()
        {
            InitializeComponent();
        }

        private readonly CancellationTokenSource _ts = new CancellationTokenSource();
        private static object _obj = new object();
        protected override void OnStart(string[] args)
        {
            
            var urlsString = ConfigurationManager.AppSettings["Urls"] ?? "";
            var intervalString = ConfigurationManager.AppSettings["Interval"] ?? (30*60).ToString(); //30����
            var interval = int.Parse(intervalString);
            var urls = urlsString?.Split(';');

            Task.Factory.StartNew(() =>
            {
             
                Log("��ʼ����");
                while (!_ts.IsCancellationRequested)
                {
                    try
                    {
                        if (urls?.Length == 0)
                        {
                            continue;
                        }
                        Log("��ʼ����");
                        foreach (var url in urls)
                        {
                            HttpLibAsyncRequest.Get(url, (p) =>
                            {
                                Log($"[{DateTime.Now}]����[{url}],�Ƿ�ɹ�[{p.Length > 0}]");
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        Log(ex.Message);
                    }
                    Thread.Sleep(interval*1000);
                }
            }, _ts.Token);


        }


        private void Log(string msg)
        {
            lock (_obj)
            {
                var logPath = AppDomain.CurrentDomain.BaseDirectory;
                using (
                    var fs = new FileStream(logPath + DateTime.Now.ToString("yyyyMMdd") + ".log",
                        FileMode.Append, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(msg);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            _ts.Cancel();
            Log("ֹͣ����");
            base.OnStop();
        }

        protected override void OnShutdown()
        {
            _ts.Cancel();
            Log("ֹͣ����");
            base.OnShutdown();
        }
    }
}
