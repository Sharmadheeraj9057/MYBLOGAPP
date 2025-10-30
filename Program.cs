using MyBlogApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Identity ke liye add karein

var builder = WebApplication.CreateBuilder(args);

// Database connection ko register karna (yeh pehle se tha)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// -- YEH NAYA CODE ADD KAREIN --
// Identity services ko register karna
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
// -- NAYA CODE KHATAM --

builder.Services.AddControllersWithViews();

// -- YEH NAYA CODE ADD KAREIN --
// Login/Register pages (jo Razor Pages hain) ko support karne ke liye
builder.Services.AddRazorPages();
// -- NAYA CODE KHATAM --


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// -- YEH NAYA CODE ADD KAREIN --
// Authentication ko activate karein
app.UseAuthentication();
// -- NAYA CODE KHATAM --

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -- YEH NAYA CODE ADD KAREIN --
// Login/Register pages ke routes ko map karein
app.MapRazorPages();
// -- NAYA CODE KHATAM --

app.Run();