using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireWeb.Models;

namespace FireWeb.Controllers
{
    public class AuctionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Exhibit()
        {
            return View();
        }

        public ActionResult Preview()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Preview(string Title, string Detail, DateTime StartTime, DateTime EndTime, int StartPrice, int DecidePrice, string Category, string Value, HttpPostedFileWrapper img1, HttpPostedFileWrapper img2, HttpPostedFileWrapper img3 , HttpPostedFileWrapper img4)
        {
            var item = new ItemModel();
            var TimeLimit = EndTime - DateTime.Now;
            var LimitAlart = item.GetLimitAlart(TimeLimit);

            var ImgList = item.WrapToImg(img1, img2, img3, img4);
            var ByteList = item.ImageToByteArray(ImgList);

            item.Title = Title;
            item.Detail = Detail;            
            item.StartPrice = StartPrice;
            item.DecidePrice = DecidePrice;
            item.Category = Category;
            item.Value = Value;
            item.BytePicture = ByteList;

            ViewBag.StartTime = StartTime.ToShortDateString();
            ViewBag.EndTime = EndTime.ToShortDateString();
            ViewBag.TimeLimit = TimeLimit;
            ViewBag.LimitAlart = LimitAlart;
            ViewBag.Year = EndTime.Year;
            ViewBag.Month = EndTime.Month;
            ViewBag.Day = EndTime.Day;

            return View(item);

        }

        public ActionResult ItemDetail(int ItemID)
        {

            var model = new ItemModel();
            model = model.GetItemDetail(ItemID);
            var LimitTime = model.EndTime - DateTime.Now;
            var LimitAlart = model.GetLimitAlart(LimitTime);

            ViewBag.StartTime = model.StartTime.ToShortDateString();
            ViewBag.EndTime = model.EndTime.ToShortDateString();
            ViewBag.TimeLimit = LimitTime;
            ViewBag.LimitAlart = LimitAlart;
            ViewBag.Year = model.EndTime.Year;
            ViewBag.Month = model.EndTime.Month;
            ViewBag.Day = model.EndTime.Day;

            return View(model);
        }
     

    
        public ActionResult ItemList()
        {

            var list = new List<ItemModel>();
            var model = new ItemModel();

            list = model.GetItemDatas();
            model.AddLimitAlarts(list);

            return View(list);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       

    }
}
