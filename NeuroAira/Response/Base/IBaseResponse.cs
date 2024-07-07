namespace NeuroAira.Response.Base
{
	public interface IBaseResponse<T> where T : class 
	{
		public T Data { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }
	}
}
