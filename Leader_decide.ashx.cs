using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.SessionState;

namespace thesis
{
    /// <summary>
    /// 教学管理人员审核
    /// </summary>
    public class Leader_decide : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            dynamic verify_data = CommonHelper.Verify(context);
            if (verify_data.error == 0)
            {
                if (context.Request["thid"] != null)            //判断有没有获得thid参数
                {
                    string thid = context.Request["thid"];
                    if (context.Request["agree"] != null)      //判断有没有获得agree参数
                    {
                        string agree = context.Request["agree"];
                        string check_role=context.Request["check_role"];
                        string comment = context.Request["comment"];
                        string back = "ThesisEasyIntroduce.ashx?check_role=" + check_role;         //用户操作后，返回的界面

                        if (string.Equals("通过", agree))                           //通过审核
                        {
                            int error = ThesisHelper.DecideThesis(thid, 1, comment,verify_data.id,verify_data.leaderLv);       //调用审核类

                            switch (error)
                            {
                                case 0:
                                    CommonHelper.YesOrNo(context, true, "审核成功！", "", back);
                                    break;
                                case -1:
                                    CommonHelper.YesOrNo(context, false, "", "审核失败，没有找到相关的论题号！", back);
                                    break;
                                case -2:
                                    CommonHelper.YesOrNo(context, false, "", "审核失败，status参数错误！", back);
                                    break;
                                case -3:
                                    CommonHelper.YesOrNo(context, false, "", "错误！你可能有一些数据没有更新，请联系管理人员！", back);
                                    //相关操作
                                    break;
                                default:
                                    CommonHelper.YesOrNo(context, false, "","不明原因!");
                                    break;
                            }
                        }
                        else
                        {
                            int error = ThesisHelper.DecideThesis(thid, -1, comment, verify_data.id, verify_data.leaderLv);
                            switch (error)
                            {
                                case 0:
                                    CommonHelper.YesOrNo(context, true, "审核成功！","", back);
                                    break;
                                case -1:
                                    CommonHelper.YesOrNo(context, false, "", "审核失败，没有找到相关的论题号！", back);
                                    break;
                                case -2:
                                    CommonHelper.YesOrNo(context, false, "", "审核失败，status参数错误！", back);
                                    break;
                                case -3:
                                    CommonHelper.YesOrNo(context, false, "", "审核失败，更新数据失败！", back);
                                    break;
                                default:
                                    CommonHelper.YesOrNo(context, false, "", "不明原因!");
                                    break;
                            }
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