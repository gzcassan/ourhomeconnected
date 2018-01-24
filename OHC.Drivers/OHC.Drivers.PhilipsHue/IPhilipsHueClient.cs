using System.Threading.Tasks;

namespace OHC.Drivers.PhilipsHue
{
    public interface IPhilipsHueClient
    {
        Task SwitchOnOffAsync(bool on, string area);
        Task SwitchToSceneAsync(string scene, string area);
    }
}