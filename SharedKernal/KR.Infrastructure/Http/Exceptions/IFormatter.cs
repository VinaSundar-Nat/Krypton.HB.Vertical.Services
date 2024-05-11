using System;
namespace KR.Infrastructure.Http.Exceptions
{
    public interface IFormatter<ErrorHandler>
    {
        void Verify(HttpResponseMessage response);
    }
}

