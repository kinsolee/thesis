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
    /// 创建一个新论题
    /// </summary>
    public class teacher_create_thesis : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "thesis_create.html";

            dynamic verify_data = CommonHelper.Verify(context);  //验证用户登录
            dynamic date = CommonHelper.Get_announcement(context);      //得到截至日期
            if (verify_data.error == 0)    //是否登录
            {
                if (DateTime.Now >=date.time)
                {
                    CommonHelper.YesOrNo(context, false, "", "此功能已经关闭！");
                }
                else
                {
                    bool save = Convert.ToBoolean(context.Request["Save"]);              // 得到Save参数，用于判断有无按下submit
                    if (!save)                                                           //第一次进去此页面
                    {                        
                        if (string.Equals("teacher", verify_data.role))                 //由于教师与学生所需数据略微不同，所以分开展示
                        {
                            var data = new { Name = verify_data.name, Role = verify_data.role, Id = verify_data.id, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                        else                             //学生展示
                        {
                            DataTable dtTeacher_name = ThesisHelper.Check_AllTeacher();

                            var data = new { Name = verify_data.name, Role = verify_data.role, Id = verify_data.id, teacher_name = dtTeacher_name.Rows };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                    }
                    else                             //用户按下保存按钮
                    { 
                        dynamic create_data = new           //获取信息
                        {
                            id = verify_data.id,
                            name = verify_data.name,
                            role = verify_data.role,
                            title = context.Request["title"].ToString(),
                            applyclass = context.Request["applyclass"].ToString(),
                            source = context.Request["source"].ToString(),
                            type = context.Request["type"].ToString(),
                            condition = context.Request["condition"].ToString(),
                            introduce = context.Request["introduce"].ToString(),
                            fouder_teacher_id = context.Request["teacher"]
                        };

                        if (ThesisHelper.CheckSameThesis(create_data.title))     //判断标题有无重复
                        {
                            int error = ThesisHelper.CreateThesis(create_data);     //更新数据库

                            switch (error)
                            {
                                case 0:
                                    CommonHelper.YesOrNo(context, true, "提交成功，请等待审核！", "", "thesis_create.ashx");
                                    break;
                                case -1:
                                    CommonHelper.YesOrNo(context, false, "", "提交失败，请重新操作", "thesis_create.ashx");
                                    break;
                                case -2:
                                    CommonHelper.YesOrNo(context, false, "", "提交失败，此学生已经有一个论题正在审核中，不可再提交论题！", "thesis_create.ashx");
                                    break;
                                case -3:
                                    CommonHelper.YesOrNo(context, false, "", "教师更新数据失败！", "thesis_create.ashx");
                                    break;
                                case -4:
                                    CommonHelper.YesOrNo(context, false, "", "学生更新数据失败！", "thesis_create.ashx");
                                    break;
                                case -5:
                                    CommonHelper.YesOrNo(context, false, "", "提交失败，此学生已经选择了一个课题！", "thesis_create.ashx");
                                    break;
                            }
                        }
                        else
                        {
                            CommonHelper.YesOrNo(context, false, "", "已经存在相同的标题，请修改后再提交！", "thesis_create.ashx");
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