using NeuroAira.Dtos;
using NeuroAira.Models;
using NeuroAira.Services.Base;
using System.Text;
using Newtonsoft.Json;

namespace NeuroAira.Services
{
	public class MessageSenderService : IMessageSenderService
	{
		private readonly HttpClient Client;
		private readonly string Url = "http://127.0.0.1:1338";

		public MessageSenderService ()
		{
			Client = new HttpClient();
		}

		public async Task<List<ConversationDetails>> SendMessageToGpt (ConversationMapModel conversationMap)
		{
			var content = new StringContent(JsonConvert.SerializeObject(conversationMap), Encoding.UTF8, "application/json");
			var response = await Client.PostAsync(Url + "/backend-api/v2/conversation", content);

			var gptMessage = await response.Content.ReadAsStringAsync();
			

			return new List<ConversationDetails>
			{
				new ConversationDetails()
				{
					ConversationContent = conversationMap.meta.content.parts[0].content,
					ConversationRole = conversationMap.meta.content.parts[0].role
				},
				new ConversationDetails()
				{
					ConversationContent = gptMessage,
					ConversationRole = "assistant"
				}
			};
		}
	}
}
