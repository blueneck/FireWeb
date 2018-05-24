using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing;
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
using System.Data.SqlClient;

namespace FireWeb.Models
{
    public class ItemModel
    {
        private static readonly string connst = ConfigurationManager.ConnectionStrings["FireDB"].ConnectionString;

        public int Id { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StartPrice { get; set; }
        public int NowPrice { get; set; }
        public int DecidePrice { get; set; }
        public int FinalPrice { get; set; }
        public string Category { get; set; }
        public string Value { get; set; }
        public List<Byte> BytePicture { get; set; }


        public void AddItem(string Title, string Detail, DateTime StartTime, DateTime EndTime, int StartPrice, int DecidePrice, string Category, string Value, List<Byte> BytePicture)
        {
            string msg = "";
            using (var con = new MySqlConnection(connst))
            {
                Guid Id = Guid.NewGuid();
                StringBuilder query = new StringBuilder(128);

                //string sqlstr = "insert into Products (name, price, category) values (@name, @price, @category)";

                //con.Open();
               

                //    query.Append("INSERT INTO ");
                //query.Append("user ");
                //query.Append("(Id,Title,name,mail,remark) ");
                //query.Append("VALUES ");
                //query.Append("DENPYO ");
                //query.Append("WHERE ");
                //query.Append("CHUMONBI >= @DATE");

                //con.Open();
                //var query = "INSERT INTO user (Id,Title,name,mail,remark) VALUES ( '" + userId + "','" + password + "', '" + name + "',  '" + mail + "', '" + remark + "');";
                //var command = new MySqlCommand(query, con);
                //command.ExecuteNonQuery();
                //con.Clone();
            }

        }


        public DataTable GetToDataset(string query)
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



        // バイト配列をImageオブジェクトに変換
        public static Image ByteArrayToImage(byte[] b)
        {
            ImageConverter imgconv = new ImageConverter();
            Image img = (Image)imgconv.ConvertFrom(b);
            return img;
        }

        // Imageオブジェクトをバイト配列に変換
        public static byte[] ImageToByteArray(Image img)
        {
            ImageConverter imgconv = new ImageConverter();
            byte[] b = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
            return b;
        }

    }
}