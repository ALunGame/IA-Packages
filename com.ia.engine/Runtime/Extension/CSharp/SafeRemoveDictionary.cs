using System.Collections.Generic;

namespace IAEngine
{
    /// <summary>
    /// 安全删除的字典
    /// </summary>
    public class SafeRemoveDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        // 用于暂时存储需要删除的键
        private List<TKey> removedKeys = new List<TKey>();

        // 重写父类的Remove方法
        public new void Remove(TKey key)
        {
            if (!removedKeys.Contains(key))
            {
                removedKeys.Add(key);
            }
        }

        // 定义一个遍历时删除的方法，遍历结束后再删除指定的键
        public void RemoveMarkedItems()
        {
            foreach (var key in removedKeys)
            {
                base.Remove(key);
            }
            removedKeys.Clear();
        }
    }
}
