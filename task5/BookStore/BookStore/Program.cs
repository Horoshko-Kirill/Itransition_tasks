using BookStore.Generation;
using BookStore.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.MapGet("/api/books", (HttpRequest req, ILogger<Program> logger) =>
{
    string seed = req.Query["seed"];
    string lang = req.Query["lang"];
    int offset = int.TryParse(req.Query["offset"], out var o) ? o : 0;
    int limit = int.TryParse(req.Query["limit"], out var l) ? l : 20;
    double likes = double.TryParse(req.Query["likes"], out var li) ? li : 5.5;
    double reviews = double.TryParse(req.Query["reviews"], out var r) ? r : 3.5;

    if (string.IsNullOrWhiteSpace(seed)) seed = "0";
    if (string.IsNullOrWhiteSpace(lang)) lang = "en";

    logger.LogInformation("Request /api/books seed={Seed} lang={Lang} offset={Offset} limit={Limit} likes={Likes} reviews={Reviews}",
        seed, lang, offset, limit, likes, reviews);

    var list = new List<Book>();
    for (int i = offset; i < offset + limit; i++)
    {
        try
        {
            var book = GenerateBook.GenerateRandomBook(i, int.TryParse(seed, out var s) ? s : 0, lang, likes, reviews);
            list.Add(book);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при генерации книги index={Index}", i);
\
            list.Add(new Book
            {
                Id = i,
                Title = $"<error generating book {i}>",
                Authors = new[] { "error" },
                Publisher = "error",
                ISBN = "error"
            });
        }
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
