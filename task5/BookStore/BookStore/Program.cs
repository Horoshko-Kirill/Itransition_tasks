

using BookStore.Generation;
using BookStore.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", (int seed, string lang, int index, int reviews) =>
{
    var book = GenerateBook.GenerateRandomBook(index, seed, lang, reviews);

    var reviewsHtml = string.Join("", book.Reviews.Select(r => $@"
        <li>
            <strong>{r.Author}</strong> 
            <em>{r.Description}</em>
        </li>"));

    var html = $@"
    <html>
    <head><title>Book Info</title></head>
    <body>
        <h1>{book.Title}</h1>
        <p><strong>Author(s):</strong> {string.Join(", ", book.Authors)}</p>
        <p><strong>Language:</strong> {lang}</p>
<img src='{book.Picture}' alt='Book Cover' style='max-width:300px; height:auto;' />
        <p><strong>Reviews:</strong></p>
        <ul>
            {reviewsHtml}
        </ul>
    </body>
    </html>";

    return Results.Content(html, "text/html");
});




app.Run();
