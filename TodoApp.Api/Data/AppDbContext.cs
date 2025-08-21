using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.DTO;

namespace TodoApp.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<TodoItem> Todos => Set<TodoItem>();
        public DbSet<JournalEntry> Journals => Set<JournalEntry>();
    }
}