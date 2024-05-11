using System;
using KR.Common.Extensions;
using KR.Infrastructure.Datastore.Interface;
using MediatR;

namespace KR.Infrastructure.Datastore.Events
{
	public class ChangeNotificationHandler: INotificationHandler<ChangeNotification>
    {
        private readonly IChangeCapture _changeCapture;
        public ChangeNotificationHandler(IChangeCapture changeCapture)
		{
            _changeCapture = changeCapture;
        }

        public async Task Handle(ChangeNotification notification, CancellationToken cancellationToken)
        {
           await _changeCapture.Store(new {
                before =notification.Before,
                after = notification.After
            }.ToJson());
        }
    }
}

