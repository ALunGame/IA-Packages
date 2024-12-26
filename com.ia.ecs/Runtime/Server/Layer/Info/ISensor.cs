using IAECS.Core;
using System;

namespace IAECS
{
    /// <summary>
    /// 世界感知器 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorldSensorAttribute : Attribute
    {
        /// <summary>
        /// 世界信息键
        /// </summary>
        public SensorType InfoKey { get; set; } = 0;

        public WorldSensorAttribute(SensorType infoKey)
        {
            this.InfoKey = infoKey;
        }
    } 
}

namespace IAECS.Layer.Info
{
    /// <summary>
    /// 信息读取层
    /// 1，所有方法都必须是对数据的只读
    /// 2，所有方法对调用环境不敏感
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseSensor<T> where T :  new()
    {
        private static readonly T instance = new T();
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        private event Action onUserDataChange;
        
        protected BaseSensor() { }
        
    }
    
    /// <summary>
    /// 世界感知器（收集世界信息）
    /// </summary>
    public interface ISensor
    {
    }
}
