using CivicLink.Services;

var builder = WebApplication.CreateBuilder(args);

// Added my services to the container here
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IIssueService, IssueService>();
builder.Services.AddSingleton<IGamificationService, GamificationService>();
builder.Services.AddSingleton<IEventService, EventService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
