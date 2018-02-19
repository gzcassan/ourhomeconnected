using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Infrastructure
{
    public interface IEventAggregator : IDisposable
    {
        IObservable<TEvent> GetEvent<TEvent>() where TEvent : IOHCEvent;
        void Publish<TEvent>(TEvent sampleEvent) where TEvent : IOHCEvent;
    }
}
