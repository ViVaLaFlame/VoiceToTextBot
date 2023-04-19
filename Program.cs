using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Threading.Tasks;
using System;
using Telegram.Bot.Polling;

namespace VoiceToTextBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");
        }

        static void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient("6046274986:AAHPeGMmNbP-SOZB8EuciaXPJzP08wPaS_w"));
            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }
    }

    class Bot : BackgroundService
    {
        private ITelegramBotClient _telegramClient;

        public Bot(ITelegramBotClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, cancellationToken: stoppingToken);

            Console.WriteLine("Бот запущен");
        } 

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Обрабатываем нажатие на кнопки из Telegram Bot API
            if (update.Type == UpdateType.CallbackQuery) 
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы нажали кнопку", cancellationToken: cancellationToken);
                return;
            }
            // Обрабатывем входящие сообщения из Telegram Bot API
            if (update.Type == UpdateType.Message)
            {
                Console.WriteLine($"Получено сообщение {update.Message.Text}");
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, $"Вы отправили сообщение {update.Message.Text}", cancellationToken: cancellationToken);
                return;
            }

        }

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Выводим в консоль информацию об ошибке
            Console.WriteLine(errorMessage);

            // Задержка перед повторным подключением
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
            Thread.Sleep(10000);

            return Task.CompletedTask;
        }
    }
}