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
        // GET: Exhibit
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
        public ActionResult Preview(string Title, string Detail, DateTime StartTime, DateTime EndTime, int StartPrice, int DecidePrice, string Category, string Value, List<Byte> BytePicture)
        {
            var TimeLimit = EndTime - DateTime.Now;
            var LimitAlart = GetLimitAlart(TimeLimit);

            


            ViewBag.Title = Title;
            ViewBag.Detail = Detail;
            ViewBag.StartTime = StartTime.ToShortDateString();
            ViewBag.EndTime = EndTime.ToShortDateString();
            ViewBag.StartPrice = StartPrice;
            ViewBag.DecidePrice = DecidePrice;
            ViewBag.Category = Category;
            ViewBag.Value = Value;
            ViewBag.BytePicture = BytePicture;
            ViewBag.TimeLimit = TimeLimit;
            ViewBag.LimitAlart = LimitAlart;

            return View();
        }

        public ActionResult ItemDetail()
        {
            try
            {
                var list = new List<ItemModel>();

                return View(list);
            }
            catch
            {
                return View();
            }
        }
     

        // GET: Exhibit/Delete/5
        public ActionResult ItemList()
        {
            return View();
        }

        // POST: Exhibit/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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

    }
}
