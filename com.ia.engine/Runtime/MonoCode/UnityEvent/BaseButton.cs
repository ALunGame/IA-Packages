using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace IAEngine
{
    /// <summary>
    /// 点击
    /// </summary>
    public class BaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        //事件被使用，其他事件无法回调
        private bool eventUsed = false;

        //点击事件
        private UnityAction<PointerEventData> onClick;
        //按下事件
        private UnityAction<PointerEventData> onDown;
        //抬起事件
        private UnityAction<PointerEventData> onUp;
        //按住事件
        private UnityAction<PointerEventData> onHold;

        //长按点击事件
        private UnityAction<PointerEventData> longClickCallback;
        private float longClickDelayTime;
        private bool longClickTriggered = false;


        private PointerEventData cacheEventData;

        //防止其他点击回调
        private static int lastDownBtnInstanceId = 0;

        private int pressedWaitFrame = 1;
        private int currPressedWaitFrame = 0;
        /// <summary>
        /// 点击
        /// </summary>
        public bool IsPressed { get; private set; }
        
        /// <summary>
        /// 按住
        /// </summary>
        public bool IsHold { get; private set; }

        private void Update()
        {
            if (IsPressed)
            {
                if (currPressedWaitFrame >= pressedWaitFrame)
                {
                    IsPressed = false;
                    currPressedWaitFrame = 0;
                }
                else
                {
                    currPressedWaitFrame++;
                }
            }
            
            if (eventUsed)
                return;

            if (cacheEventData == null)
                return;

            onHold?.Invoke(cacheEventData);

            //长按点击
            if (longClickCallback != null && !longClickTriggered)
            {
                if (Time.unscaledTime - cacheEventData.clickTime >= longClickDelayTime)
                {
                    longClickCallback?.Invoke(cacheEventData);
                    longClickTriggered = true;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsHold = true;
            IsPressed = true;
            currPressedWaitFrame = 0;

            if (eventUsed)
                return;

            cacheEventData = eventData;
            lastDownBtnInstanceId = GetInstanceID();

            onDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsHold = false;
            IsPressed = false;
            currPressedWaitFrame = 0;
            
            if (eventUsed)
                return;
            
            onUp?.Invoke(eventData);

            if (CanClick(eventData))
            {
                if (GetInstanceID() == lastDownBtnInstanceId)
                {
                    onClick?.Invoke(eventData);
                }
            }

            longClickTriggered = false;
            cacheEventData = null;
        }

        private void OnDisable()
        {
            eventUsed = false;
            longClickTriggered = false;
        }

        private bool CanClick(PointerEventData eventData)
        {
            if (longClickTriggered)
                return false;

            if (eventData.dragging && eventData.pointerPress != eventData.pointerDrag)
                return false;

            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerUpHandler>(eventData.pointerCurrentRaycast.gameObject);
            if (pointerUpHandler != eventData.pointerPress)
                return false;

            return true;
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
        /// 设置点击事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetClick(UnityAction<PointerEventData> callback)
        {
            SetEventOpen(false);
            onClick = callback;
        }

        /// <summary>
        /// 检测是否有点击监听
        /// </summary>
        /// <returns></returns>
        public bool HasClickEvent()
        {
            return onClick != null;
        }

        public void ExecuteClickFunc(PointerEventData pointerEvent)
        {
            onClick?.Invoke(pointerEvent);
        }

        /// <summary>
        /// 设置按下事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetDown(UnityAction<PointerEventData> callback)
        {
            SetEventOpen(false);
            onDown = callback;
        }

        /// <summary>
        /// 设置抬起事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetUp(UnityAction<PointerEventData> callback)
        {
            SetEventOpen(false);
            onUp = callback;
        }

        /// <summary>
        /// 设置按住事件
        /// </summary>
        /// <param name="callback"></param>
        public void SetHold(UnityAction<PointerEventData> callback)
        {
            SetEventOpen(false);
            onHold = callback;
        }

        /// <summary>
        /// 设置长按点击事件
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delayTime">长按时间</param>
        public void SetLongClick(UnityAction<PointerEventData> callback, float delayTime)
        {
            SetEventOpen(false);
            longClickCallback = callback;
            longClickDelayTime = delayTime;
        }

        #endregion
    }
}
