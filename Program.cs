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
using VoiceToTextBot.Controllers;
using VoiceToTextBot.Services;
using VoiceToTextBot.Configurations;

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
            AppSettings appSettings = BuildAppSettings();
            services.AddSingleton(BuildAppSettings());

            services.AddSingleton<IStorage, MemoryStorage>();

            // Подключаем контроллеры сообщений и кнопок
            services.AddTransient<DefaultMessageController>();
            services.AddTransient<VoiceMessageController>();
            services.AddTransient<TextMessageController>();
            services.AddTransient<InlineKeyboardController>();


            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));
            // Доступ к хранилищу сессий
            services.AddSingleton<IStorage, MemoryStorage>();
            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
            services.AddSingleton<IFileHandler, AudioFileHandler>();
        }

        static AppSettings BuildAppSettings()
        {
            return new AppSettings()
            {
                DownloadsFolder = "C:\\C# projects\\DownloadAudioTBOT",
                BotToken = "6046274986:AAHPeGMmNbP-SOZB8EuciaXPJzP08wPaS_w",
                AudioFileName = "audio",
                InputAudioFormat = "ogg",
            };

        }
    }

    
}