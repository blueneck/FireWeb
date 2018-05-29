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
        public ActionResult Preview(string Title, string Detail, DateTime StartTime, DateTime EndTime, int StartPrice, int DecidePrice, string Category, string Value, HttpPostedFileWrapper img1, HttpPostedFileWrapper img2, HttpPostedFileWrapper img3, HttpPostedFileWrapper img4)
        {
            var item = new ItemModel();
            var TimeLimit = EndTime - DateTime.Now;
            var LimitAlart = item.GetLimitAlart(TimeLimit);

            var PicPath = item.GetTmpImagePath(img1, img2, img3, img4);

            //var ImgList = item.WrapToImg(img1, img2, img3, img4);
            //var ByteList = item.ImageToByteArray(ImgList);

            item.Title = Title;
            item.Detail = Detail;
            item.StartPrice = StartPrice;
            item.DecidePrice = DecidePrice;
            item.Category = Category;
            item.Value = Value;
            item.PicPath = PicPath;
            item.StartTime = StartTime;
            item.EndTime = EndTime;

            Session["model"] = item;
      
            ViewBag.TimeLimit = TimeLimit;
            ViewBag.LimitAlart = LimitAlart;         

            return View(item);

        }

        public ActionResult Decide()
        {
            var model = (ItemModel)Session["model"];
            model.AddItem();
            return RedirectToAction("ItemList");
        }

        public ActionResult ItemDetail(String ItemID)
        {

            var model = new ItemModel();
            model = model.GetItemDetail(ItemID);
            var LimitTime = model.EndTime - DateTime.Now;
            var LimitAlart = model.GetLimitAlart(LimitTime);

            ViewBag.TimeLimit = LimitTime;
            ViewBag.LimitAlart = LimitAlart;           

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
