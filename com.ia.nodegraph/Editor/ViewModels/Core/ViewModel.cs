using IAEngine;
using System;

namespace IANodeGraph.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ViewModelAttribute : Attribute
    {
        public Type modelType;

        public ViewModelAttribute(Type modelType)
        {
            this.modelType = modelType;
        }
    }

    public class ViewModel : OwnerBindableValue
    {
    }
}
