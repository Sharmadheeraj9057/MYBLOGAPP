using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyBlogApp.Data;
using MyBlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO; // File operations ke liye zaroori
using Microsoft.AspNetCore.Hosting; // Web environment ke liye zaroori

namespace MyBlogApp.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- GET: Posts --- (No Change)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Posts.ToListAsync());
        }

        // --- GET: Posts/Details/5 --- (No Change)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }
            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null) { return NotFound(); }
            return View(post);
        }

        // --- GET: Posts/Create --- (No Change)
        public IActionResult Create()
        {
            return View();
        }

        // --- POST: Posts/Create --- (No Change)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,PublishedDate")] Post post, IFormFile? HeaderImage)
        {
            if (ModelState.IsValid)
            {
                if (HeaderImage != null && HeaderImage.Length > 0)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadsDir = Path.Combine(wwwRootPath, "images", "posts");
                    if (!Directory.Exists(uploadsDir)) { Directory.CreateDirectory(uploadsDir); }
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HeaderImage.FileName);
                    string filePath = Path.Combine(uploadsDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await HeaderImage.CopyToAsync(fileStream);
                    }
                    post.ImageUrl = "/images/posts/" + fileName;
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // --- GET: Posts/Edit/5 --- (No Change)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }
            var post = await _context.Posts.FindAsync(id);
            if (post == null) { return NotFound(); }
            return View(post);
        }

        // --- POST: Posts/Edit/5 --- (YEH UPDATE HUA HAI)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,PublishedDate,ImageUrl")] Post post, IFormFile? HeaderImage)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Puraana image path pehle se store kar lo (jo hidden field se aa raha hai)
                    string oldImagePath = post.ImageUrl;

                    if (HeaderImage != null && HeaderImage.Length > 0)
                    {
                        // 1. Nayi file save karein
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string uploadsDir = Path.Combine(wwwRootPath, "images", "posts");
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HeaderImage.FileName);
                        string filePath = Path.Combine(uploadsDir, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await HeaderImage.CopyToAsync(fileStream);
                        }
                        // 2. Post model ko naye path se update karein
                        post.ImageUrl = "/images/posts/" + fileName;

                        // 3. Puraani file (agar hai toh) delete karein
                        if (!string.IsNullOrEmpty(oldImagePath))
                        {
                            // Path ko OS ke hisaab se format karein ( / ko \ mein badlein agar Windows hai toh)
                            string oldAbsoluteImagePath = Path.Combine(wwwRootPath, oldImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldAbsoluteImagePath))
                            {
                                System.IO.File.Delete(oldAbsoluteImagePath);
                            }
                        }
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(e => e.Id == post.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // --- GET: Posts/Delete/5 --- (No Change)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound(); }
            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null) { return NotFound(); }
            return View(post);
        }

        // --- POST: Posts/Delete/5 --- (YEH UPDATE HUA HAI)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                // --- IMAGE DELETE LOGIC START ---
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // Path ko OS ke hisaab se format karein
                    string absoluteImagePath = Path.Combine(wwwRootPath, post.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(absoluteImagePath))
                    {
                        System.IO.File.Delete(absoluteImagePath);
                    }
                }
                // --- IMAGE DELETE LOGIC END ---

                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}