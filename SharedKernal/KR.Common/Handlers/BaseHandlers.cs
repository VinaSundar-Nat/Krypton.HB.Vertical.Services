using MediatR;

namespace KR.Common.Handlers
{
    public interface ICreateRequest<out TResponse> : IRequest<TResponse>
    {
        public int? Id { get; set; }
        public bool IsDependent { get; set; }
    }

    public interface IDataStreamRequest<out TResponse> : IStreamRequest<TResponse>
    {

    }

    public interface IDataRequest<out TResponse> : IRequest<TResponse>
    {
    }
}

