using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Model;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        RedisContext _manager = new RedisContext();

        [HttpGet]
        public IActionResult Index()
        {
            var customers = _manager.Get<List<Customers>>("customers");
            if(customers!=null)
            {
                customers = customers.OrderBy(x => x.Id).ToList();
                ViewBag.Customers = customers;
            }   
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(Customers model)
        {
            var customersList = _manager.Get<List<Customers>>("customers");

            if (customersList!=null)
            {
                var LastId = customersList.Max(x => x.Id);
                model.Id = LastId + 1;
            }
            else
            {
                model.Id = 1;
            }
            _manager.Set("customers", model);
            return RedirectToAction("Index");

        }

        public ActionResult Delete(int id)
        {
            var customer = _manager.Get<List<Customers>>("customers").Where(c=>c.Id==id).FirstOrDefault();
            var isRemoved = _manager.RemoveValue("customers", customer);
            return RedirectToAction("Index");
        }


        public ActionResult EditCustomer(int id)
        {
            var customer = _manager.Get<List<Customers>>("customers").Where(c => c.Id == id).FirstOrDefault();           
            return View(customer);
        }

        [HttpPost]
        public ActionResult EditCustomer(Customers customers)
        {
            Delete(customers.Id);
            _manager.Set("customers", customers);
            return RedirectToAction("Index");
        }


    }
}
