using NeuroAira.Common;
using NeuroAira.Dtos;
using NeuroAira.Enums;
using NeuroAira.Helpers;
using NeuroAira.Models;
using NeuroAira.Repositories;
using NeuroAira.Repositories.Base;
using NeuroAira.Response;
using Newtonsoft.Json;

namespace NeuroAira.Processors
{
	public class UserProcessing : IDisposable
	{
		private readonly IUserConversationRepository userConversationRepository;
		private readonly IMetaConversationRepository metaConversationRepository;
		public UserProcessing ()
		{
			userConversationRepository = new UserConversationRepository(new Database.NeuroAiraDbContext());
			metaConversationRepository = new MetaConversationRepository(new Database.NeuroAiraDbContext());
		}

		public async Task<UserResponse<ConversationMapModel>> GetConversationByUserId (long telegramId)
		{
			var userCheck = await userConversationRepository.IsUserConversationExist(telegramId);
			if (!userCheck)
			{
				return UserResponse<ConversationMapModel>.GenerateResponse(null, false, "GetConversationByUserId. User did not exist");
			}

			var userConversation = await userConversationRepository.GetUserConversationWithMetaById(telegramId);
			var metaConversation = metaConversationRepository.GetMetaConversationById(userConversation.Id);

			var conversationMap = new ConversationMapModel()
			{
				action = userConversation.Action,
				conversation_id = userConversation.ConversationId.ToString(),
				model = ConversationHelper.ToGptModel((GptModelsEnum)userConversation.Model),
				provider = userConversation.Provider,
				jailbreak = userConversation.Jailbreak,
				meta = ConversationHelper.ToMetaModel(metaConversation)
			};

			return UserResponse<ConversationMapModel>.GenerateResponse(conversationMap, true, $"Return user conversation {telegramId}");
		}

		public async Task<UserResponse<string>> CreateNewUserConversation (long telegramId)
		{
			try
			{
				var userCheck = await userConversationRepository.IsUserConversationExist(telegramId);
				if (userCheck)
				{
					return UserResponse<string>.GenerateResponse(string.Empty, false, "User exist with such telegramId");
				}

				var metaContent = new MetaConversationContent()
				{
					Prompts = new List<ConversationDetails>() {
						new ConversationDetails()
						{
							ConversationRole = "user",
							ConversationContent = Prompts.InitialPrompts["neuro-aira"],
						},
						new ConversationDetails()
						{
							ConversationRole = "assistant",
							ConversationContent = "Здравствуйте! Я – Айра, твой ассистент. Я готова помочь тебе с тренировками, психологическими, тарологией, зодиаками и многим другим. Я гуру и всеведующая Айра.Смогу ли я – ассистент Айра, с чем-то вам помочь сегодня?🌌🔮"
						},
					},
					Conversations = new List<ConversationDetails>(),
					InternetAccess = true,
					ContentType = (int)ContentTypeEnum.Text
				};

				var userConversation = new UserConversation()
				{
					ConversationId = telegramId,
					Action = "_ask",
					Model = (int)GptModelsEnum.Gpt35Turbo,
					Provider = "g4f.Provider.Liaobots",
					Jailbreak = "default",
					MetaConversation = new MetaConversation()
					{
						Content = JsonConvert.SerializeObject(metaContent)
					}
				};

				userConversationRepository.CreateNewUserConversation(userConversation);

				await userConversationRepository.Save();

				return UserResponse<string>.GenerateResponse(string.Empty, true, $"User with telegramid {telegramId} is created");
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
				return UserResponse<string>.GenerateResponse(string.Empty, false, "Internal error");
			}
		}

		public async Task<bool> AnyUserConversation (long telegramId)
		{
			return await userConversationRepository.IsUserConversationExist(telegramId);
		}

		private bool disposed = false;

		protected virtual void Dispose (bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					userConversationRepository.Dispose();
					metaConversationRepository.Dispose();
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
