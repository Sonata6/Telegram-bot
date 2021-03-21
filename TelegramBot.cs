using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

static partial class TelegramBot
{
    static TelegramBotClient Bot;
    static async Task Main()
    {
        var cts = new CancellationTokenSource();
        try
        {
            Bot = new TelegramBotClient("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            var me = await Bot.GetMeAsync();
            Console.WriteLine($"Start working for @{me.Username}");
            await Bot.ReceiveAsync(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);
        }   
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            cts.Cancel();
        }
    }

    static async Task BotOnMessageReceived(Message message)
    {
        if (message.Type != MessageType.Text)
            return;
        try
        {
            string cmd = message.Text.Split(' ')[0];
            switch (cmd)
            {
                case ("/setgroup"):
                {
                    await Commands.SetGroup(message.Text);
                    await Bot.SendTextMessageAsync(message.Chat.Id, $"Group set successfully");                 //отправляет сообщение
                    break;
                }

                case ("/getschedule"):
                {
                        await Commands.GetSchedule(message.Text);
                        await ScheduleQuery.SQuery(Commands.parts[1], Commands.parts[2], Commands.parts[3]);          // получение расписания
                        await Bot.SendTextMessageAsync(message.Chat.Id, $"{ScheduleQuery.schedule}");                 //отправляет сообщение
                        Console.WriteLine($"Receive message type: {message.Type}");                                   // проверка на отправку            
                        break;
                }

                case ("/help"):
                {
                        await Commands.Help();
                        await Bot.SendTextMessageAsync(message.Chat.Id, Commands.parts[0]);
                        break;
                }

                case ("/start"):
                {
                        await Commands.Start();
                        await Bot.SendTextMessageAsync(message.Chat.Id, Commands.parts[0]);
                        break;
                }
                default: throw new Exception("Sorry, I don't understand you ;( Check the data you entered");
            }
        }
         catch(Exception ex)
        {
            if (ex.Message.CompareTo("Этот хост неизвестен.") == 0 || ex.Message.CompareTo("Response status code does not indicate success: 503 (Service Unavailable).")==0)
                await Bot.SendTextMessageAsync(message.Chat.Id, "Error 504. IIS is not available.");
            else if(ex.Message.CompareTo("Error reading JObject from JsonReader. Path '', line 0, position 0.") == 0)
                await Bot.SendTextMessageAsync(message.Chat.Id, "The group you set does not exist.\nPlease change the group number with the /setgroup command");
            else
            await Bot.SendTextMessageAsync(message.Chat.Id, ex.Message);                      //отправляет сообщение
            Console.WriteLine(ex.Message);
        }
    }

    static async Task HandleUpdateAsync(ITelegramBotClient arg1, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
            return;
        try
        {
            await BotOnMessageReceived(update.Message);
        }
        catch (Exception exception)
        {
            ITelegramBotClient telegramBotClient=null;
            await HandleErrorAsync(telegramBotClient, exception, cancellationToken);
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient arg1, Exception exception, CancellationToken cancellationToken)
    {
        string ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",  _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}