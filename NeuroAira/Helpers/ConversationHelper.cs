using NeuroAira.Common;
using NeuroAira.Dtos;
using NeuroAira.Enums;
using NeuroAira.Models;
using Newtonsoft.Json;
using System;

namespace NeuroAira.Helpers
{
	public static class ConversationHelper
	{
		private static Random random = new Random();

		public static string ToGptModel (GptModelsEnum gptModel)
		{
			switch (gptModel)
			{
				case GptModelsEnum.Gpt4:
					return "gpt-4";
				case GptModelsEnum.Gpt35Turbo:
					return "gpt-3.5-turbo";
				default:
					return string.Empty;
			}
		}

		public static string ToContentType (ContentTypeEnum contentType)
		{
			switch (contentType)
			{
				case ContentTypeEnum.Text:
					return "text";
				default:
					return string.Empty;
			}
		}

		//public static MetaConversation ToMetaModel (this MetaMapModel metaModel) {
		//	var metaContent = new MetaConversationContent() { 
		//		U
		//	};
		//}

		public static MetaMapModel ToMetaModel (this MetaConversation metaConversation)
		{
			var metaContent = JsonConvert.DeserializeObject<MetaConversationContent>(metaConversation.Content);

			var roleContentModels = new List<RoleContentMapModel>();

			foreach (var conversation in metaContent.Conversations)
			{
				roleContentModels.Add(new RoleContentMapModel
				{
					content = conversation.ConversationContent,
					role = conversation.ConversationRole
				});
			}

			return new MetaMapModel
			{
				id = GenerateMetaId(),
				content = new MetaConversationMapModel()
				{
					parts = new List<RoleContentMapModel>(),
					internet_access = metaContent.InternetAccess,
					content_type = ToContentType((ContentTypeEnum)metaContent.ContentType),
					conversation = roleContentModels
				}
			};
		}

		private static string GenerateMetaId ()
		{
			long randomBytes = random.NextInt64(2956589730, uint.MaxValue); // Adjusted to use long
			string randomBytesBinary = Convert.ToString(randomBytes, 2);

			long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			string unixTimestampBinary = Convert.ToString(unixTimestamp, 2);

			string combinedBinary = unixTimestampBinary + randomBytesBinary;

			return Convert.ToInt64(combinedBinary, 2).ToString();
		}
	}
}
