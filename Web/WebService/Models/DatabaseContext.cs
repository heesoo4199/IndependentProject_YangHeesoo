using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebService.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public DbSet<TodoList> TodoLists { get; set; }

        public DbSet<TodoListItem> TodoListItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}