using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
            if ((bool)Session["AuthExhibid"]){
                return View();
            }
            else
            {
                return RedirectToAction("ItemList");
            }
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
            var owner = (List<string>)Session["loginAuth"];
            item.Owner = owner[1];
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
            var count = model.GetBitCount(ItemID);
            var LimitTime = model.EndTime - DateTime.Now;
            var LimitAlart = model.GetLimitAlart(LimitTime);

            ViewBag.TimeLimit = LimitTime;
            ViewBag.LimitAlart = LimitAlart;
            ViewBag.Count = count;

            return View(model);
        }

        public ActionResult ItemList()
        {
            var model = new ItemModel();
            var list = model.GetItemDatas();
            model.AddLimitAlarts(list);

            return View(list);
        }

        [HttpPost]
        public ActionResult ItemList(string userID, string column, string keyword, string searchType, bool searchStatus, string searchU, string sortColumn, string sortType)
        {
            var list = new List<ItemModel>();
            var model = new ItemModel();

            list = model.search(userID, column, keyword, searchType, searchStatus, searchU, sortColumn, sortType);
            model.AddLimitAlarts(list);

            return View(list);
        }

        [HttpPost]
        public ActionResult Did(String ItemID, int bidPrice,int NowPrice,int DecidePrice)
        {
            try
            {
                using (var tScope = new TransactionScope())
                {
                    var model = new ItemModel();
                    model.AddDid(ItemID, bidPrice, NowPrice, DecidePrice, Session["userName"].ToString());
                }
                return RedirectToAction("ItemDetail/" + ItemID);
            }
            catch
            {
                return RedirectToAction("ItemDetail/" + ItemID);
            }
        }


    }
}
