using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebService.Models;

namespace WebService.Controllers
{
    public class ExampleController : Controller
    {
        public ActionResult DoSomething()
        {
            return View();
        }

        public ActionResult DoSomethingElse(Person person)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                while (context.People.Count() > 0)
                    context.People.Remove(context.People.First());

                context.SaveChanges();

                return View(context.People.ToList());
            }
        }
    }
}