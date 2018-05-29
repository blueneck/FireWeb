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
using System.Drawing;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.IO;
using System.Net;

namespace FireWeb.Models
{
    public class ItemModel
    {
        private static readonly string connst = ConfigurationManager.ConnectionStrings["FireDB"].ConnectionString;

        public String Id { get; set; }
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
        //public List<Byte[]> BytePicture { get; set; }
        public List<String> PicPath { get; set; }
        public int LimitAlert { get; set; }


        public void AddItem()
        {
            using (var con = new MySqlConnection(connst))
            {
                Guid id = Guid.NewGuid();
                try
                {
                    con.Open();

                    var query = "";

                    var cmd = new MySqlCommand(query, con);

                    cmd.CommandText = $@"INSERT INTO items (
                    Id,Owner,Title,Detail,StartTime,EndTime,StartPrice,NowPrice,DecidePrice,Category,Value
                    )
                    VALUES(
                    @Id,@Owner,@Title,@Detail,@StartTime,@EndTime,@StartPrice,@NowPrice,@DecidePrice,@Category,@Value)";

                    cmd.Parameters.Add(new MySqlParameter("@Id", id));
                    cmd.Parameters.Add(new MySqlParameter("@Owner", this.Owner));
                    cmd.Parameters.Add(new MySqlParameter("@Title", this.Title));
                    cmd.Parameters.Add(new MySqlParameter("@Detail", this.Detail));
                    cmd.Parameters.Add(new MySqlParameter("@StartTime", this.StartTime));
                    cmd.Parameters.Add(new MySqlParameter("@EndTime", this.EndTime));
                    cmd.Parameters.Add(new MySqlParameter("@StartPrice", this.StartPrice));
                    cmd.Parameters.Add(new MySqlParameter("@NowPrice", this.StartPrice));
                    cmd.Parameters.Add(new MySqlParameter("@DecidePrice", this.DecidePrice));
                    //cmd.Parameters.Add(new MySqlParameter("@FinelPrice", this.FinalPrice));
                    cmd.Parameters.Add(new MySqlParameter("@Category", this.Category));
                    cmd.Parameters.Add(new MySqlParameter("@Value", this.Value));
                    //cmd.Parameters.Add(new MySqlParameter("@PicPath", this.PicPath));

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    con.Close();
                }

                foreach (String path in this.PicPath)
                {
                    try
                    {
                        con.Open();
                        var query = "";

                        var cmd = new MySqlCommand(query, con);

                        cmd.CommandText = $@"INSERT INTO images (
                    ItemID,path
                    )
                    VALUES(
                    @Id,@path)";

                        cmd.Parameters.Add(new MySqlParameter("@Id", id));
                        cmd.Parameters.Add(new MySqlParameter("@path", path));

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public ItemModel GetItemDetail(String id)
        {

            var list = new List<ItemModel>();

            using (var con = new MySqlConnection(connst))
            {
                con.Open();
                var query = $"SELECT * FROM items WHERE Id = '{id}';";
                var command = new MySqlCommand(query, con);
                var reader = command.ExecuteReader();
                while (reader.Read() == true)
                {
                    using (var Imgcon = new MySqlConnection(connst))
                    {
                        Imgcon.Open();

                        query = $"SELECT path FROM images where ItemID = '{reader["Id"].ToString()}';";
                        var Imgcommand = new MySqlCommand(query, Imgcon);
                        var Imgreader = Imgcommand.ExecuteReader();
                        var imagePath = new List<String>();

                        while (Imgreader.Read())
                        {
                            imagePath.Add(Imgreader["path"].ToString());
                        }
                        Imgcon.Close();

                        list.Add(new ItemModel
                        {
                            Id = reader["Id"].ToString(),
                            Owner = reader["Owner"].ToString(),
                            Title = reader["Title"].ToString(),
                            Detail = reader["Detail"].ToString(),
                            StartTime = (DateTime)reader["StartTime"],
                            EndTime = (DateTime)reader["EndTime"],
                            StartPrice = (int)reader["StartPrice"],
                            NowPrice = (int)reader["NowPrice"],
                            DecidePrice = (int)reader["DecidePrice"],
                            //FinalPrice = (int)reader["FinalPrice"],
                            Category = reader["Category"].ToString(),
                            Value = reader["Value"].ToString(),
                            PicPath = imagePath
                        });
                    }
                }
                con.Clone();
            }
            return list[0];
        }

        public List<ItemModel> GetItemDatas()
        {

            var list = new List<ItemModel>();

            using (var con = new MySqlConnection(connst))
            {
                con.Open();
                var query = "SELECT * FROM items order by PostTime desc;";
                var command = new MySqlCommand(query, con);
                var reader = command.ExecuteReader();
                
                using (var Imgcon = new MySqlConnection(connst))
                {
                    while (reader.Read())
                    {
                        Imgcon.Open();

                        query = $"SELECT path FROM images where ItemID = '{reader["Id"].ToString()}';";
                        var Imgcommand = new MySqlCommand(query, Imgcon);
                        var Imgreader = Imgcommand.ExecuteReader();
                        var imagePath = new List<String>();

                        while (Imgreader.Read())
                        {
                            imagePath.Add(Imgreader["path"].ToString());
                        }
                        Imgcon.Close();

                        list.Add(new ItemModel
                        {
                            Id = reader["Id"].ToString(),
                            Owner = reader["Owner"].ToString(),
                            Title = reader["Title"].ToString(),
                            Detail = reader["Detail"].ToString(),
                            StartTime = (DateTime)reader["StartTime"],
                            EndTime = (DateTime)reader["EndTime"],
                            StartPrice = (int)reader["StartPrice"],
                            NowPrice = (int)reader["NowPrice"],
                            DecidePrice = (int)reader["DecidePrice"],
                            //FinalPrice = (int)reader["FinalPrice"],
                            Category = reader["Category"].ToString(),
                            Value = reader["Value"].ToString(),
                            PicPath = imagePath
                        });                        
                    }
                    con.Clone();

                }
            }
            return list;
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

        public int GetLimitAlart(TimeSpan LimitTime)
        {
            var OneDay = new TimeSpan(1, 0, 0, 0);
            var HalfDay = new TimeSpan(0, 12, 0, 0);
            var OneHour = new TimeSpan(0, 1, 0, 0);

            var LimitAlart = 0;

            if (LimitTime < OneHour)
            {
                LimitAlart = 3;
            }
            else
            if (LimitTime < HalfDay)
            {
                LimitAlart = 2;
            }
            else
            if (LimitTime < OneDay)
            {
                LimitAlart = 1;
            }

            return LimitAlart;
        }

        public List<ItemModel> AddLimitAlarts(List<ItemModel> list)
        {
            var OneDay = new TimeSpan(1, 0, 0, 0);
            var HalfDay = new TimeSpan(0, 12, 0, 0);
            var OneHour = new TimeSpan(0, 1, 0, 0);
            var LimitAlart = 0;

            foreach (ItemModel m in list)
            {

                LimitAlert = 0;
                var LimitTime = m.EndTime - DateTime.Now;

                if (LimitTime < OneHour)
                {
                    LimitAlart = 3;
                }
                else
                if (LimitTime < HalfDay)
                {
                    LimitAlart = 2;
                }
                else
                if (LimitTime < OneDay)
                {
                    LimitAlart = 1;
                }

                m.LimitAlert = LimitAlert;

            }
            return list;
        }


        public List<Image> WrapToImg(HttpPostedFileWrapper img1, HttpPostedFileWrapper img2, HttpPostedFileWrapper img3, HttpPostedFileWrapper img4)
        {
            var list = new List<Image>();
            var tmp_list = new List<HttpPostedFileWrapper>();

            if (img1 != null)
            {
                tmp_list.Add(img1);
            }
            if (img2 != null)
            {
                tmp_list.Add(img2);
            }
            if (img3 != null)
            {
                tmp_list.Add(img3);
            }
            if (img4 != null)
            {
                tmp_list.Add(img4);
            }

            foreach (HttpPostedFileWrapper i in tmp_list)
            {
                string imageName = Guid.NewGuid().ToString("N").Substring(0, 10) + ".jpg";
                string path = @"C:\Users\t-tango\source\repos\FireWeb\FireWeb\tmp\" + imageName;
                i.SaveAs(path);
                list.Add(Image.FromFile(path));
            }
            return list;
        }

        public List<String> GetTmpImagePath(HttpPostedFileWrapper img1, HttpPostedFileWrapper img2, HttpPostedFileWrapper img3, HttpPostedFileWrapper img4)
        {
            var list = new List<String>();
            var tmp_list = new List<HttpPostedFileWrapper>();

            if (img1 != null)
            {
                tmp_list.Add(img1);
            }
            if (img2 != null)
            {
                tmp_list.Add(img2);
            }
            if (img3 != null)
            {
                tmp_list.Add(img3);
            }
            if (img4 != null)
            {
                tmp_list.Add(img4);
            }

            foreach (HttpPostedFileWrapper i in tmp_list)
            {
                string imageName = Guid.NewGuid().ToString("N").Substring(0, 10) + ".jpg";
                string savePath = @"C:\Users\t-tango\source\repos\FireWeb\FireWeb\tmp\" + imageName;
                i.SaveAs(savePath);
                string path = @"..\tmp\" + imageName;
                list.Add(path);
            }
            return list;
        }



        public static Image ByteArrayToImage(byte[] b)
        {
            ImageConverter imgconv = new ImageConverter();
            Image img = (Image)imgconv.ConvertFrom(b);
            return img;
        }


        public List<byte[]> ImageToByteArray(List<Image> imgs)
        {
            var list = new List<byte[]>();

            foreach (Image i in imgs)
            {
                ImageConverter imgconv = new ImageConverter();
                list.Add((byte[])imgconv.ConvertTo(i, typeof(byte[])));
            }
            return list;
        }

    }
}