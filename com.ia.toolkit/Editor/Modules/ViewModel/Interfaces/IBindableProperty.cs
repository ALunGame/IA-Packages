using System;

namespace IAToolkit.ViewModel
{
    /// <summary>
    /// 绑定数据值
    /// </summary>
    public interface IBindableProperty
    {
        event Action<object> onValueChanged;

        /// <summary>
        /// 值提示信息
        /// </summary>
        string ValueTooltip { get;}

        /// <summary>
        /// 装箱数据
        /// </summary>
        object ValueBoxed { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// 设置值不通知
        /// </summary>
        /// <param name="value"></param>
        void SetValueWithoutNotify(object value);

        /// <summary>
        /// 注册值变化事件
        /// </summary>
        /// <param name="onValueChanged"></param>
        void RegisterValueChangedEvent(Action<object> onValueChanged);

        /// <summary>
        /// 注销值变化事件
        /// </summary>
        /// <param name="onValueChanged"></param>
        void UnregisterValueChangedEvent(Action<object> onValueChanged);

        /// <summary>
        /// 清理事件
        /// </summary>
        void ClearChangedEvent();
    }
}
