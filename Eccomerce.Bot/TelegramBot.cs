using Ecommerce.API.Models;
using Ecommerce.Bot.Global;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Utilities;
using System.Diagnostics.Metrics;
using System.Runtime.Serialization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace Ecommerce.Bot
{
    class TelegramBot
    {
        static async Task Main(string[] args)
        {
            Singleton singleton = Singleton.Instance;

            Int16 succeeded = 0;
            foreach (var data in singleton.GetBotsData())
            {
                Console.WriteLine($"[Starting..] {data.bot_token}");

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync($"https://api.telegram.org/bot{data.bot_token}/deleteWebhook");
                    if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        try
                        {
                            var bot = new TelegramBotClient(data.bot_token);
                            var receiverOptions = new ReceiverOptions
                            {
                                AllowedUpdates = new UpdateType[]
                                {
                                    UpdateType.Message,
                                    UpdateType.CallbackQuery,
                                },
                            };

                            bot.StartReceiving(new DefaultUpdateHandler(UpdateHandler, ErrorHandler), receiverOptions);
                            data.bot = bot;
                            Console.WriteLine($"[Done!] {data.bot_token}");
                            succeeded++;
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine($"[Error!] {data.bot_token}\n{err}");
                        }
                    }
                }
            }
            //thread that update some data
            Timer timer = new Timer(UpdateBotsData, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

            Console.WriteLine($"{succeeded}/{singleton.GetBotsData().Count()} Bots Started successfully!");
            await Task.Delay(-1); // wait indefinitely
        }

        private static async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken arg3)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await MessageHelper.Read(bot, update);
                        break;

                    case UpdateType.CallbackQuery:
                        await CallbackQueryHelper.Read(bot, update);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                string fullMessage = err.Message + " : " + DateTime.Now.ToString();
                Console.WriteLine(fullMessage);
                await bot.SendTextMessageAsync(470533422, fullMessage);
                await bot.SendTextMessageAsync(740825237, fullMessage);
            }
        }

        private static Task ErrorHandler(ITelegramBotClient bot, Exception err, CancellationToken arg3)
        {
            string fullMessage = @$"
DateTime: {DateTime.Now}


InnerException: {err.InnerException}


Message: {err.Message}


stackTrace: {err.StackTrace}


Source: {err.Source}


HelpLink: {err.HelpLink}


HResult: {err.HResult}


TargetSite: {err.TargetSite}


BaseException: {err.GetBaseException()}


GetHashCode: {err.GetHashCode()}";

            //Console.WriteLine(fullMessage);

            bot.SendTextMessageAsync(470533422, fullMessage);
            bot.SendTextMessageAsync(740825237, fullMessage);

            return Task.CompletedTask;
        }

        private static void UpdateBotsData(object state)
        {
            // Perform the variable update
            Singleton.Instance.SetNewData(BotDataService.getBotsData());
        }
    }
}