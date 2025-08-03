using BookStore.Models;
using Bogus;

namespace BookStore.Generation
{
    public class GenerateBook
    {

        public static Book GenerateRandomBook(int index, int seed, string lang, double likeCount, double reviewCount)
        {

            string composite = $"{seed}|{lang}|{index}";

            int hash = GenerationHashKey.GetHashSeedKey(composite);

            var randomizer = new Randomizer(hash);
            var faker = new Faker(lang) { Random = randomizer };

            int likes = GenerateNumber(faker, likeCount);
            int reviews = GenerateNumber(faker, reviewCount);

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
                Publisher = faker.Company.CompanyName() + ", " + faker.Random.Int(1970, 2025).ToString(),
                Likes = likes
            };

            book.Picture = GenerationPicture.GeneratePictureUri(seed, lang, index, book.Title, book.Authors);

            for (int r = 0; r < reviews; r++)
            {
                book.Reviews.Add(GenerateRandomReview(hash, lang, r));   
            }

            return book;
        }

        public static Review GenerateRandomReview(int hash, string lang, int index)
        {
            int reviewSeed = GenerationHashKey.GetHashSeedKey($"{hash}|review|{index}");
            var reviewRandomizer = new Randomizer(reviewSeed);
            var reviewFaker = new Faker(lang) { Random = reviewRandomizer };

            var review = new Review
            {
                Author = reviewFaker.Person.FullName,
                Description = reviewFaker.Lorem.Sentence(10)
            };

            return review;
        }

        public static int GenerateNumber(Faker faker, double number)
        {
            int result = (int)number;
            number -= (int)number;
            int limit = (int)(number * 10);
            int randNumber = faker.Random.Int(1, 10);
            if (randNumber < limit)
            {
                return result + 1;
            }
            else
            {
                return result;
            }
        }
    }
}
