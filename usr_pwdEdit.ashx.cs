using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace thesis
{
    /// <summary>
    /// 用户个人密码修改
    /// </summary>
    public class usr_pwdEdit : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string filename = "~/Default";
            string template = "usr_pwdEdit.html";
            dynamic verify_data = CommonHelper.Verify(context);   //用户登录类

            if (verify_data.error == 0)    //用户是否登录
            {
                bool save = Convert.ToBoolean(context.Request["Save"]);   //判断用户是否第一次进入页面
                if (!save)   //展示
                {
                    if (string.Equals("teacher", verify_data.role))
                    {
                        var data = new { Name = verify_data.name, Id = verify_data.id, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv };
                        string html = CommonHelper.GetHtml(filename, template, data);
                        context.Response.Write(html);
                    }
                    else
                    {
                        var data = new { Name = verify_data.name, Id = verify_data.id, Role = verify_data.role };
                        string html = CommonHelper.GetHtml(filename, template, data);
                        context.Response.Write(html);
                    }
                }
                else
                {
                    string oldPwd = context.Request["oldpwd"];
                    string newPwd = context.Request["newpwd"];
                    string pwdagin = context.Request["pwdagin"];

                    if (string.Equals(newPwd, pwdagin))
                    {
                        if ((newPwd.Length >= 6) && (oldPwd.Length <= 8))
                        {
                            int error = CommonHelper.usr_edit_pwd(verify_data.id, oldPwd, newPwd);   //修改密码，并返回错误信息

                            if (error == 0)
                            {
                                CommonHelper.YesOrNo(context, true, "修改密码成功！");
                            }
                            else if (error == -2)
                            {
                                CommonHelper.YesOrNo(context, false, "", "输入的旧密码不对，请重新输入！", "usr_pwdEdit.ashx");
                            }
                            else
                            {
                                CommonHelper.YesOrNo(context, false, "", "修改密码失败，请重新输入！", "usr_pwdEdit.ashx");
                            }
                        }
                        else
                        {
                            CommonHelper.YesOrNo(context, false, "", "密码必须至少六位，最多八位！", "usr_pwdEdit.ashx");
                        }
                    }
                    else
                    {
                        CommonHelper.YesOrNo(context, false, "", "两次密码输入不一致，请重新输入！", "usr_pwdEdit.ashx");
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