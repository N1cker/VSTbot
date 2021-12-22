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

namespace VSTbot.BotConf
{
    public static class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            } catch(Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            var chatId = message.Chat.Id;
            var messageText = message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            var action = message.Text!.Split(' ')[0] switch
            {
                "/start" => Start(botClient, message),
                "/end" => End(botClient, message),
                "/help" => CommandList(botClient, message),
                _ => null
            };

            if(action == null)
            {
                return;
            }

            Message sentMessage = await action;
        }

        private static async Task<Message> Start(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new List<KeyboardButton>
            {
                new KeyboardButton("Dou"),
                new KeyboardButton("LinkedIn"),
                new KeyboardButton("Djinni") })
            {
                OneTimeKeyboard = true,
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Starting the job search process\n" +
                    "Choose source to search",
                    replyMarkup: replyKeyboardMarkup
                );
        }

        private static async Task<Message> End(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "End of the job search process",
                    replyMarkup: new ReplyKeyboardRemove()
                );
        }

        private static async Task<Message> CommandList(ITelegramBotClient botClient, Message message)
        {

            const string helpList = "/help - get the list of commands\n" +
                "/start - start job searching process\n" +
                "/end - end job searching process";

            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: helpList
                );
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}
