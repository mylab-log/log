namespace MyLab.Logging
{
    public class LogEntityCustomCondition
    {
        public string Name { get; }
        public object Value { get; }

        public LogEntityCustomCondition(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}