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
    /// 对应 教学管理人员 的 审核功能（详细信息）
    /// </summary>
    public class thesis_MoreInformation : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";                                  //模板文件目录
            string template = "ThesisMoreIntroduce.html";                //模板文件

            dynamic verify_data = CommonHelper.Verify(context);              //登录
            if (verify_data.error == 0)                                      //登录成功
            {
                if (context.Request["thid"] != null)                         //是否获得thid参数
                {
                    string thid = context.Request["thid"];                    //成功获得thid参数

                    if (context.Request["check_role"] != null)                //是否获得check_role参数
                    {
                        string check_role = context.Request["check_role"];     //判断用户-查询的是学生还是教师
                        DataTable dtthesis_data = ThesisHelper.CheckMoreThesis(thid);        //获取此角色指定的论文ID详细信息
                        if (dtthesis_data != null)
                        {
                            string fouder_teacher = ThesisHelper.CheckAdviser(thid);      //如果是学生的话，此信息则会获得论文ID的指导老师                                                                                          //如果是教师，则为NULL
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, thesis_data = dtthesis_data.Rows, fouder_teacher, check_role };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                        else
                        {
                            CommonHelper.YesOrNo(context, false, "", "参数错误！");
                        }
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "参数错误！");
                    }
                }
                else
                {
                    CommonHelper.YesOrNo(context, false, "", "参数错误！");
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