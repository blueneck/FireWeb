using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Configuration;
using FireWeb.Models;
using System.Data;
using System.Text;
using System.IO;

namespace FireWeb.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Login Form";

            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Create(string password, string name, string mail, string remark)
        {
            var sql = new SqlClass();
            sql.AddUser(password, name, mail, remark);
            return RedirectToAction("List", "Home");
        }

        public ActionResult List(int page = 1)
        {

            List<LoginModel> userLists = new List<LoginModel>();
            var sql = new SqlClass();
            var dt = sql.AgentDataset("select * from user;");


            ////もうちょっとスマートにしたい
            foreach (DataRow dr in dt.Select())
            {
                userLists.Add(new LoginModel { id = int.Parse(dr.ItemArray[0].ToString()), password = dr.ItemArray[1].ToString(), name = dr.ItemArray[2].ToString(), mail = dr.ItemArray[3].ToString(), remark = dr.ItemArray[4].ToString() });

            }

            var groups = userLists
                .Select((userList, index) => new { userList = userList, Index = index })
                .GroupBy(entry => entry.Index / 10, entry => entry.userList);

            ViewBag.firstPage = 1;
            ViewBag.backPage = page - 1;
            ViewBag.nextPage = page + 1;
            ViewBag.endPage = groups.Count();

            return View(groups.ElementAt(page - 1));
        }

        [HttpPost]
        public ActionResult List(string column, string keyword, string searchType)
        {
            var userList = Search(column, keyword, searchType);
            return View(userList);
        }


        public ActionResult Edit(int id)
        {
            var sql = new SqlClass();
            var list = sql.AgentUserSearch(id);
            return View(list);
        }

        [HttpPost]
        public ActionResult Edit(int id, string password, string name, string mail, string remark)
        {
            var sql = new SqlClass();
            sql.AgentUserEdit(id, password, name, mail, remark);
            return RedirectToAction("List", "Home");
        }



        public ActionResult Delete()
        {


            return View();
        }

        public List<LoginModel> Search(string column, string keyword, string searchType)
        {
            Session["Searchword"] = keyword;
            string[] arr = keyword.Split(' ');
            var query = "";

            switch (column)
            {
                case "id":
                    query = "SELECT * FROM user WHERE id like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " id like '%" + key + "%'";
                    }
                    break;

                case "name":
                    query = "SELECT * FROM user WHERE name like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " name like '%" + key + "%'";
                    }
                    break;

                case "mail":
                    query = "SELECT * FROM user WHERE mail like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " mail like '%" + key + "%'";
                    }
                    break;
            }

            List<LoginModel> userList = new List<LoginModel>();
            var sql = new SqlClass();
            var dt = sql.AgentDataset(query);

            foreach (DataRow dr in dt.Select())
            {
                userList.Add(new LoginModel { id = int.Parse(dr.ItemArray[0].ToString()), password = dr.ItemArray[1].ToString(), name = dr.ItemArray[2].ToString(), mail = dr.ItemArray[3].ToString(), remark = dr.ItemArray[4].ToString() });

            }

            return userList;
        }



        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            var sql = new SqlClass();
            var auth = sql.LoginAuth(password, name);

            if (auth == true)
            {
                //ここでセッション作成
                return RedirectToAction("List", "Home");

            }
            else
            {
                ViewBag.LoginErrorMessage = "ユーザー名かパスワードが違います";

            }
            return View();
        }


        public static bool IsValidMailAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return false;
            }

            try
            {
                System.Net.Mail.MailAddress a =
                    new System.Net.Mail.MailAddress(address);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        [HttpGet]
        public ActionResult ViewSession()
        {
            GetSessionValue();
            return View();
        }

        [HttpGet]
        public ActionResult InputSession()
        {
            Session["Drink"] = "Coffe";

            GetSessionValue();
            return View("ViewSession");
        }

        [HttpGet]
        public ActionResult RemoveAll()
        {
            Session.RemoveAll();

            GetSessionValue();
            return View("ViewSession");
        }

        [HttpGet]
        public ActionResult Abandon()
        {
            Session.Abandon();
            GetSessionValue();
            return View("ViewSession");
        }

        private void GetSessionValue()
        {
            object drink = Session["Drink"];
            ViewBag.Drink = drink;
        }

        public void CsvExport()
        {
            string filePath = @"C:\Users\t-tango\Downloads\douwnload.csv";

            var sql = new SqlClass();
            var dt = sql.AgentDataset("select * from user;");
            DataTableToCsv(dt, filePath, true);


        }

        static public void DataTableToCsv(DataTable dt, string filePath, bool header)
        {
            string sp = string.Empty;
            List<int> filterIndex = new List<int>();

            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    switch (dt.Columns[i].DataType.ToString())
                    {
                        case "System.Boolean":
                        case "System.Byte":
                        case "System.Char":
                        case "System.DateTime":
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.SByte":
                        case "System.Single":
                        case "System.String":
                        case "System.TimeSpan":
                        case "System.UInt16":
                        case "System.UInt32":
                        case "System.UInt64":
                            break;

                        default:
                            filterIndex.Add(i);
                            break;
                    }
                }
                if (header)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        sw.Write(sp + "\"" + col.ToString().Replace("\"", "\"\"") + "\"");
                        sp = ",";
                    }
                    sw.WriteLine();
                }
                foreach (DataRow row in dt.Rows)
                {
                    sp = string.Empty;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (filterIndex.Contains(i))
                        {
                            sw.Write(sp + "\"[データ]\"");
                            sp = ",";
                        }
                        else
                        {
                            sw.Write(sp + "\"" + row[i].ToString().Replace("\"", "\"\"") + "\"");
                            sp = ",";
                        }
                    }
                    sw.WriteLine();
                }
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExportDownloaded()
        //{
        //    // DB からデータ取得
        //    //var parentList = db.Parents.ToList();

        //    //// CSV 内容生成
        //    //var csvString = CsvService.CreateCsv(parentList);

        //    //// クライアントにダウンロードさせる形で CSV 出力
        //    //var fileName = string.Format("マスタデータ_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    //// IE で全角が文字化けするため、ファイル名を UTF-8 でエンコーディング
        //    //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", HttpUtility.UrlEncode(fileName, Encoding.UTF8)));
        //    //return Content(csvString, "text/csv", Encoding.GetEncoding("Shift_JIS"));
        //}
    }

    public class SqlClass
    {
        private static readonly string connst = ConfigurationManager.ConnectionStrings["FireDB"].ConnectionString;

        public DataTable AgentDataset(string query)
        {
            var list = new List<string>();
            var dt = new DataTable();

            using (var con = new MySqlConnection(connst))
            {

                var command = new MySqlCommand(query, con);
                var adapter = new MySqlDataAdapter(command);
                adapter.Fill(dt);

            }
            return dt;
        }

        public bool LoginAuth(string password, string name)
        {
            string pass = MySqlHelper.EscapeString(password);
            string rname = MySqlHelper.EscapeString(name);
            var query = "select * from user where name = '" + pass + "' AND password = '" + rname + "';";

            bool flg = false;
            try
            {
                using (var con = new MySqlConnection(connst))
                {
                    var command = new MySqlCommand(query, con);
                    if (command.ExecuteScalar() != null)
                    {
                        flg = true;
                    }
                }
            }
            catch
            {

            }
            return flg;
        }

        public void AddUser(string password, string name, string mail, string remark)
        {


            using (var con = new MySqlConnection(connst))
            {
                password = MySqlHelper.EscapeString(password);
                name = MySqlHelper.EscapeString(name);
                mail = MySqlHelper.EscapeString(mail);
                remark = MySqlHelper.EscapeString(remark);

                con.Open();
                var query = "INSERT INTO user (password,name,mail,remark) VALUES ( '" + password + "', '" + name + "',  '" + mail + "', '" + remark + "');";
                var command = new MySqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Clone();
            }

        }

        public void AgentUserEdit(int id, string password, string name, string mail, string remark)
        {
            using (var con = new MySqlConnection(connst))
            {
                password = MySqlHelper.EscapeString(password);
                name = MySqlHelper.EscapeString(name);
                mail = MySqlHelper.EscapeString(mail);
                remark = MySqlHelper.EscapeString(remark);

                var query = "UPDATE user SET password = '" + password + "', name = '" + name + "', mail = '" + mail + "', remark = '" + remark + "' WHERE id = " + id.ToString() + ";";
                var command = new MySqlCommand(query, con);
            }
        }



        public List<LoginModel> AgentUserSearch(int id)
        {

            var list = new List<LoginModel>();

            using (var con = new MySqlConnection(connst))
            {
                con.Open();
                var query = "SELECT * FROM user WHERE id = " + id.ToString() + ";";
                var command = new MySqlCommand(query, con);
                var reader = command.ExecuteReader();
                while (reader.Read() == true)
                {
                    list.Add(new LoginModel { id = int.Parse(reader["id"].ToString()), name = reader["name"].ToString(), password = reader["password"].ToString(), mail = reader["mail"].ToString(), remark = reader["remark"].ToString() });

                }
                con.Clone();
            }
            return list;
        }



    }
}
