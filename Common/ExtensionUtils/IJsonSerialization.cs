namespace Common.ExtensionUtils
{
    public interface IJsonParser
    {
        T DeserializeObject<T>(string objectString);
    }
}
