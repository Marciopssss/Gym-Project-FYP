using Gym_Membership.Data;
using Gym_Membership.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// ✅ Add HttpContextAccessor for session use
builder.Services.AddHttpContextAccessor();

// ✅ Add Session
builder.Services.AddSession();

// ✅ Register your DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ✅ Create admin if not exists
    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        var admin = new User
        {
            Username = "admin",
            Password = "admin123", // 🔒 you can later hash this
            Email = "admin@gym.com",
            FullName = "System Administrator",
            Role = "Admin"
        };

        db.Users.Add(admin);
        db.SaveChanges();
    }
}


app.Run();
