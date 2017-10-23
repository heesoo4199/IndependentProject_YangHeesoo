using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebService.Models
{
    public class TodoList
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        public bool Public { get; set; }

        public string User { get; set; }

        public virtual ICollection<TodoListItem> Items { get; set; }
    }
}