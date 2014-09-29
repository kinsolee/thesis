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
    /// 学生选题 简要信息
    /// </summary>
    public class student_choose : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            
            string filename = "~/Default";                                    //模板目录
            string template = "StuChooseEasyThesis.html";             //展示页面
            string back = "StuChooseEasyThesis.ashx";                      //分页超链接地址

            dynamic verify_data = CommonHelper.Verify(context);               //登录
            dynamic date = CommonHelper.Get_announcement(context);              //获得截至日期
            if (verify_data.error == 0)
            {
                if (DateTime.Now >= date.time)                   //比对有无超过截至日期
                {
                    CommonHelper.YesOrNo(context, false, "", "此功能已经关闭！");
                }
                else
                {
                    int pagenum = 1;                                              //分页，默认第一页
                    if (!string.IsNullOrEmpty(context.Request["PageNum"])&&(CommonHelper.IsNum( context.Request["PageNum"])))
                    {
                        pagenum = Convert.ToInt32(context.Request["PageNum"]);
                        
                    }

                    DataTable thesis_data = ThesisHelper.GetStuCanChoose(pagenum);   //获得相应页数的数据（可以被学生选择的课题）

                    if (thesis_data == null)                
                    {
                        CommonHelper.YesOrNo(context, false, "", "目前尚没有通过审核的论题！");
                    }
                    else
                    {
                        dynamic pdata = CommonHelper.paging("t_thesis", "isfromstu=0 and status=1 and choosed=0", "", back);  //获得总数据的数量，生成一系列超链接

                        var data = new { Id = verify_data.id, Name = verify_data.name, Role = verify_data.role, Thesis_Easy = thesis_data.Rows, pdata.pagedata,Max=pdata.Allcount ,Pagenum = pagenum  };
                        string html = CommonHelper.GetHtml(filename, template, data);
                        context.Response.Write(html);
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