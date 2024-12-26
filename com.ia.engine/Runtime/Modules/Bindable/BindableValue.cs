using System;

namespace IAEngine
{
    public class BindableValue<T> : IBindable
    {
        private T _value;

        public T Value 
        { 
            get
            {
                return _value;
            }
            set
            {
                if (Equals(Value, value))
                    return;
                _value = value;
                ValueChanged();
            }
        }

        private event Action<T> onValueChanged;

        public void ValueChanged()
        {
            if (onValueChanged != null)
                onValueChanged.Invoke(Value);
        }

        public void SetValueWithoutNotify(T value)
        {
            if (Equals(Value, value))
                return;
            _value = value;
        }

        public void RegisterValueChangedEvent(Action<T> onValueChanged)
        {
            this.onValueChanged += onValueChanged;   
        }

        public void UnregisterValueChangedEvent(Action<T> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }

        public virtual void ClearChangedEvent()
        {
            onValueChanged = null;
        }

    }
}