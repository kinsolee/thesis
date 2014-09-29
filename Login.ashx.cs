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
    /// 登录界面
    /// </summary>
    public class Login : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename="~/Login";
            string template="login.html";
            context.Session.Clear();   //清除Session的所有信息（记录用户的一些基本信息）
            
                bool login = Convert.ToBoolean(context.Request["Login"]);
                if (login)
                {
                    string usr = context.Request["Username"];
                    string pwd = context.Request["Password"];
                    dynamic login_data = CommonHelper.Login(usr, pwd);   //登录类

                    if (login_data.error == 0)   //判断用户是否登录成功！
                    {
                        //将用户的基本信息存入Session
                        context.Session["ID"] = login_data.id;           //用户ID
                        context.Session["ROLE"] = login_data.role;       //用户角色类型(学生，教师)
                        context.Session["NAME"] = login_data.name;       //用户姓名

                        if (string.Equals("teacher", login_data.role))   //判断用户是教师
                        {
                            context.Session["ISLEADER"] = login_data.isleader;        //该教师是否具有管理能力
                            context.Session["LEADERLV"] = login_data.leaderLv;         //管理能力的等级
                        }

                        if (string.Equals("111111", pwd))
                        {
                            context.Response.Redirect("usr_pwdEdit.ashx");   //密码为初始密码，重定向至修改密码
                        }
                        else
                        {
                            context.Response.Redirect("index.ashx");       //重定向至主页
                        }
                                           
                    }
                    else
                    {
                        switch ((int)login_data.error)
                        {
                            case -1:
                                CommonHelper.YesOrNo(context, false, "", "用户名输入有误", "Login.ashx");
                                break;
                            case -2:
                                CommonHelper.YesOrNo(context, false, "", "无此用户", "Login.ashx");
                                break;
                            case -3:
                                CommonHelper.YesOrNo(context, false, "", "密码输入错误,请重新输入", "Login.ashx");
                                break;
                            default:
                                CommonHelper.YesOrNo(context, false, "", "错误信息不明", "Login.ashx");
                                break;
                        }
                    }
                }
                else
                {
                    string html=CommonHelper.GetHtml(filename,template,null);
                    context.Response.Write(html);
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