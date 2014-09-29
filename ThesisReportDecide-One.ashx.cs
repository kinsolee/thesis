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
    /// ThesisReportDecide_One 的摘要说明
    /// 开题报告审核-教师
    /// </summary>
    public class ThesisReportDecide_One : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string tempfile = "~/Default";
            string temphtml = "ThesisReportDecide-One.html";
            
            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                string type = context.Request["TYPE"];
                string back = "ThesisReportDecide-One.ashx?type="+type;
                if (string.Equals("TEACHER", type))    //指导教师审核
                {
                    int pagenum = 1;                                              //分页，默认第一页
                    if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);

                    }
                    DataTable dt = ThesisHelper.GetThesisReport_Decide_Teacher(pagenum, verify_data.id, 0, verify_data.leaderLv);

                    dynamic pdata = CommonHelper.paging("t_thesis_report", "submit=1 and teacher_isagree=0 and status=0", "", back);  //获得总数据的数量，生成一系列超链接

                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Report = dt.Rows, pdata.pagedata, Max = pdata.Allcount, Pagenum = pagenum,type=0 };
                    string html = CommonHelper.GetHtml(tempfile, temphtml, data);
                    context.Response.Write(html);
                }
                else if (string.Equals("ADMIN", type) && verify_data.isleader==1)     //管理人员审核
                {
                    int pagenum = 1;                                              //分页，默认第一页
                    if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);

                    }
                    DataTable dt = ThesisHelper.GetThesisReport_Decide_Teacher(pagenum, verify_data.id,1, verify_data.leaderLv);

                    dynamic pdata = null;
                    if (verify_data.leaderLv == 1)
                    {
                        pdata = CommonHelper.paging("t_thesis_report", "submit=1 and teacher_isagree=1 and director_isagree=0 and status=0", "", back);  //获得总数据的数量，生成一系列超链接
                    }
                    else
                    {
                        pdata = CommonHelper.paging("t_thesis_report", "submit=1 and director_isagree=1 and president_isagree=0 and status=0", "", back);  //获得总数据的数量，生成一系列超链接
                    }

                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Report = dt.Rows, pdata.pagedata, Max = pdata.Allcount, Pagenum = pagenum, type = 1 };
                    string html = CommonHelper.GetHtml(tempfile, temphtml, data);
                    context.Response.Write(html);
                }
                else
                {
                    CommonHelper.YesOrNo(context, false, "", "Type参数错误！或者你没有查看此页面的能力！");
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