using Common.Models;
using MongoDB.Driver;

namespace CatalogAPI.Data
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (!existProduct)
            {
                productCollection.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>()
            {
                new Product()
                {
                    Id = new Guid(),
                    Name = "Designing Data-Intensive Applications: The Big Ideas Behind Reliable, Scalable, and Maintainable Systems",
                    Category = "IT",
                    Summary = "Data is at the center of many challenges in system design today. Difficult issues need to be figured out, such as scalability, consistency, reliability, efficiency, and maintainability. In addition, we have an overwhelming variety of tools, including relational databases, NoSQL datastores, stream or batch processors, and message brokers. What are the right choices for your application? How do you make sense of all these buzzwords?",
                    ImageFile = "https://m.media-amazon.com/images/I/51ZSpMl1-LL._SX379_BO1,204,203,200_.jpg",
                    Price = 32.17m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "Building Microservices: Designing Fine-Grained Systems 2nd Edition",
                    Category = "IT",
                    Summary = "As organizations shift from monolithic applications to smaller, self-contained microservices, distributed systems have become more fine-grained. But developing these new systems brings its own host of problems. This expanded second edition takes a holistic view of topics that you need to consider when building, managing, and scaling microservices architectures.\r\n\r\nThrough clear examples and practical advice, author Sam Newman gives everyone from architects and developers to testers and IT operators a firm grounding in the concepts. You'll dive into the latest solutions for modeling, integrating, testing, deploying, and monitoring your own autonomous services. Real-world cases reveal how organizations today manage to get the most out of these architectures. ",
                    ImageFile = "https://m.media-amazon.com/images/I/51bejzpSQyL._SX379_BO1,204,203,200_.jpg",
                    Price = 47.99m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "The Pragmatic Programmer: 20th Anniversary Edition, 2nd Edition: Your Journey to Mastery",
                    Category = "IT",
                    Summary = "The Pragmatic Programmer is one of those rare tech audiobooks you’ll listen, re-listen, and listen to again over the years. Whether you’re new to the field or an experienced practitioner, you’ll come away with fresh insights each and every time. \r\n\r\nDave Thomas and Andy Hunt wrote the first edition of this influential book in 1999 to help their clients create better software and rediscover the joy of coding. These lessons have helped a generation of programmers examine the very essence of software development, independent of any particular language, framework, or methodology, and the Pragmatic philosophy has spawned hundreds of books, screencasts, and audio books, as well as thousands of careers and success stories.",
                    ImageFile = "https://m.media-amazon.com/images/P/B0C1J5P635.01._SCLZZZZZZZ_SX500_.jpg",
                    Price = 39.99m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "Clean Architecture: A Craftsman's Guide to Software Structure and Design",
                    Category = "IT",
                    Summary = "By applying universal rules of software architecture, you can dramatically improve developer productivity throughout the life of any software system. Now, building upon the success of his best-selling books Clean Code and The Clean Coder, legendary software craftsman Robert C. Martin (“Uncle Bob”) reveals those rules and helps you apply them.",
                    ImageFile = "https://m.media-amazon.com/images/I/41-sN-mzwKL._SX218_BO1,204,203,200_QL40_FMwebp_.jpg",
                    Price = 30.03m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Category = "IT",
                    Summary = "Even bad code can function. But if code isn’t clean, it can bring a development organization to its knees. Every year, countless hours and significant resources are lost because of poorly written code. But it doesn’t have to be that way.\r\n\r\nNoted software expert Robert C. Martin presents a revolutionary paradigm with Clean Code: A Handbook of Agile Software Craftsmanship. Martin has teamed up with his colleagues from Object Mentor to distill their best agile practice of cleaning code “on the fly” into a book that will instill within you the values of a software craftsman and make you a better programmer - but only if you work at it.",
                    ImageFile = "https://m.media-amazon.com/images/I/41xShlnTZTL._SX376_BO1,204,203,200_.jpg",
                    Price = 36.50m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "A Brief History of Time",
                    Category = "Nonfiction",
                    Summary = "A landmark volume in science writing by one of the great minds of our time, Stephen Hawking’s book explores such profound questions as: How did the universe begin—and what made its start possible? Does time always flow forward? Is the universe unending—or are there boundaries? Are there other dimensions in space? What will happen when it all ends?",
                    ImageFile = "https://m.media-amazon.com/images/P/0553380168.01._SCLZZZZZZZ_SX500_.jpg",
                    Price = 18.50m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "Guns, Germs, and Steel: The Fates of Human Societies",
                    Category = "Nonfiction",
                    Summary = "\"Diamond has written a book of remarkable scope ... one of the most important and readable works on the human past published in recent years.\"\r\n\r\nWinner of the Pulitzer Prize and a national bestseller: the global account of the rise of civilization that is also a stunning refutation of ideas of human development based on race.",
                    ImageFile = "https://m.media-amazon.com/images/I/51Psz1zS5ZL._SY291_BO1,204,203,200_QL40_FMwebp_.jpg",
                    Price = 28.00m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "We Have No Idea: A Guide to the Unknown Universe",
                    Category = "Nonfiction",
                    Summary = "PHD Comics creator Jorge Cham and particle physicist Daniel Whiteson have teamed up to spelunk through the enormous gaps in our cosmological knowledge, armed with their popular infographics, cartoons, and unusually entertaining and lucid explanations of science.",
                    ImageFile = "https://m.media-amazon.com/images/P/0735211523.01._SCLZZZZZZZ_SX500_.jpg",
                    Price = 28.00m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "Sapiens: A Brief History of Humankind",
                    Category = "Nonfiction",
                    Summary = "100,000 years ago, at least six human species inhabited the earth. Today there is just one. Us. Homo sapiens.\r\n\r\nHow did our species succeed in the battle for dominance? Why did our foraging ancestors come together to create cities and kingdoms? How did we come to believe in gods, nations and human rights; to trust money, books and laws; and to be enslaved by bureaucracy, timetables and consumerism? And what will our world be like in the millennia to come?",
                    ImageFile = "https://m.media-amazon.com/images/I/71N3-FFSDxL.jpg",
                    Price = 9.00m,
                },
                new Product()
                {
                    Id = new Guid(),
                    Name = "A Short History of Nearly Everything",
                    Category = "Nonfiction",
                    Summary = "Bill Bryson describes himself as a reluctant traveller, but even when he stays safely at home he can't contain his curiosity about the world around him. \"A Short History of Nearly Everything\" is his quest to understand everything that has happened from the Big Bang to the rise of civilisation - how we got from there, being nothing at all, to here, being us. The ultimate eye-opening journey through time and space, revealing the world in a way most of us have never seen it before.",
                    ImageFile = "https://m.media-amazon.com/images/P/B004CFAWES.01._SCLZZZZZZZ_SX500_.jpg",
                    Price = 23.33m,
                },
            };
        }
    }
}
