namespace Common.Serialization
{
    public interface ISerializableModel
    {
        object[] ToObjectArray();
        void FromObjectArray(object[] properties);
    }
}
