using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Minimal API",
        Description = "Testing the minimal APIs and new features that dotnet 7 brings.",
        TermsOfService = new Uri("https://github.com/PatrickZurita"),
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Contact with me",
            Url = new Uri("https://www.linkedin.com/in/patrick-zurita-tenorio-9470241b0/"),
            Email = "ingsoftwarezt@gmail.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://github.com/PatrickZurita")
        }
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var users = new User[]
{
    new User(1),
    new User(2),
    new User(3)
};

app.MapGet("/users/{quantity})", (int quantity) =>
{
    return users.Take(quantity);
});

app.MapPost("/users",() => {
    foreach (User u in users)
    {
        u.id = u.id + 1;   
    }
});

app.Run();

internal record User(int id);