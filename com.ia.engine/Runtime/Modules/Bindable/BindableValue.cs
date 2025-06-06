using System;
using System.Collections.Generic;

namespace IAEngine
{
    public class OwnerBindableValue
    {
        private List<InternalBindableValue> bindableValues = new List<InternalBindableValue>();

        public void AddBindableValue(InternalBindableValue pValue)
        {
            if (!bindableValues.Contains(pValue))
            {
                bindableValues.Add(pValue);
            }
        }

        public void ClearBindableEvent()
        {
            foreach (var field in bindableValues)
            {
                field.ClearEvent();
            }
        }

        public void ClearBindableValues()
        {
            foreach (var field in bindableValues)
            {
                field.ClearEvent();
            }
            bindableValues.Clear();
        }
    }

    public class InternalBindableValue
    {
        public InternalBindableValue(OwnerBindableValue pOwner)
        {
            pOwner.AddBindableValue(this);
        }

        public virtual void ClearEvent()
        {

        }
    }

    public class BindableValue<T> : InternalBindableValue
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
                T oldValue = _value;
                _value = value;
                OnValueChanged(oldValue);
            }
        }

        private Action<T, T> changeNotifys = null;

        public BindableValue(OwnerBindableValue pOwner, T pValue) : base(pOwner)
        {
            _value = pValue;
        }

        public BindableValue(OwnerBindableValue pOwner) : base(pOwner)
        {
            _value = default;
        }

        public override string ToString()
        {
            return $"{_value.ToString()}";
        }

        private void OnValueChanged(T pOldValue)
        {
            if (changeNotifys != null)
                changeNotifys.Invoke(Value, pOldValue);
        }

        public void SetValueWithoutNotify(T pValue)
        {
            if (Equals(Value, pValue))
                return;
            Value = pValue;
        }

        /// <summary>
        /// 注册改变
        /// </summary>
        /// <param name="pOnValueChanged"></param>
        /// <param name="pTriggerCallBack">直接触发一次回调，OldValue这时候非法</param>
        public void RegisterChanged(Action<T, T> pOnValueChanged, bool pTriggerCallBack = false)
        {
            changeNotifys -= pOnValueChanged;
            changeNotifys += pOnValueChanged;
            
            if (pTriggerCallBack)
            {
                pOnValueChanged?.Invoke(Value, default);
            }
        }

        /// <summary>
        /// 清理改变
        /// </summary>
        /// <param name="pOnValueChanged"></param>
        public void UnregisterChanged(Action<T, T> pOnValueChanged)
        {
            changeNotifys -= pOnValueChanged;
        }

        public override void ClearEvent()
        {
            changeNotifys = null;
        }
    }
}