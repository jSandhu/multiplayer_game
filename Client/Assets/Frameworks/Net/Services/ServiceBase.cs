using Platform;

namespace Net.Services
{
    public abstract class ServiceBase
    {
        public abstract int Priority { get; }
        public abstract bool AvailableForPlatform(PlatformType platformType);
    }
}
