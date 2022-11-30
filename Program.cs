using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

#region SWAGGER_PROGRAM_CONFIG

builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Add information of your API
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
app.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

#endregion

#region MINIMAL_API

var users = new User[] // A sample array of users to test new features
{
    new User(1, "Patrick", 123321),
    new User(2, "Dwuart", 321123),
    new User(3 , "Lil", 987789)
};

// MapGET provided by ASP.NET Core
app.MapGet("/GetUserByID/{_id})",(int _id) => {
    return
        from us in users
        where us.id == _id
        select us;
}).AddEndpointFilter(async (context, next) => {
    int _id = context.GetArgument<int>(0);

    if (_id <= 0 || _id > users.Length){
        return Results.Problem("The id must be greater than 0 and be less than total records id saved."); // AddEndpointFilter
    }
    
    return await next(context);
});

// MapGet provided by ASP.NET Core
// This endpoint will have filters, this a new feature of .NET 7
app.MapGet("/users/{quantity})", (int quantity) =>
{
    return users.Take(quantity);
}).AddEndpointFilter(async (context, next) => {
    int quantity = context.GetArgument<int>(0);

    if (quantity <= 0) {
        return Results.Problem("The quantity must be greater than 0."); // AddEndpointFilter
    }

    return await next(context);
}).AddEndpointFilter<MyFilter>(); // Add other filter

internal record User(int id, string name, int dni); // A sample user to test new features

public class MyFilter : IEndpointFilter // Implement Endopint Filter Class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int quantity = context.GetArgument<int>(0);

        if (quantity > 20) {
            return Results.Problem("The quantity must be less than 20."); 
        }
        
        return await next(context);    
    }
}

#endregion