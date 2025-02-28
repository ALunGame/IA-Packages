namespace IANodeGraph
{
    public interface IGetPortValue
    {
        object GetValue(string portName);
    }

    public interface IGetPortValue<T>
    {
        T GetValue(string portName);
    }

    public interface ISetPortValue
    {
        void GetValue(string portName, object value);
    }

    public interface ISetPortValue<T>
    {
        void GetValue(string portName, T value);
    }
}
