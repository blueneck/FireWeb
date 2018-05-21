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
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
        public ActionResult Create(string userId , string password, string name, string mail, string remark)
        {
            var sql = new SqlClass();
            bool flg = false;

            if (sql.IsCheckUserID(userId) == false)
            {
                ViewBag.userIdMsg = "既に使われているユーザーIDです";
                flg = true;
            }

            if (sql.IsCheckPassword(password))
            {
                ViewBag.passwordMsg = "半角英小文字/大文字/数字をそれぞれ1種類以上含む6文字以上にしてください";
                flg = true;
            }

            if (sql.IsCheckMailAddress(mail))
            {
                ViewBag.mailMsg = "有効なメールアドレスではありません";
                flg = true;
            }
            if (flg)
            {
                return View();
            }
            

            sql.AddUserData(userId,password, name, mail, remark);
            return RedirectToAction("List", "Home");
        }

        public ActionResult List(int page = 1)
        {

            List<LoginModel> userLists = new List<LoginModel>();
            var sql = new SqlClass();
            var dt = sql.AgentDataset("select * from user order by id desc;");


            ////もうちょっとスマートにしたい
            foreach (DataRow dr in dt.Select())
            {
                userLists.Add(new LoginModel { id = int.Parse(dr.ItemArray[0].ToString()) ,userId = dr.ItemArray[1].ToString(), name = dr.ItemArray[3].ToString(), mail = dr.ItemArray[4].ToString(), remark = dr.ItemArray[5].ToString() });
            }

            var groups = userLists
                .Select((userList, index) => new { userList = userList, Index = index })
                .GroupBy(entry => entry.Index / 10, entry => entry.userList);

            ViewBag.NowPage = page;
            ViewBag.EndPage = groups.Count();

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
            var list = sql.GetUserData(id);
            return View(list[0]);
        }

        [HttpPost]
        public ActionResult Edit(int id, string userId,string password, string name, string mail, string remark)
        {
            var sql = new SqlClass();
            sql.UserDataUpdate(id, userId , password, name, mail, remark);
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
                    query = "SELECT * FROM user WHERE userId like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " userId like '%" + key + "%'";
                    }
                    query = query + " order by id desc";
                    break;

                case "name":
                    query = "SELECT * FROM user WHERE name like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " name like '%" + key + "%'";
                    }
                    query = query + " order by id desc";
                    break;

                case "mail":
                    query = "SELECT * FROM user WHERE mail like '%" + arr[0] + "%'";
                    foreach (string key in arr.Skip(1))
                    {
                        query = query + " " + searchType + " mail like '%" + key + "%'";
                    }
                    query = query + " order by id desc";
                    break;
            }

            List<LoginModel> userList = new List<LoginModel>();
            var sql = new SqlClass();
            var dt = sql.AgentDataset(query);

            foreach (DataRow dr in dt.Select())
            {
                userList.Add(new LoginModel { id = int.Parse(dr.ItemArray[0].ToString()), userId = dr.ItemArray[1].ToString(), name = dr.ItemArray[3].ToString(), mail = dr.ItemArray[4].ToString(), remark = dr.ItemArray[5].ToString() });
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
                return RedirectToAction("List", "Home");

            }
            else
            {
                ViewBag.LoginErrorMessage = "ユーザー名かパスワードが違います";

            }
            return View();
        }


        

        public ActionResult CsvExport()
        {
            var url = @"C:\Users\t-tango\source\repos\FireWeb\FireWeb\tmp\tmp.csv";
            string tmpfilePath = @"C:\Users\t-tango\source\repos\FireWeb\FireWeb\tmp\tmp.csv";
            string filePath = @"C:\Users\t-tango\Downloads\douwnload.csv";

            var sql = new SqlClass();
            var dt = sql.AgentDataset("select * from user;");
            DataTableToCsv(dt, tmpfilePath, true);

            var client = new WebClient();
            client.DownloadFile(url, filePath + ".csv");

            return RedirectToAction("List","Home");

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
        public ActionResult ImportCsv(HttpPostedFileWrapper uploadFile)
        {
            string fileName = Guid.NewGuid().ToString("N").Substring(0, 10);

            if (uploadFile != null)
            {
                uploadFile.SaveAs(Server.MapPath("~/tmp/importTmp.csv"));
            }

            List<string> row = null;
            var sql = new SqlClass();

            using (var csv = new CsvReader(Server.MapPath("~/tmp/importTmp.csv")))
            {
                while ((row = csv.ReadRow()) != null)
                {
                    if (sql.IsCheckUserID(row[1]) == false)
                    {
                        sql.AddUserData(row[1], row[2], row[3], row[4], row[5]);
                    }
                }
            }


            return RedirectToAction("List", "Home");

        }
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


        public bool IsCheckUserID(string userId)
        {
            userId = MySqlHelper.EscapeString(userId);
            var query = "select * from user where userId = '" + userId + "';";

            bool flg = false;
            try
            {
                using (var con = new MySqlConnection(connst))
                {
                    con.Open();
                    var command = new MySqlCommand(query, con);
                    var reader = command.ExecuteReader();                  
                    if (reader.Read() != true)
                    {
                        flg = true;
                    }
                    con.Clone();
                }
            }
            catch (Exception ex)
            {

            }
            return flg;
        }

        public bool IsCheckPassword(string password)
        {
            bool flg = false;
            bool result = Regex.IsMatch(password , @"^ (?=.*[0 - 9])(?=.*[a - z])(?=.*[A - Z])[0 - 9a - zA - Z\-]{ 6,}$");
            return flg;
        }



        public void AddUserData(string userId ,string password, string name, string mail, string remark)
        {
            string msg = "";
            using (var con = new MySqlConnection(connst))
            {
                userId = MySqlHelper.EscapeString(userId);
                password = MySqlHelper.EscapeString(password);
                mail = MySqlHelper.EscapeString(mail);
                name = MySqlHelper.EscapeString(name);
                remark = MySqlHelper.EscapeString(remark);

                con.Open();
                var query = "INSERT INTO user (userId,password,name,mail,remark) VALUES ( '" + userId + "','" +password + "', '" + name + "',  '" + mail + "', '" + remark + "');";
                var command = new MySqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Clone();
            }

        }

        public void UserDataUpdate(int id, string userId, string password, string name, string mail, string remark)
        {
            using (var con = new MySqlConnection(connst))
            {
                userId = MySqlHelper.EscapeString(userId);
                password = MySqlHelper.EscapeString(password);
                name = MySqlHelper.EscapeString(name);
                mail = MySqlHelper.EscapeString(mail);
                remark = MySqlHelper.EscapeString(remark);

                con.Open();
                var query = "UPDATE user SET userId = '" + userId + "', password = '" + password + "', name = '" + name + "', mail = '" + mail + "', remark = '" + remark + "' WHERE id = " + id.ToString() + ";";
                var command = new MySqlCommand(query, con);
                command.ExecuteNonQuery();
                con.Clone();
            }
        }



        public List<LoginModel> GetUserData(int id)
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
                    list.Add(new LoginModel { id = int.Parse(reader["id"].ToString()), userId = reader["userId"].ToString(), name = reader["name"].ToString(), password = reader["password"].ToString(), mail = reader["mail"].ToString(), remark = reader["remark"].ToString() });

                }
                con.Clone();
            }
            return list;
        }

        public bool IsCheckMailAddress(string address)
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

        


    }


    public class CsvReader : IDisposable
    {        
        private StreamReader stream = null;
        private bool isQuotedField = false;
        public CsvReader(string path) :
            this(path, Encoding.Default)
        {
        }
        public CsvReader(string path, Encoding encoding)
        {
            this.stream = new StreamReader(path, encoding);
        }

        public CsvReader(Stream stream)
        {
            this.stream = new StreamReader(stream);
        }

        public CsvReader(StringBuilder data)
        {
            var buffer = Encoding.Unicode.GetBytes(data.ToString());
            var memory = new MemoryStream(buffer);
            this.stream = new StreamReader(memory);
        }

        public List<List<string>> ReadToEnd()
        {
            var data = new List<List<string>>();
            var record = new List<string>();

            while ((record = this.ReadRow()) != null)
            {
                data.Add(record);
            }

            return data;
        }

        public Task<List<List<string>>> ReadToEndAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return this.ReadToEnd();
            });
        }

        public List<string> ReadRow()
        {
            var file = this.stream;
            var line = string.Empty;
            var record = new List<string>();
            var field = new StringBuilder();

            while ((line = file.ReadLine()) != null)
            {
                for (var i = 0; i < line.Length; i++)
                {
                    var item = line[i];

                    if (item == ',' && !this.isQuotedField)
                    {
                        record.Add(field.ToString());
                        field.Clear();
                    }
                    else if (item == '"')
                    {
                        if (!this.isQuotedField)
                        {
                            if (field.Length == 0)
                            {
                                this.isQuotedField = true;
                                continue;
                            }
                        }
                        else
                        {
                            if (i + 1 >= line.Length)
                            {
                                this.isQuotedField = false;
                                continue;
                            }
                        }

                        var peek = line[i + 1];

                        if (peek == '"')
                        {
                            field.Append('"');
                            i += 1;
                        }
                        else if (peek == ',' && this.isQuotedField)
                        {
                            this.isQuotedField = false;
                            i += 1;
                            record.Add(field.ToString());
                            field.Clear();
                        }
                    }
                    else
                    {
                        field.Append(item);
                    }
                }

                if (this.isQuotedField)
                {
                    field.Append(Environment.NewLine);
                }
                else
                {
                    record.Add(field.ToString());

                    return record;
                }
            }

            return null;
        }

        public Task<List<string>> ReadRowAsync()
        {
            return Task.Factory.StartNew<List<string>>(() =>
            {
                return this.ReadRow();
            });
        }

        public void Close()
        {
            if (this.stream == null)
            {
                return;
            }

            this.stream.Close();
        }

        public void Dispose()
        {
            if (this.stream == null)
            {
                return;
            }

            this.stream.Close();
            this.stream.Dispose();
            this.stream = null;
        }
    }
}
