using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IAEngine
{
    /// <summary>
    /// 拖拽
    /// </summary>
    public class BaseDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
    {
        //事件被使用，其他事件无法回调
        private bool eventUsed = false;

        private UnityAction<PointerEventData> onBeginDrag;
        private UnityAction<PointerEventData> onDrag;
        private UnityAction<PointerEventData> onDrop;
        private UnityAction<PointerEventData> onEndDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventUsed)
                return;
            onBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventUsed)
                return;

            onDrag?.Invoke(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventUsed)
                return;
            onDrop?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventUsed)
                return;
            onEndDrag?.Invoke(eventData);
        }

        #region Public

        /// <summary>
        /// 设置事件开启
        /// </summary>
        /// <param name="isOpen"></param>
        public void SetEventOpen(bool isOpen)
        {
            eventUsed = isOpen;
        }

        /// <summary>
        /// 设置开始拖拽事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetBeginDrag(UnityAction<PointerEventData> callback)
        {
            onBeginDrag = callback;
        }

        /// <summary>
        /// 设置拖拽事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetDrag(UnityAction<PointerEventData> callback)
        {
            onDrag = callback;
        }

        /// <summary>
        /// 设置开始Drop事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetDrop(UnityAction<PointerEventData> callback)
        {
            onDrop = callback;
        }

        /// <summary>
        /// 设置拖拽结束事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetEndDrag(UnityAction<PointerEventData> callback)
        {
            onEndDrag = callback;
        }

        #endregion
    }
}
