using NeuroAira.Response.Base;

namespace NeuroAira.Response
{
	public class AudioTextResponse<T> : IBaseResponse<T> where T : class
	{
		public T Data { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }

		public static AudioTextResponse<T> GenerateResponse (T data, bool status, string message)
		{
			return new AudioTextResponse<T> { Data = data, Status = status, Message = message };
		}
	}
}
