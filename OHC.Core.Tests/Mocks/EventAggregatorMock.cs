using Moq;
using OHC.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace OHC.Core.Tests.Mocks
{
    public static class EventAggregatorMock
    {
        public static Mock<IEventAggregator> CreateEventAggregatorMock()
        {
            var mock = new Mock<IEventAggregator>();
            //mock.Setup(x => x.)

            return mock;
        }
    }
}
