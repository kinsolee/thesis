using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace thesis
{
    /// <summary>
    /// 开题报告
    /// </summary>
    public class ThesisReport : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string tempfile = "~/Default/ueditor";
            string temphtml = "ThesisReport.html";
            string back="ThesisReport.ashx";
            dynamic verify_data = CommonHelper.Verify(context);
            if (verify_data.error == 0)
            {
               bool save = Convert.ToBoolean(context.Request["Save"]);
               if (!save)
               {
                   DataTable dt = ThesisHelper.ThesisReportEasy(verify_data.id);
                   if (dt == null)
                   {
                       CommonHelper.YesOrNo(context, false, "", "你没有论题，无法编辑开题报告！");
                   }
                   else
                   {
                       int frist_open = Convert.ToInt32(SqlHelper.ExecuteScalar("select count(*) from t_thesis_report,t_th_stu where t_th_stu.thid=t_thesis_report.thid and t_th_stu.sid=@sid", new SqlParameter("@sid", verify_data.id)));
                       var data = new { Id = verify_data.id, Role = verify_data.role, Name = verify_data.name, pro = dt.Rows, frist_open };
                       string html = CommonHelper.GetHtml(tempfile, temphtml, data);
                       context.Response.Write(html);
                   }
               }
               else
               {
                   string submit = context.Request["agree"];
                   string thid = context.Request["Thid"];
                   string report = context.Request["ThesisReport"];
                   if (string.Equals("保存", submit))
                   {
                       int error = ThesisHelper.SaveThesisReport(thid, report, -1);

                       switch (error)
                       {
                           case 0:
                               CommonHelper.YesOrNo(context, true,"保存成功！","",back);
                               break;
                           case -1:
                               CommonHelper.YesOrNo(context, false, "", "submit参数错误！");
                               break;
                           case -2:
                               CommonHelper.YesOrNo(context,false, "", "保存数据失败");
                               break;
                           case -3:
                               CommonHelper.YesOrNo(context, false, "", "插入数据失败！");
                               break;
                           default:
                               CommonHelper.YesOrNo(context, false, "", "不明原因错误！");
                               break;
                       }
                   }
                   else if (string.Equals("提交", submit))
                   {
                       int error = ThesisHelper.SaveThesisReport(thid, report, 1);

                       switch (error)
                       {
                           case 0:
                               CommonHelper.YesOrNo(context, true, "提交成功！", "", back);
                               break;
                           case -1:
                               CommonHelper.YesOrNo(context, false, "", "submit参数错误！");
                               break;
                           case -4:
                               CommonHelper.YesOrNo(context, false, "", "没有找到记录，请先保存再提交！");
                               break;
                           case -5:
                               CommonHelper.YesOrNo(context, false, "", "提交报告失败！");
                               break;
                           default:
                               CommonHelper.YesOrNo(context, false, "", "不明原因错误！");
                               break;
                       }
                   }
                   else
                   {
                       CommonHelper.YesOrNo(context, false, "", "AGREE参数错误！");
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