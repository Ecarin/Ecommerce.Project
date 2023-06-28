using Ecommerce.Bot.Helper;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services.Pti7;
using Ecommerce.Contracts.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Ecommerce.Bot.Global
{
    public class CallbackQueryHelper
    {
        static ITelegramBotClient? _bot;
        static BotDataDto? _thisBot;
        static Message? _message;
        public static async Task Read(ITelegramBotClient bot, Update update)
        {
            _bot = bot;
            _thisBot = Singleton.Instance.GetBotsData().Where(x => x.bot == bot).First();
            _message = update.Message;

            var callbackQuery = update.CallbackQuery;
            var callbackQueryData = callbackQuery.Data;
            long chatId = callbackQuery.From.Id;

            Console.WriteLine(@$"
===========================================
BotUsername = @{_thisBot.bot_username}
UserInfo = '{callbackQuery.From.Id}' @{callbackQuery.From.Username} *{callbackQuery.From.FirstName} {callbackQuery.From.LastName}*
Action = CallbackQuery : {callbackQueryData}
===========================================");

            if (callbackQueryData == "about_us")
            {
                await bot.SendTextMessageAsync(chatId, _thisBot.about_us);
            }
            else if (callbackQueryData == "contact_us")
            {
                await bot.SendTextMessageAsync(chatId, _thisBot.contact_us);
            }

            #region Pti7
            else if (callbackQueryData.Contains("pti7"))
            {
                if (callbackQueryData == "pti7_masters")
                {
                    var availabte_masters = await MasterService.GetMasters();
                    if (availabte_masters == null || availabte_masters.Count() == 0)
                    {
                        await bot.SendTextMessageAsync(
                            chatId,
                            "استادی برای نمایش یافت نشد \n \n برای شروع مجدد /start");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(
                            chatId,
                            "برای دیدن اطلاعات کامل استاتید لطفا بر روی نام آنها کلیک کنید",
                            replyMarkup: Pti7.ShowMastersList(availabte_masters));
                    }
                }
                else if (callbackQueryData.StartsWith("pti7_master"))
                {
                    int masterId = Convert.ToInt32(callbackQueryData.Replace("pti7_master", ""));
                    var masterData = await MasterService.GetMaster(masterId);
                    if (masterData == null)
                    {
                        await bot.SendTextMessageAsync(
                            chatId,
                            "اطلاعات استاد یافت نشد");
                    }
                    else
                    {
                        string[] Photos_Path = masterData.Photo_Path.Split(',');
                        string currentDirectory = Directory.GetCurrentDirectory();
                        string targetDirectory = currentDirectory.Replace(@"Eccomerce.Bot\bin\Debug\net7.0", "Ecommerce.Api");
                        targetDirectory = targetDirectory.Replace("Eccomerce.Bot", "Eccomerce.Api");

                        foreach (string photo in Photos_Path)
                        {
                            using (var photoStream = System.IO.File.Open(Path.Combine(targetDirectory, photo), FileMode.Open))
                            {
                                var photoFile = new InputFileStream(photoStream);

                                string description = $@"{masterData.Name} {masterData.Last_Name}

{masterData.Description}";
                                await bot.SendPhotoAsync(chatId, photoFile, caption: description);
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}
