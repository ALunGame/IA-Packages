using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAToolkit.ViewModel
{
    /// <summary>
    /// 视图数据模板
    /// </summary>
    /// <typeparam name="TKey">键</typeparam>
    /// <typeparam name="TValue">值</typeparam>
    public interface IViewModel<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        T GetPropertyValue<T>(string propertyName);

        void SetPropertyValue<T>(string propertyName, T value);

        void BindingProperty<T>(string propertyName, Action<T> onValueChangedCallback);

        void ClearBindingEvent();
    }
}
