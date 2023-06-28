using Ecommerce.Contracts.Models.Tables;
using Ecommerce.Contracts.Models.Tables.Pti7;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Ecommerce.Bot.Helper
{
    public class GlobalButtons
    {
        public static InlineKeyboardMarkup ShowAdmin(BotDataDto bot)
        {
            InlineKeyboardButton[] Buttons1 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"{bot.shop_name} پنل مدیریت فروشگاه")
                {
                    WebApp = new WebAppInfo{ Url = bot.admin_panel}
                },
            };
            InlineKeyboardButton[] Buttons2 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"{bot.bot_username} تنظیمات ربات")
                {
                    WebApp = new WebAppInfo{ Url = bot.bot_settings}
                },
            };
            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] { Buttons1, Buttons2 });
        }
        public static InlineKeyboardMarkup ShowWelcome(BotDataDto bot)
        {
            InlineKeyboardButton[] Buttons1 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"پروفایل👤")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_profile}
                },
            };
            InlineKeyboardButton[] Buttons2 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"سبد خرید🛒")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_cart}
                },
            };
            InlineKeyboardButton[] Buttons3 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"سفارشات🛍")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_order}
                },
            };
            InlineKeyboardButton[] Buttons4 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"درباره ماℹ️")
                {
                    CallbackData = $"about_us"
                },
            };
            InlineKeyboardButton[] Buttons5 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"تماس با ما📞")
                {
                    CallbackData = $"contact_us"
                },
            };

            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] {
                Buttons1, Buttons2, Buttons3, Buttons4, Buttons5
            });
        }
    }
    #region Pti7
    public class Pti7
    {
        public static InlineKeyboardMarkup ShowWelcome(BotDataDto bot)
        {
            InlineKeyboardButton[] Buttons1 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"پروفایل👤")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_profile}
                },

            };

            InlineKeyboardButton[] Buttons2 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"سفارشات🛍")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_order}
                },
                new InlineKeyboardButton($"سبد خرید🛒")
                {
                    WebApp = new WebAppInfo{ Url = bot.user_cart}
                },
            };
            InlineKeyboardButton[] Buttons3 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"اساتید ما📚")
                {
                    CallbackData = $"pti7_masters"
                },
            };
            InlineKeyboardButton[] Buttons4 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"درباره ماℹ️")
                {
                    CallbackData = $"about_us"
                },
                new InlineKeyboardButton($"تماس با ما📞")
                {
                    CallbackData = $"contact_us"
                },
            };
            InlineKeyboardButton[] Buttons5 = new InlineKeyboardButton[]
            {
                new InlineKeyboardButton($"کانال موزیک🎶")
                {
                    Url = $"https://t.me/pti7music"
                },
            };

            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] {
                Buttons1, Buttons2, Buttons3, Buttons4, Buttons5
            });
        }

        public static InlineKeyboardMarkup ShowMastersList(IEnumerable<MasterDto> masters)
        {
            var keyboardInline = new InlineKeyboardButton[masters.Count()][];
            var keyboardButtons = new InlineKeyboardButton[masters.Count()];
            int counter = 0;
            foreach (var data in masters)
            {
                keyboardButtons[counter] = new InlineKeyboardButton(generateButtonText(data))
                {
                    CallbackData = "pti7_master" + data.Id,
                };
                counter++;
            }
            for (var i = 0; i < masters.Count(); i++)
            {
                keyboardInline[i] = keyboardButtons.Take(1).ToArray();
                keyboardButtons = keyboardButtons.Skip(1).ToArray();
            }
            return keyboardInline;
        }
        private static string generateButtonText(MasterDto data)
        {
            return $"استاد {data.Name} {data.Last_Name}";
        }
    }
    #endregion
}
