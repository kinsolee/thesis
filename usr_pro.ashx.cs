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
    /// 用户个人资料展示
    /// </summary>
    public class usr_pro : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "usr_pro.html";

            dynamic verify_data = CommonHelper.Verify(context);  //验证用户登录
            if (verify_data.error == 0)    //是否登录
            {
                bool save = Convert.ToBoolean(context.Request["Save"]);   //是不是第一次进入页面
                if (!save)  //展示信息
                {
                    DataTable usr_pro = CommonHelper.usr_pro(verify_data.id);  //获得用户个人资料
                    if (usr_pro == null)
                    {
                        CommonHelper.YesOrNo(context, false, "", "读取资料失败，请重试！");
                    }
                    else
                    {
                        if (string.Equals("teacher", verify_data.role)) //判断用户角色分别展示内容
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv,pro=usr_pro.Rows };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                        else
                        {
                            var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, pro = usr_pro.Rows };
                            string html = CommonHelper.GetHtml(filename, template, data);
                            context.Response.Write(html);
                        }
                    }
                }
                else    //用户提交表单
                {
                    if (string.Equals("student", verify_data.role))   //判断用户角色
                    {
                        string shorttel = context.Request["shorttel"];
                        string tel = context.Request["tel"];
                        dynamic usr_data = new { shorttel, tel };
                        int error = CommonHelper.usr_edit(verify_data.id, usr_data);   //修改用户个人资料，并返回错误信息

                            switch (error)
                            {
                                case 0:
                                    CommonHelper.YesOrNo(context, true, "保存成功！", "", "usr_pro.ashx");
                                    break;
                                case -1:
                                    CommonHelper.YesOrNo(context, false, "", "账号错误！");
                                    break;
                                case -2:
                                    CommonHelper.YesOrNo(context, false, "", "异常，找不到用户！");
                                    break;
                                case -3:
                                    CommonHelper.YesOrNo(context, false, "", "异常，影响多个用户！");
                                    break;
                                case -4:
                                    CommonHelper.YesOrNo(context, false, "", "电话号码不是全数字！", "usr_pro.ashx");
                                    break;
                                case -5:
                                    CommonHelper.YesOrNo(context, false, "", "电话号码超出长度！", "usr_pro.ashx");
                                    break;       
                            }
                        
                        
                    }
                    else         //教师
                    {
                        string tel = context.Request["tel"];
                        string shorttel = context.Request["shorttel"];
                        string direction = context.Request["direction"];
                        direction = direction.Replace(" ", "&nbsp");
                        direction = direction.Replace("\n", "<br>");
                        string email = context.Request["email"];
                        string jobtitle = context.Request["jobtitle"];
                        dynamic usr_data = new { tel, shorttel, jobtitle, direction, email };
                        int error = CommonHelper.usr_edit(verify_data.id, usr_data); //修改用户个人资料，并返回错误信息


                        switch (error)
                        {
                            case 0:
                                CommonHelper.YesOrNo(context, true, "保存成功！", "", "usr_pro.ashx");
                                break;
                            case -1:
                                CommonHelper.YesOrNo(context, false, "", "账号错误！");
                                break;
                            case -2:
                                CommonHelper.YesOrNo(context, false, "", "异常，找不到用户！");
                                break;
                            case -3:
                                CommonHelper.YesOrNo(context, false, "", "异常，影响多个用户！");
                                break;
                            case -4:
                                CommonHelper.YesOrNo(context, false, "", "电话号码不是全数字！", "usr_pro.ashx");
                                break;
                            case -5:
                                CommonHelper.YesOrNo(context, false, "", "电话号码超出长度！", "usr_pro.ashx");
                                break;

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