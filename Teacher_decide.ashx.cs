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
    /// 申请列表，教师审核是否同意 当指导老师
    /// </summary>
    public class Teacher_decide : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                if (context.Request["thid"] != null)
                {
                    string thid = context.Request["thid"];
                    string agree = context.Request["agree"];

                    if (string.Equals("通过", agree))
                    {

                        int error = ThesisHelper.TeacherAgree(thid, 1);  //调用类，处理此事件

                        switch (error)
                        {
                            case 0:
                                CommonHelper.YesOrNo(context,true,"同意该学生的申请！");
                                break;
                            case -1:
                                CommonHelper.YesOrNo(context,false,"","更新数据库失败！");
                                break;
                            case -2:
                                CommonHelper.YesOrNo(context,false,"","参数错误，请重试！");
                                break;
                            case -3:
                                CommonHelper.YesOrNo(context, false, "", "你已经超过指导学生数量上限，不能再指导更多的学生！");
                                break;
                            default:
                                CommonHelper.YesOrNo(context,false,"","不明错误，请联系管理员！");
                                break;
                        }
                    }
                    else if (string.Equals("不通过", agree))
                    {

                        int error = ThesisHelper.TeacherAgree(thid, -1);   //调用类，处理此事件

                        switch (error)
                        {
                            case 0:
                                CommonHelper.YesOrNo(context, true, "拒绝该学生的申请！");
                                break;
                            case -1:
                                CommonHelper.YesOrNo(context,false,"","更新数据库失败！");
                                break;
                             case -2:
                                CommonHelper.YesOrNo(context,false,"","参数错误，请重试！");
                                break;
                            default:
                                CommonHelper.YesOrNo(context,false,"","不明错误，请联系管理员！");
                                break;
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