using Microsoft.Extensions.Logging;
using OHC.Core.Events;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace OHC.Core.Infrastructure
{
    public class EventAggregator : IEventAggregator
    {
        readonly Subject<object> subject = new Subject<object>();
        private ILogger<EventAggregator> logger;

        public EventAggregator(ILogger<EventAggregator> logger)
        {
            this.logger = logger;
        }

        public IObservable<TEvent> GetEvent<TEvent>() where TEvent : IOhcEvent
        {
            //TODO: consider this when we get error shutdowns: https://github.com/ReactiveX/RxJava/issues/3870
            return subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent ohcEvent) where TEvent : IOhcEvent
        {
            logger.LogInformation("Publishing event: {event}", ohcEvent.ToEventDescription());
            try
            {
                subject.OnNext(ohcEvent);
            }
            catch(Exception ex)
            {
                logger.LogError("Error publishing event: {message}", ex.Message, ex);
            }
        }

        bool disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            subject.Dispose();

            disposed = true;
        }
    }
}
