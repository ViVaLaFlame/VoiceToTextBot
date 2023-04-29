using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VoiceToTextBot.Configurations;
using VoiceToTextBot.Services;

namespace VoiceToTextBot.Controllers
{
    public class VoiceMessageController
    {
        private readonly AppSettings _appSettings;
        private readonly ITelegramBotClient _telegramClient;
        private readonly IFileHandler _audioFileHandler;

        public VoiceMessageController(AppSettings appSettings, ITelegramBotClient telegramBotClient,
            IFileHandler audioFileHandler)
        {
            _telegramClient = telegramBotClient;
            _appSettings = appSettings;
            _audioFileHandler = audioFileHandler;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            var fileId = message.Voice?.FileId;
            if (fileId == null)
                return;

            await _audioFileHandler.Download(fileId, ct);

            await _telegramClient.SendTextMessageAsync(message.Chat.Id, "Голосовое сообзщение загружено", cancellationToken: ct);
        }
    }
}
