using Platform;
using System;

namespace Net.Services.Config
{
    public abstract class ConfigServiceBase : ServiceBase
    {
        public override int Priority { get { return (int)PriorityType.Default; } }
        public override bool AvailableForPlatform(PlatformType platformType) { return true; }

        public void GetPlatformConfig(Action<PlatformConfig> successHandler, Action failureHandler)
        {
            PlatformConfig platformConfig = PlatformConfig.Instance;
            getPlatformConfig(platformConfig, successHandler, failureHandler);
        }

        protected abstract void getPlatformConfig(PlatformConfig platformConfig, Action<PlatformConfig> successHandler, Action failureHandler);
    }
}
