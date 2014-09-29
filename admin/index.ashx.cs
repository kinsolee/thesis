using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization; 

namespace thesis.admin
{
    /// <summary>
    /// index 的摘要说明
    /// </summary>
    public class index : BaseHandler
    {
        
       public index(){
            hs = new Dictionary<string, Func<string>>();
            string filename = "~/admin/Tpl";
            hs.Add("add_staff", delegate() 
　　　　    {
            return CommonHelper.GetHtml(filename, "add_staff.html", new { });
　　　　    });
            hs.Add("add_stu", delegate()
            {
                return CommonHelper.GetHtml(filename, "add_stu.html", new { });
            });
           //显示学生列表页
            hs.Add("stu_list", delegate()
            {
                int p = 1;
                if (req("p") != null) {
                    p = Convert.ToInt32(req("p"));//获取分页参数
                }
                int size=10;//分页大小
                int count =Convert.ToInt32(SqlHelper.ExecuteScalar("select COUNT(id) from t_student"));//获取总人数
                DataTable stu_dt = SqlHelper.ExecuteDataTable(@"select * from (select *,ROW_NUMBER() over (order by sid asc) as no from t_student) v where v.no between @start and @end",
                    new SqlParameter("@end", p * size),
                    new SqlParameter("@start", (p-1) * size +1));
                string paging = CommonHelper.paging(count, size, p);
                //string tpl = "stu_list.html;";
                if (HttpContext.Current.Request.Headers["x-requested-with"] == "XMLHttpRequest")
                {
                    dynamic databack =new { data= CommonHelper.GetHtml(filename, "public/stu_list_tmpl.html", new { stu_list = stu_dt.Rows, page = paging })};
                    return jss(databack);
                }
                return CommonHelper.GetHtml(filename, "list.html", new { stu_list = stu_dt.Rows, page = paging,member="stu" });
            });
           //显示教务员列表
            hs.Add("tec_list", delegate()
            {
                int p = 1;
                if (req("p") != null)
                {
                    p = Convert.ToInt32(req("p"));//获取分页参数
                }
                int size = 10;//分页大小
                int count = Convert.ToInt32(SqlHelper.ExecuteScalar("select COUNT(id) from t_teacher"));//获取总人数
                DataTable stu_dt = SqlHelper.ExecuteDataTable(@"select * from (select *,ROW_NUMBER() over (order by tid asc) as no from t_teacher) v where v.no between @start and @end",
                    new SqlParameter("@end", p * size),
                    new SqlParameter("@start", (p - 1) * size + 1));
                string paging = CommonHelper.paging(count, size, p);
                //string tpl = "stu_list.html;";
                if (HttpContext.Current.Request.Headers["x-requested-with"] == "XMLHttpRequest")
                {
                    dynamic databack = new { data = CommonHelper.GetHtml(filename, "public/tec_list_tmpl.html", new { stu_list = stu_dt.Rows, page = paging }) };
                    return jss(databack);
                }
                return CommonHelper.GetHtml(filename, "list.html", new { tec_list = stu_dt.Rows, page = paging, member = "tec" });
            });
        }
        public override void  ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            HttpRequest req = context.Request;
            string action = req["a"].ToLower();
            string result = hs[action]();
            context.Response.Write(result);
        }
        
    }
}