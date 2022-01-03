using VSTbot.Parsing.Model;
using VSTbot.Parsing.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text;

namespace VSTbot.BotConf
{
    public static class Handlers
    {
        static QueryData queryData = new QueryData();
        static ParseSite parse;
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

            Task<Message> action = null;
            switch (messageText)
            {
                case "/start":
                    action = SelectSite(botClient, message);
                    break;
                case "/end":
                    action = End(botClient, message);
                    break;
                case "/help":
                    action = CommandList(botClient, message);
                    break;
                case string p when(p == "Dou" || p == "Djinni"):
                    queryData.SiteName = messageText;
                    action = WriteParameters(botClient, message, queryData);
                    break;
                default:
                    if (queryData.SiteName != null)
                    {
                        queryData.ParamString = messageText;
                        action = GetResult(botClient, message, queryData);
                    }
                    else
                        action = End(botClient, message);
                    break;
            }

            if (action == null)
            {
                return;
            }

            Message sentMessage = await action;
        }
        
        private static async Task<Message> SelectSite(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new List<KeyboardButton>
            {
                new KeyboardButton("Dou"),
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

        private static async Task<Message> WriteParameters(ITelegramBotClient botClient, Message message, QueryData queryData)
        {
            parse = new ParseSite(queryData.SiteName);
            string paramTemplateMessage = await parse.GetParamTemplateAsync();
            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: paramTemplateMessage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
        }

        private static async Task<Message> GetResult(ITelegramBotClient botClient, Message message, QueryData queryData)
        {
            string result = await parse.GetResultAsync(queryData.ParamString);
            if (result == null)
                result = "Didn't found anything suitable";
            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: result
                ); 
        }

        private static async Task<Message> End(ITelegramBotClient botClient, Message message)
        {
            queryData.SiteName = null;
            queryData.ParamString = null;
            return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "End of the job search process",
                    replyMarkup: new ReplyKeyboardRemove()
                );
        }

        private static async Task<Message> CommandList(ITelegramBotClient botClient, Message message)
        {
            queryData.SiteName = null;
            queryData.ParamString = null;

            const string helpList = "The current job search process has been stopped.\n" +
                "/help - get the list of commands\n" +
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
