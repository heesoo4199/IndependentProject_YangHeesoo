using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class TodoListItem
    {
        public int Id { get; set; }

        public int TodoListId { get; set; }

        public TodoList TodoList { get; set; }

        public string Description { get; set; }

        public DateTimeOffset DueDate { get; set; }

        public bool Completed { get; set; }
    }
}