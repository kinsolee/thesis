using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;


namespace thesis
{
    /// <summary>
    /// announcement 的摘要说明
    /// </summary>
    public class announcement : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            string filename = "~/Default";
            string template = "announcement.html";
            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                dynamic ann = CommonHelper.Get_announcement(context);
                bool save = Convert.ToBoolean(context.Request["Save"]);
                    if (!save)
                    {
                        if (string.Equals("student", verify_data.role))
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, Ann = ann.ann, Expiry_date = ann.expiry_date };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                        else
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role,IsLeader=verify_data.isleader,LeaderLv=verify_data.leaderLv, Ann = ann.ann, Expiry_date = ann.expiry_date };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                    }
                    else
                    {
                        string Ann = context.Request["ann"];
                        string expiry_date = context.Request["expiry_date"];

                        CommonHelper.YesOrNo(context, CommonHelper.Edit_announcement(Ann, Convert.ToDateTime(expiry_date)));

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