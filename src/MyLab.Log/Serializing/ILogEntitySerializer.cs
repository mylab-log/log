namespace MyLab.Log.Serializing
{
    /// <summary>
    /// Describes <see cref="LogEntity"/> serializer
    /// </summary>
    public interface ILogEntitySerializer
    {
        /// <summary>
        /// Serializes a <see cref="LogEntity"/> to <see cref="string"/>
        /// </summary>
        string Serialize(LogEntity logEntity);
    }
}