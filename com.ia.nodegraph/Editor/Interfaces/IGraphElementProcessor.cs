using System;
using UnityEngine;

namespace IANodeGraph
{
    public interface IGraphElementProcessor
    {
        object Model { get; }

        Type ModelType { get; }
    }

    public interface IGraphElementProcessor<T> : IGraphElementProcessor
    {

    }

    public interface IGraphElementProcessor_Scope
    {
        public Vector2Int Position { get; set; }
    }
}
