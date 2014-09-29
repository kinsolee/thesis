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
    /// 对应 学生【我的论题】功能
    /// </summary>
    public class Student_thesis : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "Student_thesis.html";

            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                DataTable dt = ThesisHelper.StuCheckMyThesis(verify_data.id);      //得到此学生所选择的课题的详细信息

                if (dt == null)
                {
                    CommonHelper.YesOrNo(context, false, "", "你还没有选择论题！");
                }
                else
                {
                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, StuThesis_data = dt.Rows };
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