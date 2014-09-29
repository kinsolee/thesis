using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace thesis
{
    /// <summary>
    /// 提供 毕业论文选题汇总 的xls下载
    /// </summary>
    public class Download : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
           
            dynamic verify_data = CommonHelper.Verify(context);
            
            if (verify_data.error == 0)
            {
                //获得数据表（汇总）
                DataTable dt = SqlHelper.ExecuteDataTable(@"((select t_thesis.fouder_teacher_id as 工号,t_teacher.name as 指导教师,t_thesis.title as 毕业论文题目,ISNULL(t_student.name,null) as 所指导的学生,ISNULL(t_th_stu.sid,null) as 学号,ISNULL(t_student.department,null) as 班级,'否' as 是否学生自命题,ISNULL(t_student.tel,null)as 学生长号,ISNULL(t_student.shorttel,null)as 学生短号  from t_thesis left join t_th_stu on t_thesis.thid=t_th_stu.thid and t_thesis.status=1 and t_thesis.fouder_teacher_id=t_th_stu.tid left join t_student on t_th_stu.sid=t_student.sid left join t_teacher on t_teacher.tid=t_thesis.fouder_teacher_id  where t_thesis.status=1 and isfromstu=0)
UNION ALL
(select t_thesis.fouder_teacher_id as 工号,t_teacher.name as 指导教师,t_thesis.title as 毕业论文题目,ISNULL(t_student.name,null) as 所指导的学生,ISNULL(t_th_stu.sid,null) as 学号,ISNULL(t_student.department,null) as 班级,'是' as 是否学生自命题,ISNULL(t_student.tel,null)as 学生长号,ISNULL(t_student.shorttel,null)as 学生短号  from t_thesis left join t_th_stu on t_thesis.thid=t_th_stu.thid and t_thesis.status=1 and t_thesis.fouder_teacher_id=t_th_stu.tid left join t_student on t_th_stu.sid=t_student.sid left join t_teacher on t_teacher.tid=t_thesis.fouder_teacher_id  where t_thesis.status=1 and isfromstu=1))order by t_thesis.fouder_teacher_id ");
                NpoiHelper.ExportDataTableToExcel(dt, DateTime.Now.Year+"年毕业论文选题汇总.xls", "毕业论文选题汇总");
               
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