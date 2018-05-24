using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FireWeb.Controllers
{
    public class ItemListController : Controller
    {
        // GET: ItemList
        public ActionResult Index()
        {
            return View();
        }

        // GET: ItemList/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ItemList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemList/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemList/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ItemList/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemList/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ItemList/Delete/5
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
