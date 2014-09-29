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
    /// 对应 学生选题中获得 指定论题ID的详细信息，并是否选择
    /// </summary>
    public class choose_MoreInfro : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            string filename = "~/Default";                                  //模板文件目录
            string template = "StuChooseMoreThesis.html";                //模板文件

            dynamic verify_data = CommonHelper.Verify(context);              //登录
            dynamic date = CommonHelper.Get_announcement(context);                  //获得截至日期
            if (verify_data.error == 0)                                      //登录成功
            {
                if (context.Request["thid"] != null)                         //是否获得thid参数
                {
                    string thid = context.Request["thid"];
                                                                                //学生是否选择该论题

                    if (DateTime.Now >= date.time)                        //判断是否超过截至日期
                    {
                        CommonHelper.YesOrNo(context, false, "", "此功能已经关闭！");
                    }
                    else
                    {
                        if (context.Request["choose"] == null)                      //展示基本信息
                        {
                            DataTable thesis_chooseMore = ThesisHelper.GetAgreeMoreThesis(thid);    //获得被选择论题的详细信息
                            if (thesis_chooseMore == null)
                            {
                                CommonHelper.YesOrNo(context, false, "", "没有找到相关信息！");
                            }
                            else
                            {
                                var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, thesis_data = thesis_chooseMore.Rows };
                                string html = CommonHelper.GetHtml(filename, template, data);
                                context.Response.Write(html);
                            }
                        }
                        else                                              //学生选择此论题 
                        {
                            int error = ThesisHelper.StuChooseThesis(verify_data.id, thid);

                            switch (error)
                            {
                                case 0:
                                    CommonHelper.YesOrNo(context, true, "选择此课题成功！");
                                    break;
                                case -1:
                                    CommonHelper.YesOrNo(context, false, "", "没有找到学生的信息！");
                                    break;
                                case -2:
                                    CommonHelper.YesOrNo(context, false, "", "你已经选择了一个论题！");
                                    break;
                                case -3:
                                    CommonHelper.YesOrNo(context, false, "", "错误！你可能有些数据没有更新，请联系管理员！");
                                    //相关冻结程序，
                                    break;
                                case -4:
                                    CommonHelper.YesOrNo(context, false, "", "你有一个论题正在等待审核中，不能选择论题！");
                                    break;
                                case -5:
                                    CommonHelper.YesOrNo(context, false, "", "该老师所指导学生数量已经达到上限，请选择其他教师！");
                                    break;
                                default:
                                    CommonHelper.YesOrNo(context, false, "", "不明错误！");
                                    break;
                            }
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