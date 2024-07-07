using NeuroAira.Response.Base;

namespace NeuroAira.Response
{
	public class UserResponse<T> : IBaseResponse<T> where T : class
	{
		public T Data { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }

		public static UserResponse<T> GenerateResponse (T data, bool status, string message)
		{
			return new UserResponse<T> { Data = data, Status = status, Message = message };
		}
	}
}
