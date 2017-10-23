using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebService.Models;

namespace WebService.Controllers
{
    [Authorize]
    public class TodoListItemsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: TodoListItems
        public ActionResult Index(string listName)
        {
            var todoListItems = db.TodoListItems
                .Where(t => t.TodoList.User == User.Identity.Name)
                .Include(t => t.TodoList);

            if (listName != null)
            {
                todoListItems = todoListItems
                    .Where(t => t.TodoList.Name == listName);
            }

            return View(todoListItems
                .OrderBy(t => t.DueDate)
                .ToList());
        }

        // GET: TodoListItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoListItem todoListItem = db.TodoListItems.Find(id);
            if (todoListItem == null)
            {
                return HttpNotFound();
            }
            if (todoListItem.TodoList.User != User.Identity.Name)
            {
                return new HttpUnauthorizedResult();
            }
            return View(todoListItem);
        }

        // GET: TodoListItems/Create
        public ActionResult Create()
        {
            ViewBag.TodoListId = new SelectList(
                db.TodoLists.Where(t => t.User == User.Identity.Name), 
                "Id", "Name");

            return View();
        }

        // POST: TodoListItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TodoListId,Description,DueDate,Completed")] TodoListItem todoListItem)
        {
            var todoList = db.TodoLists.Find(todoListItem.TodoListId);
            if (todoList.User != User.Identity.Name)
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.TodoListItems.Add(todoListItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }



            ViewBag.TodoListId = new SelectList(
                db.TodoLists.Where(t => t.User == User.Identity.Name), "Id", "Name", todoListItem.TodoListId);
            return View(todoListItem);
        }

        // GET: TodoListItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoListItem todoListItem = db.TodoListItems.Find(id);
            if (todoListItem == null)
            {
                return HttpNotFound();
            }

            ViewBag.TodoListId = new SelectList(db.TodoLists, "Id", "Name", todoListItem.TodoListId);
            return View(todoListItem);
        }

        // POST: TodoListItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TodoListId,Description,DueDate,Completed")] TodoListItem todoListItem)
        {
            var todoList = db.TodoLists.Find(todoListItem.TodoListId);
            if (todoList.User != User.Identity.Name)
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                db.Entry(todoListItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TodoListId = new SelectList(db.TodoLists, "Id", "Name", todoListItem.TodoListId);
            return View(todoListItem);
        }

        // GET: TodoListItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodoListItem todoListItem = db.TodoListItems.Find(id);
            if (todoListItem == null)
            {
                return HttpNotFound();
            }
            if (todoListItem.TodoList.User != User.Identity.Name)
            {
                return new HttpUnauthorizedResult();
            }
            return View(todoListItem);
        }

        // POST: TodoListItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TodoListItem todoListItem = db.TodoListItems.Find(id);
            db.TodoListItems.Remove(todoListItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
