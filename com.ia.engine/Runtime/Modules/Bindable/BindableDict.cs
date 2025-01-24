using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    public class BindableDict<T1, T2> : InternalBindableValue, IEnumerable<KeyValuePair<T1, T2>>, IDictionary<T1, T2>
    {
        private Dictionary<T1, T2> _value;
        public Dictionary<T1, T2> Value
        {
            get
            {
                return _value;
            }
            set
            {
                Clear();
                _value = value;
                if (value != null)
                {
                    foreach (var item in _value)
                    {
                        onAdded?.Invoke(item.Key, item.Value);
                    }

                }
            }
        }

        #region Fields

        private event Action<T1, T2> onAdded;
        private event Action<T1, T2> onRemoved;
        private event Action<T1, T2> onItemChanged;
        private event Action onClear;

        #endregion

        public BindableDict(OwnerBindableValue pOwner, Dictionary<T1, T2> pDict) : base(pOwner)
        {
            _value = pDict;
        }

        public BindableDict(OwnerBindableValue pOwner) : base(pOwner)
        {
            _value = new Dictionary<T1, T2>();
        }

        #region Override

        public T2 this[T1 key]
        {
            get { return Value[key]; }
            set { SetItem(key, value); }
        }

        public ICollection<T1> Keys
        {
            get => _value.Keys;
        }

        public List<T1> NewKeys
        {
            get 
            { 
                return new List<T1>(_value.Keys); 
            }
        }

        public ICollection<T2> Values
        {
            get => _value.Values;
        }

        public int Count
        {
            get => _value.Count;
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T1 key, T2 value)
        {
            if (_value.ContainsKey(key))
            {
                Debug.LogError($"{GetType().Name}:Add{key} Error, 重复的键");
                return;
            }
            _value.Add(key, value);
            onAdded?.Invoke(key, value);
        }

        public void Clear()
        {
            foreach (var key in NewKeys)
            {
                T2 value = _value[key];
                if (value is InternalBindableValue)
                {
                    InternalBindableValue bindValue = value as InternalBindableValue;
                    bindValue.ClearEvent();
                }
                Remove(key);
            }

            _value.Clear();
            onClear?.Invoke();
        }

        public bool ContainsKey(T1 key)
        {
            return _value.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public bool Remove(T1 key)
        {
            if (!_value.ContainsKey(key))
            {
                Debug.LogError($"{GetType().Name}:Remove{key} Error, 不存在键");
                return false;
            }
            T2 value = _value[key];
            _value.Remove(key);
            onRemoved?.Invoke(key, value);
            return true;
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            if (!_value.ContainsKey(key))
            {
                value = default;
                Debug.LogError($"{GetType().Name}:TryGetValue{key} Error, 不存在键");
                return false;
            }
            value = _value[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        #endregion

        #region NotImplementedException

        public void Add(KeyValuePair<T1, T2> item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            throw new NotImplementedException();
        } 

        #endregion

        #region Private

        protected void SetItem(T1 pKey, T2 pValue)
        {
            Value[pKey] = pValue;
            onItemChanged?.Invoke(pKey, pValue);
        }

        #endregion

        #region Public

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="pOnItemAdd"></param>
        public void RegisterAdd(Action<T1, T2> pOnItemAdd)
        {
            this.onAdded -= pOnItemAdd;
            this.onAdded += pOnItemAdd;
        }

        /// <summary>
        /// 清除添加
        /// </summary>
        /// <param name="pOnItemAdd"></param>
        public void UnregisterAdd(Action<T1, T2> pOnItemAdd)
        {
            this.onAdded -= pOnItemAdd;
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="pOnItemRemove"></param>
        public void RegisterRemove(Action<T1, T2> pOnItemRemove)
        {
            this.onRemoved -= pOnItemRemove;
            this.onRemoved += pOnItemRemove;
        }

        /// <summary>
        /// 清理移除
        /// </summary>
        /// <param name="pOnItemRemove"></param>
        public void UnregisterRemove(Action<T1, T2> pOnItemRemove)
        {
            this.onRemoved -= pOnItemRemove;
        }

        /// <summary>
        /// 注册改变
        /// </summary>
        /// <param name="pOnItemChanged"></param>
        public void RegisterItemChange(Action<T1, T2> pOnItemChanged)
        {
            this.onItemChanged -= pOnItemChanged;
            this.onItemChanged += pOnItemChanged;
        }

        /// <summary>
        /// 清理改变
        /// </summary>
        /// <param name="pOnItemChanged"></param>
        public void UnregisterItemChange(Action<T1, T2> pOnItemChanged)
        {
            this.onItemChanged -= pOnItemChanged;
        }

        /// <summary>
        /// 注册清空
        /// </summary>
        /// <param name="pOnClear"></param>
        public void RegisterClear(Action pOnClear)
        {
            this.onClear -= pOnClear;
            this.onClear += pOnClear;
        }

        /// <summary>
        /// 清理清空
        /// </summary>
        /// <param name="pOnClear"></param>
        public void UnregisterClear(Action pOnClear)
        {
            this.onClear -= pOnClear;
        }

        #endregion

        public override void ClearEvent()
        {
            base.ClearEvent();
            onAdded = null;
            onRemoved = null;
            onItemChanged = null;
            onClear = null;
        }
    }
}
