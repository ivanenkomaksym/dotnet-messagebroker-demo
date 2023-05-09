using CatalogAPI.Entities;
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
                    Id = "602d2149e773f2a3990b47f5",
                    Name = "Designing Data-Intensive Applications: The Big Ideas Behind Reliable, Scalable, and Maintainable Systems",
                    Category = "Books",
                    Summary = "Data is at the center of many challenges in system design today. Difficult issues need to be figured out, such as scalability, consistency, reliability, efficiency, and maintainability. In addition, we have an overwhelming variety of tools, including relational databases, NoSQL datastores, stream or batch processors, and message brokers. What are the right choices for your application? How do you make sense of all these buzzwords?",
                    ImageFile = "product-1.png",
                    Price = 32.17,
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f6",
                    Name = "Building Microservices: Designing Fine-Grained Systems 2nd Edition",
                    Category = "Books",
                    Summary = "As organizations shift from monolithic applications to smaller, self-contained microservices, distributed systems have become more fine-grained. But developing these new systems brings its own host of problems. This expanded second edition takes a holistic view of topics that you need to consider when building, managing, and scaling microservices architectures.\r\n\r\nThrough clear examples and practical advice, author Sam Newman gives everyone from architects and developers to testers and IT operators a firm grounding in the concepts. You'll dive into the latest solutions for modeling, integrating, testing, deploying, and monitoring your own autonomous services. Real-world cases reveal how organizations today manage to get the most out of these architectures. ",
                    ImageFile = "product-2.png",
                    Price = 47.99,
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f7",
                    Name = "The Pragmatic Programmer: 20th Anniversary Edition, 2nd Edition: Your Journey to Mastery",
                    Category = "Books",
                    Summary = "The Pragmatic Programmer is one of those rare tech audiobooks you’ll listen, re-listen, and listen to again over the years. Whether you’re new to the field or an experienced practitioner, you’ll come away with fresh insights each and every time. \r\n\r\nDave Thomas and Andy Hunt wrote the first edition of this influential book in 1999 to help their clients create better software and rediscover the joy of coding. These lessons have helped a generation of programmers examine the very essence of software development, independent of any particular language, framework, or methodology, and the Pragmatic philosophy has spawned hundreds of books, screencasts, and audio books, as well as thousands of careers and success stories.",
                    ImageFile = "product-3.png",
                    Price = 39.99,
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f8",
                    Name = "Clean Architecture: A Craftsman's Guide to Software Structure and Design",
                    Category = "Books",
                    Summary = "By applying universal rules of software architecture, you can dramatically improve developer productivity throughout the life of any software system. Now, building upon the success of his best-selling books Clean Code and The Clean Coder, legendary software craftsman Robert C. Martin (“Uncle Bob”) reveals those rules and helps you apply them.",
                    ImageFile = "product-4.png",
                    Price = 30.03,
                },
                new Product()
                {
                    Id = "602d2149e773f2a3990b47f9",
                    Name = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Category = "Books",
                    Summary = "Even bad code can function. But if code isn’t clean, it can bring a development organization to its knees. Every year, countless hours and significant resources are lost because of poorly written code. But it doesn’t have to be that way.\r\n\r\nNoted software expert Robert C. Martin presents a revolutionary paradigm with Clean Code: A Handbook of Agile Software Craftsmanship. Martin has teamed up with his colleagues from Object Mentor to distill their best agile practice of cleaning code “on the fly” into a book that will instill within you the values of a software craftsman and make you a better programmer - but only if you work at it.",
                    ImageFile = "product-5.png",
                    Price = 36.50,
                }
            };
        }
    }
}
