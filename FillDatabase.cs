using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.database.context;
using TelegramBot.database.model;
using TelegramBot.database.service;

namespace TelegramBot
{
    public class FillDatabase
    {
        private static IServiceProvider _serviceProvider;


        //public static async Task Main()
        //{
        //    InitDb();
        //    await SeedDatabase();
        //}

        private static void InitDb()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TelegramBotDbContext>(options =>
                  options.UseSqlite("Data Source=D:\\Курсач\\TelegramBot 2.9\\TelegramBot\\data\\database.db"));
            services.AddScoped<UserService>();
            _serviceProvider = services.BuildServiceProvider();

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
                dbContext.Database.Migrate();
            }
        }

        private static async Task SeedDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();


                if (!context.ProductSizes.Any())
                {
                    var sizes = new List<ProductSize>
                        {
                            new ProductSize { Size = "1"},
                            new ProductSize { Size = "2"},
                            new ProductSize { Size = "3"},
                            new ProductSize { Size = "4"},
                            new ProductSize { Size = "5"},

                        };

                    context.ProductSizes.AddRange(sizes);
                }


                if (!context.Product.Any())
                {
                    var products = new List<Product>
                        {
                            new Product { Name = "Renault Logan", Description = "🐎Мощность: 82 л.с.\r\n⛽️Расход топлива: 7 л/100 км\r\n\r\n🚗Renault Logan 2015 — это надежность и экономичность в одном автомобиле. Этот седан идеально подходит для городских поездок и загородных путешествий, обеспечивая комфорт и уверенность на дороге. Прекрасный выбор для тех, кто ценит практичность и качество. Довезет вас с комфортом и уверенностью!\r\nОценка:🌕🌕🌕🌑🌑 \r\n\r\n📍Растояние от вас: 250м", Price = 10, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\1.jpg")},
                            new Product { Name = "Tesla Cybertruck", Description = "🐎Мощность: 800+ л.с.\r\n⛽️Расход топлива: 0 л/100 км (электро)\r\n\r\n🛸Tesla Cybertruck — это не просто автомобиль, а настоящий прорыв в будущее. С невероятной мощностью и экологичностью, этот электрический пикап предлагает уникальный дизайн и передовые технологии. Идеален для тех, кто хочет выделяться и заботится о природе. Довезет вас с футуристическим размахом!\r\nОценка:🌕🌕🌕🌕🌑\r\n\r\n📍Растояние от вас: 170 м", Price = 17, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\2.jpg")},
                            new Product { Name = "Daewoo Matiz", Description = "🐎Мощность: 51 л.с.\r\n⛽️Расход топлива: 5.5 л/100 км\r\n\r\n🚗Daewoo Matiz 2005 — компактный и экономичный автомобиль, идеальный для городской среды. Легкий в управлении и парковке, он позволяет быстро и удобно передвигаться по городу. Отличный выбор для ежедневных поездок. Довезет вас с легкостью и удобством!\r\nОценка:🌕🌕🌕🌕🌕\r\n\r\n📍Растояние от вас: 550 м", Price = 15, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\3.jpg")},
                            new Product { Name = "Чайка", Description = "🐎Мощность: 195 л.с.\r\n⛽️Расход топлива: 15 л/100 км\r\n\r\n🚗Чайка — это легендарный автомобиль, олицетворяющий роскошь и стиль прошлого века. Его элегантный дизайн и просторный салон не оставят равнодушными ни водителя, ни пассажиров. Привлеките внимание и почувствуйте себя на высоте. Довезет вас с королевским шиком!\r\nОценка:🌕🌕🌕🌕🌑\r\n\r\n📍Растояние от вас: 250 м", Price = 220, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\4.jpg")},
                            new Product { Name = "BMW M5", Description = "🐎Мощность: 600 л.с.\r\n⛽️Расход топлива: 10.5 л/100 км\r\n\r\n🚗BMW M5 — это сочетание мощности и элегантности. Этот спортивный седан предлагает впечатляющую производительность и премиальный комфорт. Идеален для тех, кто ценит скорость и престиж. Довезет вас с адреналином и престижем!\r\nОценка:🌕🌕🌕🌕🌑\r\n\r\nРастояние от вас:", Price = 18, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\5.jpg")},
                            new Product { Name = "Mercedes-Benz", Description = "🐎Мощность: 115 л.с.\r\n⛽️Расход топлива: 14 л/100 км\r\n\r\n🚗Mercedes-Benz 1939 — классический автомобиль, который переносит вас в золотую эпоху автомобилестроения. Его винтажный дизайн и историческая значимость делают его уникальным выбором для особых случаев. Довезет вас с ретро-шармом!\r\nОценка:🌕🌕🌕🌕🌑\r\n\r\n📍Растояние от вас:320 м", Price = 20, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\6.jpg")},
                            new Product { Name = "DA-Ride", Description = "🐎Мощность: 1000 л.с.\r\n⛽️Расход топлива: 9 л/100 км\r\n\r\n🏇🏼DA-Ride — это воплощение силы и инноваций на четырех колесах. С мощностью в 1000 лошадиных сил, этот автомобиль предлагает непревзойденную производительность и динамику. Его стильный и аэродинамичный дизайн привлекает внимание на дороге, а передовые технологии обеспечивают комфорт и безопасность на высшем уровне. DA-Ride идеально подходит для тех, кто стремится к превосходству и не готов идти на компромиссы. Довезет вас с невероятной скоростью и абсолютным комфортом!\r\nОценка:🌕🌕🌕🌕🌕🌕\r\n\r\n📍Растояние от вас: 2 м", Price = 100, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\7.jpg")},
                            new Product { Name = "BelAZ", Description = "🐎Мощность: 4500 л.с.\r\n⛽️Расход топлива: 1300 л/100 км\r\n\r\n🚜BelAZ — это гигант среди автомобилей. Этот карьерный самосвал, предназначенный для перевозки огромных грузов, впечатляет своей мощностью и выносливостью. Идеален для тяжелых условий и больших задач. Довезет вас с мощью и внушительностью!\r\nОценка:🌕🌕🌑🌑🌑\r\n\r\n📍Растояние от вас: 1409 м", Price = 1210, ImageData = ConvertImageToByte("D:\\1\\TelegramBot 2.9\\TelegramBot\\data\\images\\8.jpg")},

                        };


                    context.Product.AddRange(products);
                }


                await context.SaveChangesAsync();
            }
        }

        private static byte[] ConvertImageToByte(string imagePath)
        {
            return System.IO.File.ReadAllBytes(imagePath);
        }
    }
}
