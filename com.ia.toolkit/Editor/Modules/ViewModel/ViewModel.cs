using System;
using System.Collections;
using System.Collections.Generic;

namespace IAToolkit.ViewModel
{
    public class ViewModel : IViewModel<string, IBindableProperty>
    {
        [NonSerialized]
        Dictionary<string, IBindableProperty> internalBindableProperties;
        Dictionary<string, IBindableProperty> InternalBindableProperties
        {
            get
            {
                if (internalBindableProperties == null)
                    internalBindableProperties = new Dictionary<string, IBindableProperty>();
                return internalBindableProperties;
            }
        }

        public IEnumerable<string> Keys
        {
            get { return InternalBindableProperties.Keys; }
        }

        public IEnumerable<IBindableProperty> Values
        {
            get { return InternalBindableProperties.Values; }
        }

        public int Count
        {
            get { return InternalBindableProperties.Count; }
        }

        public IBindableProperty this[string propertyName]
        {
            get { return InternalBindableProperties[propertyName]; }
            set { InternalBindableProperties[propertyName] = value; }
        }

        public IEnumerator<KeyValuePair<string, IBindableProperty>> GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalBindableProperties.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return InternalBindableProperties.ContainsKey(key);
        }

        public bool TryGetValue(string key, out IBindableProperty value)
        {
            return InternalBindableProperties.TryGetValue(key, out value);
        }

        public T GetPropertyValue<T>(string propertyName)
        {
            return (T)this[propertyName].ValueBoxed;
        }

        public void SetPropertyValue<T>(string propertyName, T value)
        {
            this[propertyName].ValueBoxed = value;
        }

        public void BindingProperty<T>(string propertyName, Action<T> onValueChangedCallback)
        {
            this[propertyName].RegisterValueChangedEvent((object value) => {
                onValueChangedCallback?.Invoke((T)value);
            });
        }

        public void BindingProperty(string propertyName, Action<object> onValueChangedCallback)
        {
            this[propertyName].RegisterValueChangedEvent((object value) => {
                onValueChangedCallback?.Invoke(value);
            });
        }

        public void ClearBindingEvent()
        {
            foreach (var item in Values)
            {
                item.ClearChangedEvent();
            }
        }
    }
}
