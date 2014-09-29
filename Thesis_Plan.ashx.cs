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
    /// 对应 用户 查看进度的功能 （简单信息）
    /// </summary>
    public class Thesis_Plan : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "Thesis_Plan.html";
            string back="Thesis_Plan.ashx";

            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                int pagenum = 1;                                              //分页，默认第一页
                if (context.Request["PageNum"] != null)
                    if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))    //不为空，不为字符
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);

                    }
                DataTable dt = ThesisHelper.CheckSomebodyThesis(verify_data.id, pagenum);

                if (dt == null)
                {
                    CommonHelper.YesOrNo(context, false, "", "目前你没有任何历史进度或者正在等待审核的进度");
                }
                else
                {
                    if (string.Equals("student", verify_data.role))              //判断是教师还是学生进入该页面
                    {
                        dynamic pdata = CommonHelper.paging("t_thesis", "isfromstu=1 and fouder_id=" + verify_data.id, "", back);
                        DataTable dt_report = ThesisHelper.GetThesisReport_Decide_Teacher(1, verify_data.id, 3, 0);
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, ThesisPlan = dt.Rows, pdata.pagedata, Pagenum = pagenum, Max = pdata.Allcount,Report=dt_report.Rows };
                        string html = CommonHelper.GetHtml(filename, template, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        dynamic pdata = CommonHelper.paging("t_thesis", "isfromstu=0 and fouder_id=" + verify_data.id, "", back);
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, ThesisPlan = dt.Rows, pdata.pagedata, Pagenum = pagenum, Max = pdata.Allcount };
                        string html = CommonHelper.GetHtml(filename, template, data);
                        context.Response.Write(html);
                    }
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