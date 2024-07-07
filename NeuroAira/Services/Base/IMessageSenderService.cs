using NeuroAira.Dtos;
using NeuroAira.Models;

namespace NeuroAira.Services.Base
{
	public interface IMessageSenderService
	{
		public Task<List<ConversationDetails>> SendMessageToGpt (ConversationMapModel conversationMap);
	}
}
