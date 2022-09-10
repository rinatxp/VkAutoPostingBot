using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace VkAutoPostingBot
{
    public static class Tg
    {
        private static TelegramBotClient bot = new TelegramBotClient(Config.TOKEN);
        private static VkMem Mem;

        public static async Task Send()
        {
            Mem = Data.MemsForTg.Take();

            string text = $"{Mem.Message}\n(Группа {Mem.Group.Name})";
            var media = Mem.PhotoURLs.Where(w => w != null).Select(s =>
                                        new InputMediaPhoto(new InputMedia(s.ToString()))
                                        { Caption = text });

            await bot.SendMediaGroupAsync(Config.CHAT_ID, media);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Нет👎🏻","no"),
                InlineKeyboardButton.WithCallbackData("Да!❤","yes"),
            },
        });
            await bot.SendTextMessageAsync(Config.CHAT_ID, "Выберите:", replyMarkup: inlineKeyboard);
        }

        public static async void Start()
        {
            var receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>(),
                ThrowPendingUpdates = true,
            };

            bot.StartReceiving(updateHandler: UpdateHandlers.HandleUpdateAsync,
                               pollingErrorHandler: UpdateHandlers.PollingErrorHandler,
                               receiverOptions: receiverOptions);

            await Send();

            Console.ReadLine();
        }

        public static class UpdateHandlers
        {
            public static async Task HandleUpdateAsync(ITelegramBotClient botClient,
                                                        Update update, CancellationToken cancellationToken)
            {
                var handler = update.Type switch
                {
                    UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                    UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                };

                try
                {
                    await handler;
                }
                catch (Exception exception)
                {
                    PollingErrorHandler(botClient, exception, cancellationToken);
                }
            }

            private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
            {
                string answer = (message.From.Id == Config.CHAT_ID)
                                    ? "Воспользуйтесь кнопкой"
                                    : "Я тебя не знаю..";
                await botClient.SendTextMessageAsync(message.Chat!, answer);
            }

            private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery cb)
            {
                if (cb.From.Id != Config.CHAT_ID)
                {
                    await botClient.SendTextMessageAsync(cb.Message.Chat.Id, "Я тебя не знаю..");
                }
                else
                {
                    switch (cb.Data)
                    {
                        case "yes":
                            Data.MemsInQueue.Add(Mem);
                            break;
                        case "no":
                            VkMem.Write(Mem);
                            break;
                        default:
                            await botClient.SendTextMessageAsync(Config.CHAT_ID, "Неверная команда, повторите");
                            return;
                    }

                    Data.MemsPosted.Add(Mem);
                    await Send();
                }
            }

            public static Task PollingErrorHandler(ITelegramBotClient botClient,
                                Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException =>
                                            $"Telegram API Error:\n[{apiRequestException.ErrorCode}" +
                                            $"]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Logger.Log(ErrorMessage);
                return Task.CompletedTask;
            }
        }
    }
}


