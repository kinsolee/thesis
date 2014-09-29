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
    /// 对应 用户的  查看  进度  功能（详细信息）
    /// </summary>
    public class Thesis_MoreHistory : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            string filename = "~/Default";
            string template = "Thesis_MoreHistory.html";

            dynamic verify_data = CommonHelper.Verify(context);    //登录
            if (verify_data.error == 0)
            {
                if (context.Request["thid"] != null)     //是否能获取到thid参数
                {
                    string thid = context.Request["thid"];

                    DataTable dtThesis = ThesisHelper.CheckMoreThesis(thid);   //查找thid的详细信息
                    if (dtThesis != null)
                    {
                        string Check_Teacher = ThesisHelper.CheckAdviser(thid);
                        if (string.Equals("student", verify_data.role))             //教师和学生分开展示，因为数据有略微的区别
                        {
                            
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, thesis_data = dtThesis.Rows,Check_Teacher };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                        else              //教师
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, thesis_data = dtThesis.Rows, Check_Teacher };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "论文号参数错误!");
                    }
                }
                else
                {
                    CommonHelper.YesOrNo(context, false, "", "论文号参数错误！");
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