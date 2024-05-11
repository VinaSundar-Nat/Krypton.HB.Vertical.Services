using System;
using KR.Common.Handlers;
using MediatR;

namespace KR.Common.Adapter;

public abstract class BaseAdapter
{
    protected readonly IMediator _mediatr;

    protected BaseAdapter(IMediator mediatr)
    {
        this._mediatr = mediatr;
    }

    protected async Task<TResponse> Send<TRequest, TResponse>(TRequest model, CancellationToken token)
      where TRequest : IDataRequest<TResponse>
    {
        return await _mediatr.Send(model, token);
    }
}


