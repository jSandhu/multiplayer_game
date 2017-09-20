using Common.ExtensionUtils;
using Newtonsoft.Json;

namespace InstanceServer.PlayFabNetExtensions
{
    public class PlayFabServerJsonParser : IJsonParser
    {
        public T DeserializeObject<T>(string objectString)
        {
            return JsonConvert.DeserializeObject<T>(objectString);
        }
    }
}
