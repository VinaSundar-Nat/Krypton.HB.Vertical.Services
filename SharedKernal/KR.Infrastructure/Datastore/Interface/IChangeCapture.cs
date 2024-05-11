using System;
namespace KR.Infrastructure.Datastore.Interface
{
	public interface IChangeCapture
	{
		public Task Store(string data);
	}
}

