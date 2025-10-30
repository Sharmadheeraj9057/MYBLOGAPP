using System.ComponentModel.DataAnnotations;
// using Microsoft.AspNetCore.Mvc.ModelBinding;  <-- YEH LINE HATA DI GAYI HAI

namespace MyBlogApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty; // <-- WARNING FIX 1

        [Required]
        // [AllowHtml] <-- YEH LINE HATA DI GAYI HAI (MERI GALTI THI)
        public string Content { get; set; } = string.Empty; // <-- WARNING FIX 2

        [Display(Name = "Header Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }
    }
}