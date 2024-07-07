using NeuroAira.Dtos;
using NeuroAira.Processors;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NeuroAira.Telegram
{
	public class TelegramBotManager
	{
		// for test 7089762296:AAHL-4NUo3asfLOFNVoZjmYBIId630MzWpc
		// actual 7086939896:AAHBhY49meuq2brduLHVHg4rfNmMnEVNbxQ
		public static TelegramBotClient Client = new TelegramBotClient(token: "7086939896:AAHBhY49meuq2brduLHVHg4rfNmMnEVNbxQ");

		public TelegramBotManager ()
		{
		}

		internal async Task GetUpdatesAsync ()
		{
			User me = await Client.GetMeAsync();

			if (me != null && !string.IsNullOrEmpty(me.Username))
			{
				int offset = 0;
				while (true)
				{
					try
					{
						Update[] updates = await Client.GetUpdatesAsync(offset);
						if (updates != null && updates.Count() > 0)
						{
							foreach (Update update in updates)
							{
								ProcessUpdate(update);
								offset = update.Id + 1;
							}
						}

					}
					catch (Exception ex) { Console.WriteLine($"{ex.Message}, StackTrace: {ex.StackTrace}"); }
				}
			}
		}

		private async Task ProcessUpdate (Update update)
		{
			switch (update.Type)
			{
				case UpdateType.Message:
					ProcessMessage(update.Message);
					break;
			}
		}

		private async Task ProcessMessage (Message message)
		{
			if (message.Chat.Type == ChatType.Private)
			{
				UserProcessing userProcessing = new UserProcessing();
				MessageTextProcessing messageTextProcessing = new MessageTextProcessing();
				AudioTextProcessing audioTextProcessing = new AudioTextProcessing();

				if (message.Text != null && message.Text.Contains("start"))
				{
					var response = await userProcessing.CreateNewUserConversation(message.From.Id);

					if (response.Status == false)
					{
						Console.WriteLine(response.Message);
						return;
					}
					else
					{
						Console.Write(response.Message);
					}

					ConversationMapModel conversationMap = (await userProcessing.GetConversationByUserId(message.From.Id)).Data;

					var metaResponse = await messageTextProcessing.SendMessage("Привет ты кто?", message.From.Id);

					if (metaResponse.Status == false) { Console.WriteLine(metaResponse.Message); return; }

					await Client.SendTextMessageAsync(message.Chat.Id, metaResponse.Data.Where(c => c.ConversationRole == "assistant").First().ConversationContent);
				}
				else
				{
					string voiceText = string.Empty;

					if (message.Voice != null)
					{
						var voiceId = message.Voice.FileId;
						var voiceInfo = await Client.GetFileAsync(voiceId);
						var voicePath = voiceInfo.FilePath;
	
						if (!Directory.Exists("content")) Directory.CreateDirectory("content");


						string voiceOggFilePath = $"content/{message.From.Id}_audio.oga";


						var voiceFileResponse = await audioTextProcessing.DownloadVoiceFile(voiceOggFilePath, voicePath);
						var convertToMp3Response = await audioTextProcessing.ConvertOggToMp3(voiceOggFilePath, message.From.Id);

						if (!convertToMp3Response.Status) { Console.WriteLine(convertToMp3Response.Message); return; }

						var voiceGptResponse = await audioTextProcessing.ConvertVoiceToText(convertToMp3Response.Data);

						if (!voiceGptResponse.Status) { Console.WriteLine(voiceGptResponse); return; }

						voiceText = voiceGptResponse.Data;
					}

					var metaResponse = await messageTextProcessing.SendMessage(!string.IsNullOrEmpty(voiceText) ? voiceText : message.Text, message.From.Id);

					if (metaResponse.Status == false)
					{
						Console.WriteLine(metaResponse);
						return;
					}

					await messageTextProcessing.SaveConversation(metaResponse.Data, message.From.Id);

					string assistantMessage = metaResponse.Data[1].ConversationContent;

					await Client.SendTextMessageAsync(message.Chat.Id, assistantMessage);
				}

				userProcessing.Dispose();
				messageTextProcessing.Dispose();
				audioTextProcessing.Dispose();
			}
		}


	}
}
