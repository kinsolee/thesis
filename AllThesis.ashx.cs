using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace thesis.Properties
{
    /// <summary>
    /// 教学管理人员获得全部被选择的论题
    /// </summary>
    public class AllThesis : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            string filename = "~/Default";
            string template_index = "AllThesis.html";          //主页面
            string template_role = "Selected_Role.html"; //教师查看选择课题学生\指导老师的资料
            string back = "Allthesis.ashx";                    //超链接地址，用于分页

            dynamic verify_data = CommonHelper.Verify(context);      //登录

            if (verify_data.error == 0)                        //是否登录
            {
                if (context.Request["sid"] != null)             //能否获得sid参数，判断是点击 【查看学生信息】 还是【参看论题信息】
                {
                    string sid = context.Request["sid"];         //【查看学生信息】

                    DataTable dt = CommonHelper.usr_pro(sid);     //获得学生详细资料
                    if (dt == null)
                    {

                    }

                    if (dt != null)
                    {
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Select_Student = dt.Rows, Select = "student" };
                        string html = CommonHelper.GetHtml(filename, template_role, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "出现错误，没有找到此学生的信息！");
                    }
                }
                else if (context.Request["tid"] != null)
                {
                    string tid = context.Request["tid"];         //【查看指导老师信息】

                    DataTable dt = CommonHelper.usr_pro(tid);     //获得教师详细资料
                    if (dt != null)
                    {
                        if (string.Equals("student", verify_data.role))
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, Select_Teacher = dt.Rows, Select = "teacher" };
                            string html = CommonHelper.GetHtml(filename, template_role, data);
                            context.Response.Write(html);
                        }
                        else
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, Select_Teacher = dt.Rows, Select = "teacher" };
                            string html = CommonHelper.GetHtml(filename, template_role, data);
                            context.Response.Write(html);
                        }

                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "出现错误，没有找到此指导老师的信息！");
                    }
                }
                else     //   【查看论题信息】
                {
                    int pagenum = 1;
                    //分页，默认第一页
                    dynamic count = ThesisHelper.ThesisCountGather();   //获得一些统计数据  ，关于一系列论题
                    if (!string.IsNullOrEmpty(context.Request["PageNum"]) && (CommonHelper.IsNum(context.Request["PageNum"])))    //不为空，不为字符
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);

                    }

                    DataTable dt = ThesisHelper.GetAllAgreeThesis(pagenum);  //获得简单的被选择课题信息
                    if (dt != null)
                    {
                        dynamic pdata = CommonHelper.paging("t_thesis", "status=1", "", back);
                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, ThesisSelected = dt.Rows, pdata.pagedata, Pagenum = pagenum, Max = pdata.Allcount, count };
                        string html = CommonHelper.GetHtml(filename, template_index, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "目前尚没有同学选择论题！");
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