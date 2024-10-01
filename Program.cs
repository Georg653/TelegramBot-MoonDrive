using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.database.context;
using TelegramBot.database.service;
using TelegramBot.handler;
using TelegramBot.handler.admin;
using TelegramBot.handler.user;

// Определение основного пространства имён бота
namespace StarbucksBot
{
    // Основной класс бота
    public class Bot
    {
        // Объект клиента Telegram Bot API
        private static ITelegramBotClient BotClient;
        // Параметры для метода начала получения обновлений
        private static ReceiverOptions ReceiverOptions;
        // Провайдер для внедрения зависимостей
        private static IServiceProvider _serviceProvider;
        // Токен для доступа к Bot API
        public const string TOKEN = "";

        // Основной метод, точка входа в программу
        public static async Task Main()
        {
            await InitDb(); // Инициализация базы данных
            // Создание экземпляра клиента телеграм бота
            BotClient = new TelegramBotClient(TOKEN);
            // Настройка параметров приема сообщений
            ReceiverOptions = new ReceiverOptions
            {
                ThrowPendingUpdates = true,  // Будет прерывать обработку при получении неподтвержденных обновлений
            };

            // Установка команд бота
            BotClient.SetMyCommandsAsync(new[]{
                new BotCommand{ Command="start", Description="Вернуться к самому началу"},
                new BotCommand{ Command="cancel", Description="Нажми сюда, если все зависло (обнуление состояния)"}
            });

            // Создание токена отмены
            using var cts = new CancellationTokenSource();

            // Начало приема обновлений
            BotClient.StartReceiving(UpdateHandler, ErrorHandler, ReceiverOptions, cts.Token);

            // Получение информации о боте и вывод её в консоль
            var me = await BotClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");

            // Регистрация обработчиков команд
            RegisterHandlers();

            // Задержка для асинхронной работы бота
            await Task.Delay(-1);
        }

        // Метод для инициализации базы данных
        private static async Task InitDb()
        {
            var services = new ServiceCollection();
            services.AddDbContext<TelegramBotDbContext>(options =>
                options.UseSqlite("Data Source=D:\\Курсач\\TelegramBot 2.9\\TelegramBot\\data\\database.db"), ServiceLifetime.Scoped);
            // Регистрация сервисов
            services.AddScoped<UserService>();
            services.AddScoped<ProductService>();
            services.AddScoped<ProductSizeService>();
            services.AddScoped<CartService>();
            services.AddScoped<CartItemService>();
            services.AddScoped<OrderService>();
            // Создание провайдера сервисов
            _serviceProvider = services.BuildServiceProvider();

            // Применение миграций к базе данных
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TelegramBotDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }

        // Обработчик входящих сообщений
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обработка команды /cancel
            if (await CancelCommand(botClient, update, _serviceProvider)) return;

            // Получение состояния пользователя
            State state = await StateHandler.GetUserState(_serviceProvider, update?.Message, update?.CallbackQuery);
            if (state != State.None)
            {
                await StateHandler.GetInstance().HandleStateAsync(state, botClient, update, _serviceProvider);
                return;
            }

            // Обработка колбэк-кнопок
            if (update.Type == UpdateType.CallbackQuery)
            {
                await CallbacksHandler.GetInstance().Handle(update.CallbackQuery.Data, botClient, update.CallbackQuery, _serviceProvider);
            }

            // Дополнительные обработчики
            await AdminHandler.Handler(botClient, update, cancellationToken, _serviceProvider);
            await UserHandler.Handler(botClient, update, cancellationToken, _serviceProvider);
        }

        // Обработчик ошибок
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Формирование сообщения об ошибке и его вывод
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        // Регистрация обработчиков команд
        private static void RegisterHandlers()
        {
            UserHandler.RegisterHandlers(_serviceProvider);
            AdminHandler.RegisterHandlers(_serviceProvider);
        }

        // Обработчик команды отмены
        private static async Task<bool> CancelCommand(ITelegramBotClient botClient, Update update, IServiceProvider service)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message != null && update.Message.Text != null && update.Message.Text.ToLower().Equals("/cancel"))
                {
                    await StateHandler.SetUserState(State.None, update.Message, service);
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Состояние обнулено");
                    return true;
                }
            }
            return false;
        }
    }
}