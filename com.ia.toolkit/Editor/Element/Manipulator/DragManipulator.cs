using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit.Element
{
    public class DragManipulator
    {
        private VisualElement target;
        public Action<IPointerEvent> OnDragStart;
        public Action<IPointerEvent> OnDraging;
        public Action<IPointerEvent> OnDragEnd;
        
        public DragManipulator(VisualElement pTarget)
        {
            this.target = pTarget;
            RegisterCallbacksOnTarget();
        }

        public void Clear()
        {
            UnregisterCallbacksFromTarget();
        }
        
        public void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        }
    
        public void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        }
    
        private Vector2 targetStartPosition { get; set; }
    
        private Vector3 pointerStartPosition { get; set; }
    
        private bool enabled { get; set; }

        private void PointerDownHandler(PointerDownEvent evt)
        {
            targetStartPosition = target.transform.position;
            pointerStartPosition = evt.position;
            enabled = true;
            target.CapturePointer(evt.pointerId);
            OnDragStart?.Invoke(evt);
        }
    
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                OnDraging?.Invoke(evt);
            }
        }
    
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
                enabled = false;
                OnDragEnd?.Invoke(evt);
            }
        }
    }
}