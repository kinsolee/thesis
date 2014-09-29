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
    /// Teacher_agree 的摘要说明
    /// </summary>
    public class Teacher_agree : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            string filename = "~/Default";
            string template_index = "Teacher_agree.html";          //主页面
            string template_role = "Selected_Role.html";            //教师查看选择课题学生的资料
            string template_thesis = "Teacher_decide.html";         //查看课题详细信息
            string back = "Teacher_agree.ashx";                    //超链接地址，用于分页

            dynamic verify_data = CommonHelper.Verify(context);      //登录

            if (verify_data.error == 0)                        //是否登录
            {
                if (context.Request["sid"] != null)             //能否获得sid参数，判断是点击 【查看学生信息】 还是【参看论题信息】
                {
                    string sid = context.Request["sid"];         //【查看学生信息】

                    DataTable dt = CommonHelper.usr_pro(sid);     //获得学生详细资料
                    if (dt != null)
                    {
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Select_Student = dt.Rows };
                        string html = CommonHelper.GetHtml(filename, template_role, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "出现错误，没有找到此学生的信息！");
                    }
                }
                else if(context.Request["thid"]!=null)            //【查看论文详细信息】
                {
                    string thid = context.Request["thid"];

                    DataTable dt = ThesisHelper.CheckMoreThesis(thid);
                    if (dt != null)
                    {
                        string fouder_teacher = ThesisHelper.CheckAdviser(thid);
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Teacher = dt.Rows, fouder_teacher };
                        string html = CommonHelper.GetHtml(filename, template_thesis, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "出现错误，没有找到此论题的信息！");
                    }
                }
                else     //   【查看简单论题信息】
                {
                    int pagenum = 1;                                              //分页，默认第一页
                    if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))    //不为空，不为字符
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);

                    }

                    DataTable dt = ThesisHelper.GetStuIntroduce(verify_data.id, pagenum);  //获得学生的申请列表
                    if (dt != null)
                    {
                        dynamic pdata = CommonHelper.paging("t_thesis", "fouder_teacher_id=" + verify_data.id + "and teacher_isagree=0 and isfromstu=1", "", back);
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Thesis_agree = dt.Rows, pdata.pagedata, Pagenum = pagenum, Max = pdata.Allcount };
                        string html = CommonHelper.GetHtml(filename, template_index, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "目前尚没有同学选择你的论题！");
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