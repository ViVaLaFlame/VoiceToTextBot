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
using VoiceToTextBot.Models;

namespace VoiceToTextBot.Services
{
    public interface IStorage
    {
        /// <summary>
        /// Получение сессии пользователя по идентификатору
        /// </summary>
       
        Session GetSession(long chatId);
    }
}
