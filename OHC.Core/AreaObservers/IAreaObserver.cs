using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OHC.Core.AreaObservers
{
    public interface IAreaObserver
    {
        Task StartAsync();
        Task StopAsync();
    }
}
