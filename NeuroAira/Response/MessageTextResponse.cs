using NeuroAira.Response.Base;

namespace NeuroAira.Response
{
	public class MessageTextResponse<T> : IBaseResponse<T> where T : class
	{
		public T Data { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }

		public static MessageTextResponse<T> GenerateResponse (T data, bool status, string message)
		{
			return new MessageTextResponse<T> { Data = data, Status = status, Message = message };
		}
	}
}
