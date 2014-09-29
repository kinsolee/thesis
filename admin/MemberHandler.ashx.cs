using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace thesis.admin
{
    /// <summary>
    /// handler_staff 的摘要说明
    /// </summary>
    public class MemberHandler : BaseHandler//继承控制器基类
    {
        public MemberHandler()
        {
            //创建字典映射方法
            hs = new Dictionary<string, Func<string>>();

            //添加老师
            hs.Add("add_tec", delegate()
        {
            string tid = req("tid").ToString();
            string name = req("name").ToString();
            string institute = req("institute").ToString();
            string degree = req("degree").ToString();
            string jobtitle = req("jobtitle").ToString();
            int role = Convert.ToInt32(req("role").ToString());
            int isLeader = 0;
            int leaderLevel = role - 1;//添加权限
            isLeader = (Convert.ToBoolean(role - 1)) ? 1 : 0;//判断教师资格
            int id = SqlHelper.ExecuteNonQuery("insert into t_teacher (tid,pwd,name,institute,degree,jobtitle,isleader,leaderlevel) values(@tid,@pwd,@name,@institute,@degree,@jobtitle,@isleader,@leaderlevel)",
                new SqlParameter("@tid", tid),
                new SqlParameter("@name", name),
                new SqlParameter("@institute", institute),
                new SqlParameter("@degree", degree),
                new SqlParameter("@jobtitle", jobtitle),
                new SqlParameter("@isleader", isLeader),
                new SqlParameter("@leaderlevel", leaderLevel),
                new SqlParameter("@pwd", CommonHelper.EncryptionMD5("111111"))
            );
            if (id > 0)
            {
                return CommonHelper.GetHtml("~/admin/Tpl", "tec_list.html", new { });
                //return jss(new { status = 1, msg = "添加成功" });
            }
            else
            {
                return jss(new { status = 0, msg = "添加失败" });
            }

        });

            //添加学生
            hs.Add("add_stu", delegate()
            {
                int id = Convert.ToInt32(req("id"));
                string sid = req("sid").ToString();
                string name = req("name").ToString();
                string institute = req("institute").ToString();
                string department = req("department").ToString();
                if (id > 0)
                {
                    string s_class = req("class");
                    string tel = req("tel");
                    string shorttel = req("shorttel");
                    id = SqlHelper.ExecuteNonQuery("update t_student set sid=@sid,name=@name,institute=@institute,department=@department,class=@class,tel=@tel,shorttel=@shorttel where id=@id",
                                      new SqlParameter("@sid", sid),
                                      new SqlParameter("@name", name),
                                      new SqlParameter("@institute", institute),
                                      new SqlParameter("@department", department),
                                      new SqlParameter("@class", s_class),
                                      new SqlParameter("@tel", tel),
                                      new SqlParameter("@shorttel", shorttel),
                                      new SqlParameter("@id", id)
                                  );
                }
                else
                {
                    id = SqlHelper.ExecuteNonQuery("insert into t_student (sid,pwd,name,institute,department) values(@sid,@pwd,@name,@institute,@department)",
                                       new SqlParameter("@sid", sid),
                                       new SqlParameter("@name", name),
                                       new SqlParameter("@institute", institute),
                                       new SqlParameter("@department", department),
                                       new SqlParameter("@pwd", CommonHelper.EncryptionMD5("111111"))
                                   );
                }

                if (id > 0)
                {
                    //HttpContext.Current.Response.Redirect("index.ashx?a=stu_list");
                    //return CommonHelper.GetHtml("~/admin/Tpl", "stu_list.html", new { });
                    return jss(new { status = 1, msg = "添加成功" });
                }
                else
                {
                    return jss(new { status = 0, msg = "添加失败" });
                }
            });
            //删除账号
            hs.Add("mb_del", delegate()
            {
                int id = Convert.ToInt32(req("id"));
                string type = req("type");
                if (id > 0)
                {
                    string table = "";
                    switch (type)
                    {
                        case "stu":
                            table = "t_student";
                            break;
                        case "tec":
                            table = "t_teacher";
                            break;
                    }
                    if (table != "")
                    {
                        id = SqlHelper.ExecuteNonQuery("delete from "+table+" where id = @id", new SqlParameter("@id", id));
                    }
                }
                return jss(new { status = 1, msg = "删除成功" });
            });
            //冻结账号
            hs.Add("mb_fs", delegate()
            {
                int id = Convert.ToInt32(req("id"));
                string type = req("type");
                int is_active = int.Parse( req("is_active"));
                if (id > 0)
                {
                    string table = "";
                    switch (type)
                    {
                        case "stu":
                            table = "t_student";
                            break;
                        case "tec":
                            table = "t_teacher";
                            break;
                    }
                    id = SqlHelper.ExecuteNonQuery("update " + table + " set is_active= " + is_active + " where id = @id", new SqlParameter("@id", id));
                }
                return jss(new { status = 1, msg = "操作成功" });
            });
            //重置密码
            hs.Add("mb_rspsw", delegate()
            {
                int id = Convert.ToInt32(req("id"));
                string type = req("type");
                if (id > 0)
                {
                    string table = "";
                    switch (type)
                    {
                        case "stu":
                            table = "t_student";
                            break;
                        case "tec":
                            table = "t_teacher";
                            break;
                    }
                    id = SqlHelper.ExecuteNonQuery("update " + table + " set pwd='" + CommonHelper.EncryptionMD5("111111") + "' where id = @id", new SqlParameter("@id", id));
                }
                return jss(new { status = 1, msg = "重置成功" });
            });
            //获取对应模板
            hs.Add("get_tpl", delegate()
            {
                string t = req("t");
                string Tpl = "";
                dynamic data = new { };
                switch (t)
                {
                    case "stu_add":
                        Tpl = "add_stu.html";
                        break;
                    case "stu_edit":
                        Tpl = "add_stu.html";
                        int id = Convert.ToInt32(req("id"));
                        if (id > 0)
                        {
                            DataTable dt = SqlHelper.ExecuteDataTable("select * from t_student where id =@id", new SqlParameter("@id", id));
                            data = dt.Rows[0];
                        }
                        break;
                    case "tec_add":
                        Tpl = "add_tec.html";
                        break;
                    case "tec_eidt":
                        Tpl = "edit_tec.html";
                        break;
                }
                //if (!System.IO.Directory.Exists("Tpl/" + Tpl)) return jss(new { status = 0, msg = "获取模板错误" });
                //else
                return jss(new { status = 1, data = CommonHelper.GetHtml("~/admin/Tpl", Tpl, new { data = data }) });
            });
        }




    }
}