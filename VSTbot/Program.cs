using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace VSTbot
{
    internal class Program
    {
        private static TelegramBotClient botClient;
        static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient(BotConf.Configuration.Token);
            using CancellationTokenSource cts = new CancellationTokenSource();
            ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

            botClient.StartReceiving(
                BotConf.Handlers.HandleUpdateAsync,
                BotConf.Handlers.HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            User bot = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{bot.Username}");
            Console.ReadLine();

            cts.Cancel();
        }
    }
}
