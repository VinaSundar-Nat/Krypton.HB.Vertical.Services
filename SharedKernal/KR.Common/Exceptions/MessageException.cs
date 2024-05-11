using System;
namespace KR.Common.Exceptions;

public class MessageException : Exception
{
    public MessageException(Exception ex, string message)
        : base(message, ex)
    {
	}
}

