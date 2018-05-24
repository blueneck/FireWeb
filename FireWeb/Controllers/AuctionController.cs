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
            ViewBag.Title = Title;
            ViewBag.Detail = Detail;
            ViewBag.StartTime = StartTime;
            ViewBag.EndTime = EndTime;
            ViewBag.StartPrice = StartPrice;
            ViewBag.DecidePrice = DecidePrice;
            ViewBag.Category = Category;
            ViewBag.Value = Value;
            ViewBag.BytePicture = BytePicture;

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
    }
}
