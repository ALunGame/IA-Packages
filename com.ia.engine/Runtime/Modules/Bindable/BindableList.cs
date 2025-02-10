using System;
using System.Collections;
using System.Collections.Generic;

namespace IAEngine
{
    /// <summary>
    /// 绑定集合类型
    /// </summary>
    public class BindableList<T> : InternalBindableValue, IEnumerable<T>, IList<T>
    {
        private List<T> _value;
        public List<T> Value
        {
            get
            {
                return _value;
            }
            set
            {
                Clear();
                _value = value;
                if (_value.IsLegal())
                {
                    for (int i = 0; i < _value.Count; i++)
                    {
                        onAdded?.Invoke(_value[i]);
                    }
                }
            }
        }

        #region Fields

        private event Action<T> onAdded;
        private event Action<int, T> onInserted;
        private event Action<T> onRemoved;
        private event Action<T> onItemChanged;
        private event Action onClear;

        #endregion

        public BindableList(OwnerBindableValue pOwner, List<T> pList) : base(pOwner)
        {
            _value = pList;
        }

        public BindableList(OwnerBindableValue pOwner) : base(pOwner)
        {
            _value = new List<T>();
        }

        #region Override

        public T this[int index]
        {
            get { return Value[index]; }
            set { SetItem(index, value); }
        }

        public int Count
        {
            get { return Value.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return Value.IndexOf(item);
        }

        public void Add(T item)
        {
            Value.Add(item);
            onAdded?.Invoke(item);
        }

        public void Insert(int index, T item)
        {
            Value.Insert(index, item);
            onInserted?.Invoke(index,item);
        }

        public bool Remove(T item)
        {
            if (Value.Remove(item))
            {
                onRemoved?.Invoke(item);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T v = Value[index];
            Remove(Value[index]);
        }

        public bool Contains(T item)
        {
            return Value.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Value.CopyTo(array, arrayIndex);
        }

        public void Clear()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                if (Value[i] is InternalBindableValue)
                {
                    InternalBindableValue bindValue = Value[i] as InternalBindableValue;
                    bindValue.ClearEvent();
                }
                RemoveAt(i);
            }
            Value.Clear();
            onClear?.Invoke();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        #endregion

        public override void ClearEvent()
        {
            base.ClearEvent();
            onAdded = null;
            onInserted = null;
            onRemoved = null;
            onItemChanged = null;
            onClear = null;
        }

        public bool IsLegal()
        {
            return _value.IsLegal();
        }

        #region Private

        protected void SetItem(int index, T item)
        {
            Value[index] = item;
            onItemChanged?.Invoke(item);
        }

        #endregion

        #region Public

        /// <summary>
        /// 注册添加
        /// </summary>
        /// <param name="pOnItemAdd"></param>
        public void RegisterAdd(Action<T> pOnItemAdd)
        {
            this.onAdded -= pOnItemAdd;
            this.onAdded += pOnItemAdd;
        }

        /// <summary>
        /// 清除添加
        /// </summary>
        /// <param name="pOnItemAdd"></param>
        public void UnregisterAdd(Action<T> pOnItemAdd)
        {
            this.onAdded -= pOnItemAdd;
        }

        /// <summary>
        /// 注册插入
        /// </summary>
        /// <param name="pOnItemInserted"></param>
        public void RegisterInserted(Action<int,T> pOnItemInserted)
        {
            this.onInserted -= pOnItemInserted;
            this.onInserted += pOnItemInserted;
        }

        /// <summary>
        /// 清理插入
        /// </summary>
        /// <param name="pOnItemInserted"></param>
        public void UnregisterInserted(Action<int,T> pOnItemInserted)
        {
            this.onInserted -= pOnItemInserted;
        }

        /// <summary>
        /// 注册移除
        /// </summary>
        /// <param name="pOnItemRemove"></param>
        public void RegisterRemove(Action<T> pOnItemRemove)
        {
            this.onRemoved -= pOnItemRemove;
            this.onRemoved += pOnItemRemove;
        }

        /// <summary>
        /// 清理移除
        /// </summary>
        /// <param name="pOnItemRemove"></param>
        public void UnregisterRemove(Action<T> pOnItemRemove)
        {
            this.onRemoved -= pOnItemRemove;
        }

        /// <summary>
        /// 注册改变
        /// </summary>
        /// <param name="pOnItemChanged"></param>
        public void RegisterItemChange(Action<T> pOnItemChanged)
        {
            this.onItemChanged -= pOnItemChanged;
            this.onItemChanged += pOnItemChanged;
        }

        /// <summary>
        /// 清理改变
        /// </summary>
        /// <param name="pOnItemChanged"></param>
        public void UnregisterItemChange(Action<T> pOnItemChanged)
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
    }
}