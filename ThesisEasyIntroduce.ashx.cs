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
    /// 对应 教学管理人员 的 审核功能（简单信息）
    /// </summary>
    public class thesis_examine : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";                                    //模板目录
            string template_Easy = "ThesisEasyIntroduce.html";             //展示页面
            string back = "ThesisEasyIntroduce.ashx";                      //分页超链接地址

            dynamic verify_data = CommonHelper.Verify(context);               //登录
            if (verify_data.error == 0)
            {
                string check_role = context.Request["check_role"];
                int pagenum = 1;                                              //分页，默认第一页
                if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))    //不为空，不为字符
                {
                    pagenum = Convert.ToInt32(context.Request["PageNum"]);

                }

                if (string.Equals("student", check_role))                     //管理人员，点击学生审核
                {
                    DataTable dtStudent = ThesisHelper.CheckEasyRoleThesis("student", verify_data.leaderLv,pagenum);       //获得指定页数（一页10个数据）的数据
                    if (dtStudent == null)
                    {
                        CommonHelper.YesOrNo(context, false, "", "当前尚没有学生自命题！");    
                    }
                    else
                    {                                                         //传递数据
                        if (verify_data.leaderLv == 1)
                        {
                            dynamic pdata = CommonHelper.paging("t_thesis", "status=0 and isfromstu=1 and director_isagree=0", check_role, back);
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Thesis_Easy = dtStudent.Rows, pdata.pagedata, Pagenum = pagenum, check_role, Max = pdata.Allcount };
                            string html = CommonHelper.GetHtml(filename, template_Easy, data);
                            context.Response.Write(html);
                        }
                        else if(verify_data.leaderLv==2)
                        {
                            dynamic pdata = CommonHelper.paging("t_thesis", "status=0 and isfromstu=1 and director_isagree=1", check_role, back);
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Thesis_Easy = dtStudent.Rows, pdata.pagedata, Pagenum = pagenum, check_role, Max = pdata.Allcount };
                            string html = CommonHelper.GetHtml(filename, template_Easy, data);
                            context.Response.Write(html);
                        }

                    }
                }
                else
                {                                                              //同上
                    DataTable dtTeacher = ThesisHelper.CheckEasyRoleThesis("teacher",verify_data.leaderLv, pagenum);
                    if (dtTeacher == null)
                    {
                        CommonHelper.YesOrNo(context, false, "", "当前尚没有教师自命题！");
                    }
                    else
                    {
                        if (verify_data.leaderLv == 1)
                        {
                            dynamic pdata = CommonHelper.paging("t_thesis", "status=0 and isfromstu=0 and director_isagree=0", "?check_role=" + check_role, back);
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Thesis_Easy = dtTeacher.Rows, pdata.pagedata, Pagenum = pagenum, check_role, Max = pdata.Allcount };
                            string html = CommonHelper.GetHtml(filename, template_Easy, data);
                            context.Response.Write(html);
                        }
                        else if (verify_data.leaderLv == 2)
                        {
                            dynamic pdata = CommonHelper.paging("t_thesis", "status=0 and isfromstu=0 and director_isagree=1", "?check_role=" + check_role, back);
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Thesis_Easy = dtTeacher.Rows, pdata.pagedata, Pagenum = pagenum, check_role, Max = pdata.Allcount };
                            string html = CommonHelper.GetHtml(filename, template_Easy, data);
                            context.Response.Write(html);
                        }
                    
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