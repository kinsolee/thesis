using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace thesis
{
    /// <summary>
    /// index 的摘要说明
    /// </summary>
    public class index : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "index.html";
            dynamic verify_data = CommonHelper.Verify(context);   //用户登录类

            if (verify_data.error == 0)    //用户是否登录
            {
                dynamic ann = CommonHelper.Get_announcement(context);
                if (string.Equals("student", verify_data.role))
                {
                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, Ann = ann.ann, Expiry_date = ann.expiry_date };
                    string html = CommonHelper.GetHtml(filename, template, data);
                    context.Response.Write(html);
                }
                else
                {
                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Ann = ann.ann, Expiry_date = ann.expiry_date };
                    string html = CommonHelper.GetHtml(filename, template, data);
                    context.Response.Write(html);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}