using AccountMgmt.Models;
using log4net.Config;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DI for DbContext
builder.Services.AddDbContext<TransactionDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



//app.UseCsp(options => options
//        .DefaultSources(s => s.Self())
//        .ScriptSources(s => s.Self().CustomSources("https://trusted-cdn.com")));


app.Use(async (context, next) =>
{

    //context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    //context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    //context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'; font-src 'self'; img-src 'self'; frame-src 'self'");
    context.Response.Headers.Append("Content-Security-Policy-Report-Only", "default-src 'self'; script-src 'self' https://code.jquery.com https://cdnjs.cloudflare.com https://stackpath.bootstrapcdn.com https://cdn.jsdelivr.net; style-src 'self' https://stackpath.bootstrapcdn.com https://cdnjs.cloudflare.com; font-src 'self' https://cdnjs.cloudflare.com; img-src 'self'; frame-src 'self'");

    await next();  // Call the next middleware in the pipeline
});






app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();