using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IAEngine
{
    // 线程安全的ID生成器
    public static class IdGenerator 
    { 
        private const long MaxSafeValue = long.MaxValue - 1; 
        private static long _counter = 1;
        
        public static long Next
        {
            get
            {
                long newValue = Interlocked.Increment(ref _counter);
        
                if (newValue > MaxSafeValue)
                {
                    throw new InvalidOperationException(
                        $"ID generator overflow detected. Current value: {newValue}");
                }

                return newValue;
            }
        }
    }
    
    /// <summary>
    /// 通用安全引用容器（自动处理目标失效场景）
    /// <para>当目标被销毁/重建时自动解除引用</para>
    /// </summary>
    /// <typeparam name="T">需实现IIdentifiable接口的引用类型</typeparam>
    public readonly struct ManagedRef<T> where T : class, IRefItem
    {
        private readonly long _targetId;
        private readonly WeakReference<T> _targetRef;

        public ManagedRef(T target)
        {
            if (target == null)
            {
                _targetId = 0;
                _targetRef = null;
                return;
            }

            if (target.InstanceId == 0)
                throw new ArgumentException("ManagedRef: Target must have a valid instance ID", nameof(target));

            _targetId = target.InstanceId;
            _targetRef = new WeakReference<T>(target);
        }

        /// <summary>
        /// 获取当前引用有效性状态
        /// </summary>
        public bool IsValid => _targetId != 0 && TryGetTarget(out _);

        /// <summary>
        /// 安全获取目标实例（自动失效检测）
        /// </summary>
        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => TryGetTarget(out var target) ? target : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetTarget(out T target)
        {
            target = null;
            return _targetRef != null && 
                   _targetRef.TryGetTarget(out target) && 
                   target.InstanceId == _targetId;
        }

        public static implicit operator ManagedRef<T>(T target) => new(target);
        public static implicit operator T(ManagedRef<T> reference) => reference.Value;

        public override string ToString() => 
            IsValid ? $"ManagedRef<{typeof(T).Name}>[{_targetId}]" : "Invalid Reference";
    }

    /// <summary>
    /// 托管引用
    /// </summary>
    public interface IRefItem
    {
        /// <summary>
        /// 实例唯一标识（0表示无效ID）
        /// </summary>
        long InstanceId { get; }
    }
}
