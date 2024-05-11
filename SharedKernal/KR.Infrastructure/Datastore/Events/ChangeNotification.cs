using System;
using KR.Infrastructure.Datastore.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KR.Infrastructure.Datastore.Events
{
	public record ChangeNotification(PropertyValues Before, PropertyValues After) : INotification;

    public class ChangeCapture : IChangeCapture
    {
        public async Task Store(string data)
        {
            return;
        }
    }


}

