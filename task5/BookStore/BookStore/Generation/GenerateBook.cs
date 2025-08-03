using BookStore.Models;
using Bogus;

namespace BookStore.Generation
{
    public class GenerateBook
    {

        public static Book GenerateRandomBook(int index, int seed, string lang, int reviewCount)
        {

            string composite = $"{seed}|{lang}|{index}";

            int hash = GenerationHashKey.GetHashSeedKey(composite);

            var randomizer = new Randomizer(hash);
            var faker = new Faker(lang) { Random = randomizer };

            int authorCount = faker.Random.Int(1, 3);
            var authors = new string[authorCount];

            for (int i = 0; i < authorCount; i++)
            {

                authors[i] = faker.Name.FullName();

            }

            var book = new Book
            {
                Id = index,
                Title = faker.Lorem.Sentence(3),
                Authors = authors,
                ISBN = faker.Random.Replace("###-#-##-######-#"),
                Publisher = faker.Company.CompanyName()
            };

            book.Picture = GenerationPicture.GeneratePictureUri(seed, lang, index, book.Title, book.Authors);

            for (int r = 0; r < reviewCount; r++)
            {
                int reviewSeed = GenerationHashKey.GetHashSeedKey($"{hash}|review|{r}");
                var reviewRandomizer = new Randomizer(reviewSeed);
                var reviewFaker = new Faker(lang) { Random = reviewRandomizer };

                var review = new Review
                {
                    Author = reviewFaker.Person.FullName,
                    Description = reviewFaker.Lorem.Sentence(10)
                };
                book.Reviews.Add(review);   
            }

            return book;
        }

    }
}
