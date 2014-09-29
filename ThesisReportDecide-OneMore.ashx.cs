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
    /// ThesisReportDecide_OneMore 的摘要说明
    /// 详细的开题报告
    /// </summary>
    public class ThesisReportDecide_OneMore : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string tempfile = "~/Default/ueditor";
            string temphtml = "ThesisReportDecide-OneMore.html";
            

            dynamic verify_data = CommonHelper.Verify(context);

            if (verify_data.error == 0)
            {
                bool save = Convert.ToBoolean(context.Request["SAVE"]);
                string thid = context.Request["thid"];     //one传来的thid
                string type = context.Request["type"];    //type=1（admin）通过管理员进入该页面，=0为教师
                                                            //-1为仅为浏览
                string type_string;
                if (string.Equals("0", type))
                    type_string = "TEACHER";
                else
                    type_string = "ADMIN";
                string back = "ThesisReportDecide-One.ashx?type=" + type_string;    //生成超链接的地址
                if (!save)   //第一次进入此页面
                {
                    DataTable dt = ThesisHelper.GetThesisReport_Infromation(thid);
                    var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, IsLeader = verify_data.isleader, LeaderLv = verify_data.leaderLv, pro = dt.Rows,type };
                    string html = CommonHelper.GetHtml(tempfile, temphtml, data);
                    context.Response.Write(html);
                }
                else    //前端传来数据
                {
                    int status = 0;
                    
                    string comment = context.Request["comment"];     //获取性格信息
                    string agree = context.Request["agree"];
                    string type_again = context.Request["type"];    //因为传回来，刷新了ashx，所以上面的type丢失，重新获取

                    if (string.Equals("同意", agree))    //判断用户按下哪个按钮，并赋值
                        status = 1;
                    else
                        status = -1;

                    int error = ThesisHelper.ThesisReport_Decide(thid, status, comment, verify_data.leaderLv,Convert.ToInt32(type_again));    //判断类


                    switch (error)
                    {
                        default:
                            CommonHelper.YesOrNo(context, false, "", "发生异常！");
                            break;
                        case 0:
                            CommonHelper.YesOrNo(context, true,"审核成功！","",back);
                            break;
                        case -1:
                            CommonHelper.YesOrNo(context, false, "", "LV参数异常!", back);
                            break;
                        case -2:
                            CommonHelper.YesOrNo(context, false, "", "更新数据失败！！", back);
                            break;
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