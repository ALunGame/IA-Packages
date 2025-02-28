namespace IANodeGraph
{
    public interface IGraphElementView
    {
        IGraphElementProcessor V { get; }

        void OnCreate();

        void OnDestroy();
    }

    public interface IGraphElementView<T> : IGraphElementView where T : IGraphElementProcessor
    {
        T ViewModel { get; }
    }
}
