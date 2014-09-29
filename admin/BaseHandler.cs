using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace thesis.admin
{
    public class BaseHandler : IHttpHandler, IRequiresSessionState
    {
      protected BaseHandler()
        {

        }
        protected Dictionary<string, Func<string>> hs; 
        public virtual void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            HttpRequest req = context.Request;
            string action = req["a"].ToLower();
            string result = hs[action]();
            context.Response.Write(result);
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        protected static string req(string key)
        {
            return HttpContext.Current.Request[key];
        }
        protected static string jss(object obj)
        {
            JavaScriptSerializer JSS = new JavaScriptSerializer();
            return JSS.Serialize(obj);
        } 
    }
}
