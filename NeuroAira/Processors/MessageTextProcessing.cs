using NeuroAira.Common;
using NeuroAira.Dtos;
using NeuroAira.Mappings;
using NeuroAira.Models;
using NeuroAira.Repositories;
using NeuroAira.Repositories.Base;
using NeuroAira.Response;
using NeuroAira.Services;
using NeuroAira.Services.Base;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;

namespace NeuroAira.Processors
{
	public class MessageTextProcessing : IDisposable
	{
		private readonly IMetaConversationRepository metaConversationRepository;
		private readonly IUserConversationRepository userConversationRepository;
		private readonly IMessageSenderService messageSenderService;

		public MessageTextProcessing ()
		{
			metaConversationRepository = new MetaConversationRepository(new Database.NeuroAiraDbContext());
			userConversationRepository = new UserConversationRepository(new Database.NeuroAiraDbContext());
			messageSenderService = new MessageSenderService();
		}

		public async Task<MessageTextResponse<List<ConversationDetails>>> SendMessage (string message, long telegramId)
		{
			var userConversation = await userConversationRepository.GetUserConversationWithMetaById(telegramId);

			var metaConversation = metaConversationRepository.GetMetaConversationById(userConversation.Id);

			var metaContent = JsonConvert.DeserializeObject<MetaConversationContent>(metaConversation.Content);

			using var api = new OpenAIClient("key");

			var messages = new List<Message>
			{
				new Message(Role.System, string.Format(Prompts.InitialPrompts["neuro-aira"], DateTime.Now.ToShortDateString()))
			};

			foreach (var conversation in metaContent.Conversations)
			{
				messages.Add(new Message(conversation.ConversationRole == "user" ? Role.User : Role.Assistant, conversation.ConversationContent));
			}

			messages.Add(new Message(Role.User, message));

			var chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo);
			var response = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
			var choice = response.FirstChoice;
			Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message} | Finish Reason: {choice.FinishReason}");

			try
			{
				return new MessageTextResponse<List<ConversationDetails>>
				{
					Data = new List<ConversationDetails>() {
						new ConversationDetails() {
							ConversationContent = messages.Last().Content,
							ConversationRole = "user"
						},
						new ConversationDetails() {
							ConversationContent = choice.Message,
							ConversationRole = "assistant"
						}
					},
					Status = true,
					Message = $"Message was sended by {telegramId}"
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				return new MessageTextResponse<List<ConversationDetails>>
				{
					Data = null,
					Status = false,
					Message = ex.Message
				};
			}
		}

		public async Task SaveConversation (List<ConversationDetails> conversation, long telegramId)
		{
			var userConversation = await userConversationRepository.GetUserConversationWithMetaById(telegramId);

			var metaConversation = metaConversationRepository.GetMetaConversationById(userConversation.Id);

			var metaContent = JsonConvert.DeserializeObject<MetaConversationContent>(metaConversation.Content);

			metaContent.Conversations.AddRange(conversation);

			if (metaContent.Conversations.Count >= 6)
			{
				metaContent.Conversations.RemoveRange(0, 2);
			}

			metaConversation.Content = JsonConvert.SerializeObject(metaContent);

			metaConversationRepository.UpdateMetaConversation(metaConversation);

			await metaConversationRepository.Save();
		}

		private bool disposed = false;

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					metaConversationRepository.Dispose();
					userConversationRepository.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose ()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
