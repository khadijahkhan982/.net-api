using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

public class Blog
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Body is required")]
    [StringLength(5000, MinimumLength = 10, ErrorMessage = "Body must be between 10 and 5000 characters")]
    public string? Body { get; set; }
}

public partial class Program
{
    private const string ApiKeyHeaderName = "X-Password";
    private const string ApiKeyValue = "mysecretpassword011";
    
    private static List<Blog> _blogs = new List<Blog>
    {
        new Blog { Id = 1, Title = "First Blog Post", Body = "Initial content." }
    };

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/blogs", StringComparison.OrdinalIgnoreCase) && 
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var actualValue) || 
                    actualValue.FirstOrDefault() != ApiKeyValue)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized: Missing or invalid verification token.");
                    return; 
                }
            }
            await next.Invoke();
        });

        app.Use(async (context, next) =>
        {
            var startTime = DateTime.UtcNow;
            Console.WriteLine($"Start Time: {startTime}");
            Console.WriteLine($"Request Path: {context.Request.Path}");
            await next.Invoke();
            var duration = DateTime.UtcNow - startTime;
            Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
            Console.WriteLine($"Response Time: {duration.TotalMilliseconds} ms");
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        }
        
        app.MapPost("/blogs", (Blog newBlog) =>
        {
            // Validate the model
            var context = new ValidationContext(newBlog, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(newBlog, context, results, validateAllProperties: true);

            if (!isValid)
            {
                var errors = results.Select(r => r.ErrorMessage).ToList();
                return Results.BadRequest(new { errors });
            }

            newBlog.Id = _blogs.Count > 0 ? _blogs.Max(b => b.Id) + 1 : 1;
            _blogs.Add(newBlog);
            return Results.Created($"/blogs/{newBlog.Id}", newBlog);
        });

        app.MapGet("/blogs", () => 
        {
            return Results.Ok(_blogs);
        });

        app.MapGet("/blogs/{id}", (int id) =>
        {
            var blog = _blogs.FirstOrDefault(b => b.Id == id);
            return blog is not null ? Results.Ok(blog) : Results.NotFound();
        });

        app.MapPut("/blogs/{id}", (int id, Blog updatedBlog) =>
        {
            var existingBlog = _blogs.FirstOrDefault(b => b.Id == id);
            if (existingBlog is null) return Results.NotFound();

            // Validate the model
            var context = new ValidationContext(updatedBlog, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(updatedBlog, context, results, validateAllProperties: true);

            if (!isValid)
            {
                var errors = results.Select(r => r.ErrorMessage).ToList();
                return Results.BadRequest(new { errors });
            }

            existingBlog.Title = updatedBlog.Title;
            existingBlog.Body = updatedBlog.Body;
            return Results.NoContent();
        });

        app.MapDelete("/blogs/{id}", (int id) =>
        {
            var blogToRemove = _blogs.FirstOrDefault(b => b.Id == id);
            if (blogToRemove is null) return Results.NotFound();

            _blogs.Remove(blogToRemove);
            return Results.NoContent();
        });
        
        await Task.Run(() => app.RunAsync());
    }
}