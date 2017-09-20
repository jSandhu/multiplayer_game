using Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Net.Services
{
    public enum PriorityType { Disabled = -1, Default = 0 }

    public static class ServiceResolver
    {
        private static Dictionary<Type, ServiceBase> resolvedTypes = new Dictionary<Type, ServiceBase>();

        public static T GetForCurrentPlatform<T>() where T : ServiceBase
        {
            PlatformType platformType = PlatformConfig.Instance.PlatformType;

            ServiceBase highestPriorityServiceBase = null;
            Type typeT = typeof(T);
            if (!resolvedTypes.TryGetValue(typeof(T), out highestPriorityServiceBase))
            {
                var typelist = Assembly.GetAssembly(typeT).GetTypes().Where(t =>
                    t.IsClass &&
                    typeT.IsAssignableFrom(t) && 
                    t.IsAbstract == false);

                foreach (var type in typelist)
                {
                    ServiceBase instance = Activator.CreateInstance(type) as ServiceBase;
                    if (!instance.AvailableForPlatform(platformType))
                    {
                        continue;
                    }
                    if (highestPriorityServiceBase == null || instance.Priority > highestPriorityServiceBase.Priority)
                    {
                        highestPriorityServiceBase = instance;
                    }
                }

                if (highestPriorityServiceBase == null)
                {
                    throw new Exception("Couldn't resolve a service for Type: " + typeT.ToString() + ", PlatformType: " + platformType.ToString());
                }

                resolvedTypes.Add(typeT, highestPriorityServiceBase);
            }

            Debug.Log(string.Format("Resolved [{0}] for [{1}], Platform: {2}.", 
                highestPriorityServiceBase.GetType().ToString(), typeT.ToString(), platformType.ToString()));

            return (T)highestPriorityServiceBase;
        }
    }
}
