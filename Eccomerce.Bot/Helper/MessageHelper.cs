using Ecommerce.API.Models;
using Ecommerce.Bot.Helper;
using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Services;
using Ecommerce.Contracts.Utilities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Ecommerce.Bot.Global
{
    public class MessageHelper
    {
        static ITelegramBotClient? _bot;
        static BotDataDto? _thisBot;
        static Message? _message;

        public static async Task Read(ITelegramBotClient bot, Update update)
        {
            _bot = bot;
            _thisBot = Singleton.Instance.GetBotsData().Where(x => x.bot == bot).First();
            _message = update.Message;
            switch (_message.Type)
            {
                case MessageType.Text:
                    await MessageType_Text();
                    break;

                default:
                    break;
            }
        }
        private static async Task MessageType_Text()
        {
            if (_thisBot.special)
            {
                // Get the function name from the database
                string functionName = $"{_thisBot.shop_name}_Response"; // Replace with your logic to retrieve the function name

                // Create an instance of the class that contains the function
                var myClass = new MessageHelper();

                // Use reflection to get the MethodInfo of the function
                var methodInfo = myClass.GetType().GetMethod(functionName);

                // Check if the method exists
                if (methodInfo != null)
                {
                    // Invoke the method using the instance and any required arguments
                    methodInfo.Invoke(myClass, null);
                }
                else
                {
                    // The method does not exist
                    Console.WriteLine("Function not found");
                    await _bot.SendTextMessageAsync(_message.Chat.Id, $"{functionName}:Function not found");
                }

            }
            else
                GlobalResponse();
        }

        private static async Task StartResponse(InlineKeyboardMarkup welcome_buttons, InlineKeyboardMarkup admin_buttons)
        {
            try
            {
                if (_message.Text.ToLower() == "/start")
                {
                    var user = new UserRequest()
                    {
                        name = _message.Chat.FirstName,
                        last_name = _message.Chat.LastName,
                        username = _message.Chat.Username,
                    };

                    Console.WriteLine(@$"
===========================================
BotUsername = @{_thisBot.bot_username}
UserInfo = '{_message.Chat.Id}' @{_message.Chat.Username} *{_message.Chat.FirstName} {_message.Chat.LastName}*
Action = TextMessage : {_message.Text}
===========================================");
                    await UserService.UpsertUserAsync(user, _message.Chat.Id.ToString(), _thisBot.shop_name);

                    await _bot.SendTextMessageAsync(_message.Chat.Id, _thisBot.welcome_message, replyMarkup: welcome_buttons);

                    if (_message.Chat.Id.ToString() == _thisBot.owner_id)
                    {
                        //user is admin
                        await _bot.SendTextMessageAsync(_message.Chat.Id, "شما مدیر سیستم هستید!", replyMarkup: admin_buttons);
                    }
                }
                else
                {
                    await _bot.SendTextMessageAsync(_message.Chat.Id, "متوجه دستور شما نشدم🤖");
                }
            }
            catch
            {

            }

        }
        public static async void GlobalResponse()
        {
            await StartResponse(GlobalButtons.ShowWelcome(_thisBot), GlobalButtons.ShowAdmin(_thisBot));
        }

        public static async void Pti7_Response()
        {
            await StartResponse(Pti7.ShowWelcome(_thisBot), GlobalButtons.ShowAdmin(_thisBot));
        }
    }
}
