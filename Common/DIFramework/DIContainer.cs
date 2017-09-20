using System;
using System.Collections.Generic;

namespace DIFramework
{
    public class DIContainer
    {
        private static Dictionary<int, DIContainer> containersByID = new Dictionary<int, DIContainer>();

        public static DIContainer GetByID(int id)
        {
            return containersByID[id];
        }

        public static DIContainer RegisterContainer(int id)
        {
            DIContainer container = new DIContainer();
            containersByID.Add(id, container);
            return container;
        }

        public static T GetInstanceByContextID<T>(int contextID)
        {
            DIContainer container = null;
            if (!containersByID.TryGetValue(contextID, out container))
            {
                throw new Exception("DIContainer has not been registered for ID: " + contextID.ToString());
            }
            return container.GetRegisteredInstance<T>();
        }

        public static T CreateInstanceByContextID<T>(int contextID)
        {
            DIContainer container = null;
            if (!containersByID.TryGetValue(contextID, out container))
            {
                throw new Exception("DIContainer has not been registered for ID: " + contextID.ToString());
            }
            return container.CreateInstance<T>();
        }

        private Dictionary<Type, object> registeredTypeToInstance = new Dictionary<Type, object>();
        private Dictionary<Type, Type> typeMappings = new Dictionary<Type, Type>();

        public void RegisterInstance<T>(T instance)
        {
            Type t = typeof(T);
            if (registeredTypeToInstance.ContainsKey(t))
            {
                throw new Exception("An instance of type : " + t + " has already been registered. Unregister the existing instance first.");
            }
            registeredTypeToInstance.Add(t, instance);
        }

        public void RegisterInstance<T>(object catalogService)
        {
            throw new NotImplementedException();
        }

        public void UnregisterInstance<T>()
        {
            registeredTypeToInstance.Remove(typeof(T));
        }

        public bool HasRegisteredInstance<T>()
        {
            return registeredTypeToInstance.ContainsKey(typeof(T));
        }

        public T GetRegisteredInstance<T>()
        {
            Type t = typeof(T);
            object instance;
            registeredTypeToInstance.TryGetValue(t, out instance);
            if (instance == null)
            {
                throw new Exception("An instance has not been registered for type : " + t);
            }
            return (T)instance;
        }

        public void MapType<T, T2>()
        {
            Type tGen = typeof(T);
            if (typeMappings.ContainsKey(tGen))
            {
                throw new Exception("Type " + tGen + " has already been mapped to " + typeMappings[tGen]);
            }
            typeMappings.Add(tGen, typeof(T2));
        }

        public void UnmapType<T>()
        {
            typeMappings.Remove(typeof(T));
        }

        public T CreateInstance<T>()
        {
            Type tKey = typeof(T);
            Type tTarget;
            typeMappings.TryGetValue(tKey, out tTarget);
            if (tTarget == null)
            {
                throw new Exception("A target type has not been mapped for : " + tKey);
            }

            return (T)Activator.CreateInstance(tTarget);
        }
    }
}
