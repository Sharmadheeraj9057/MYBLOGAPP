using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Ise change karein
using Microsoft.EntityFrameworkCore;
using MyBlogApp.Models; 

namespace MyBlogApp.Data
{
    // Yahaan : DbContext ko : IdentityDbContext se badlein
    public class ApplicationDbContext : IdentityDbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Yeh line pehle se thi, ise rehne dein
        public DbSet<Post> Posts { get; set; } 
    }
}