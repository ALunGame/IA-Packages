using System;

namespace IAToolkit.ViewModel
{
    public class BindableProperty<T> : IBindableProperty
    {
        public event Func<T> Getter;
        public event Action<T> Setter;
        public event Action<object> onValueChanged;

        public T Value
        {
            get
            {
                if (Getter == null)
                    throw new NotImplementedException("haven't get method");
                return Getter();
            }
            set
            {
                if (Setter == null)
                    throw new NotImplementedException("haven't set method");
                if (Equals(Value, value))
                    return;
                Setter(value);
                ValueChanged();
            }
        }
        public object ValueBoxed
        {
            get { return Value; }
            set { Value = (T)value; }
        }
        public Type ValueType { get { return typeof(T); } }

        private string valueTooltip = "";
        public string ValueTooltip { get => valueTooltip;}

        public BindableProperty() { }
        public BindableProperty(Func<T> getter,string tooltip = "") { this.Getter = getter; this.valueTooltip = tooltip; }
        public BindableProperty(Func<T> getter, Action<T> setter, string tooltip = "") { this.Getter = getter; this.Setter = setter; this.valueTooltip = tooltip; }

        public void ValueChanged()
        {
            if (onValueChanged != null)
                onValueChanged.Invoke(Value);
        }

        public void RegisterValueChangedEvent(Action<object> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }

        public void UnregisterValueChangedEvent(Action<object> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }

        public void SetValueWithoutNotify(object value)
        {
            Setter((T)value);
        }

        public void ClearChangedEvent()
        {
            while (this.onValueChanged != null)
                this.onValueChanged -= this.onValueChanged;
        }
        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }
    }

    public class BindableProperty : IBindableProperty
    {
        public event Func<object> Getter;
        public event Action<object> Setter;
        public event Action<object> an;
        public event Action<object> onValueChanged;

        public object Value
        {
            get
            {
                if (Getter == null)
                    throw new NotImplementedException("haven't get method");
                return Getter();
            }
            set
            {
                if (Setter == null)
                    throw new NotImplementedException("haven't set method");
                if (Equals(Value, value))
                    return;
                Setter(value);
                ValueChanged();
            }
        }
        public object ValueBoxed
        {
            get { return Value; }
            set { Value = value; }
        }
        public Type ValueType { get { return Value.GetType(); } }

        private string valueTooltip = "";
        public string ValueTooltip { get => valueTooltip; }

        public BindableProperty() { }
        public BindableProperty(Func<object> getter, string tooltip = "") { this.Getter = getter; this.valueTooltip = tooltip; }
        public BindableProperty(Func<object> getter, Action<object> setter, string tooltip = "") { this.Getter = getter; this.Setter = setter; this.valueTooltip = tooltip; }

        public void ValueChanged()
        {
            if (onValueChanged != null)
                onValueChanged.Invoke(Value);
        }

        public void RegisterValueChangedEvent(Action<object> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }

        public void UnregisterValueChangedEvent(Action<object> onValueChanged)
        {
            this.onValueChanged -= onValueChanged;
        }

        public void SetValueWithoutNotify(object value)
        {
            SetValueWithoutNotify(value);
        }

        public void ClearChangedEvent()
        {
            while (this.onValueChanged != null)
                this.onValueChanged -= this.onValueChanged;
        }
        public override string ToString()
        {
            return (Value != null ? Value.ToString() : "null");
        }
    }
}
