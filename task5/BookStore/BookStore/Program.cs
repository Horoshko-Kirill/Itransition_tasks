var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Urls.Add($"http://*:{Environment.GetEnvironmentVariable("PORT") ?? "5000"}");

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseStaticFiles();

app.MapGet("/api/books", (string seed, string lang, int offset = 0, int limit = 20, double likes = 5.5, double reviews = 3.5) =>
{
    var list = new List<Book>();
    for (int i = offset; i < offset + limit; i++)
    {
        var book = GenerateBook.GenerateRandomBook(i, int.TryParse(seed, out var s) ? s : 0, lang, likes, reviews);
        list.Add(book);
    }
    return Results.Json(new
    {
        seed,
        lang,
        offset,
        limit,
        likes,
        reviews,
        items = list
    });
});

app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();
