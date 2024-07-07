using NeuroAira.Telegram;

namespace NeuroAira
{
	internal class Program
	{
		static async Task Main (string[] args)
		{
			TelegramBotManager telegramBotManager = new TelegramBotManager();
			await telegramBotManager.GetUpdatesAsync();
		}
	}
}