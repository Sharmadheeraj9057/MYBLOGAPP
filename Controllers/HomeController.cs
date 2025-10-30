using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyBlogApp.Models;
using MyBlogApp.Data; // Ise add karein
using Microsoft.EntityFrameworkCore; // Ise add karein

namespace MyBlogApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context; // Context ko register karein

    // Constructor ko modify karein
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Index action ko modify karein
    public async Task<IActionResult> Index()
    {
        // Database se saare posts fetch karein (naye se puraane order mein)
        var posts = await _context.Posts.OrderByDescending(p => p.PublishedDate).ToListAsync();
        return View(posts); // Posts ko view mein pass karein
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}