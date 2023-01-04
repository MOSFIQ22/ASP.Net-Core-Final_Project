using Microsoft.EntityFrameworkCore;
using MVC_Core_Mid_Monthly_1268474.HostedServices;
using MVC_Core_Mid_Monthly_1268474.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CourseDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("db")));
builder.Services.AddHostedService<DbSeederHostedService>();
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
var app = builder.Build();

app.UseStaticFiles();
app.MapDefaultControllerRoute();


app.Run();
