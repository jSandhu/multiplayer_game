using Common.ExtensionUtils;
using UnityEngine;

namespace PlayFabExtensions
{
    public class PlayFabClientJsonParser : IJsonParser
    {
        public T DeserializeObject<T>(string objectString)
        {
            return JsonUtility.FromJson<T>(objectString);
        }
    }
}
