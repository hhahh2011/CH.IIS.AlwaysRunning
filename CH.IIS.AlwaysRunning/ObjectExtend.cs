using System;
using System.Reflection;

namespace CH.IIS.AlwaysRunning
{
    #region 对象扩展

    public static class ObjectExtend
    {
        #region 其它
        /// <summary>
        /// 序列化一个对象为 QueryString
        /// </summary>
        public static string ToQueryString(this object Parameters)
        {
            string querystring = "";
            int i = 0;
            try
            {

                PropertyInfo[] properties;
#if NETFX_CORE
                properties = Parameters.GetType().GetTypeInfo().DeclaredProperties.ToArray();
#else
                properties = Parameters.GetType().GetProperties();
#endif



                foreach (var property in properties)
                {
                    querystring += property.Name + "=" + System.Uri.EscapeDataString(property.GetValue(Parameters, null).ToString());

                    if (++i < properties.Length)
                    {
                        querystring += "&";
                    }
                }



            }
            catch (NullReferenceException e)
            {
                throw new ArgumentNullException("参数必须为一个对象 并且不能为空", e);
            }

            return querystring;
        } 
        #endregion
    }

    #endregion 对象扩展
}