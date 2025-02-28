//using UnityEngine;
//using UnityEngine.UIElements;

//namespace IAToolkit.Element
//{
//    /// <summary>
//    /// 左键点击触发上下文菜单的操作器
//    /// 支持：点击延迟检测、动态菜单构建、坐标系自动转换
//    /// </summary>
//    public class LeftClickContextMenuManipulator : MouseManipulator
//    {
//        private readonly System.Action<DropdownMenu> m_MenuBuilder;
//        private Vector2 m_PointerStartPosition;
//        private bool m_IsPotentialContextClick;
//        private const float k_ClickMoveThreshold = 5f; // 像素移动阈值

//        public LeftClickContextMenuManipulator(System.Action<DropdownMenu> menuBuilder)
//        {
//            m_MenuBuilder = menuBuilder;
//            activators.Add(new ManipulatorActivationFilter
//            {
//                button = MouseButton.LeftMouse,
//                modifiers = EventModifiers.None
//            });
//        }

//        protected override void RegisterCallbacksOnTarget()
//        {
//            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
//            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
//            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
//            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureLost);
//        }

//        protected override void UnregisterCallbacksFromTarget()
//        {
//            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
//            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
//            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
//            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureLost);
//        }

//        private void OnPointerDown(PointerDownEvent evt)
//        {
//            if (!CanStartManipulation(evt)) return;

//            m_PointerStartPosition = evt.localPosition;
//            m_IsPotentialContextClick = true;

//            target.CaptureMouse();
//            evt.StopPropagation();
//        }

//        private void OnPointerMove(PointerMoveEvent evt)
//        {
//            if (!m_IsPotentialContextClick) return;

//            // 检测移动距离是否超过阈值
//            var delta = evt.localPosition - m_PointerStartPosition;
//            if (delta.sqrMagnitude > k_ClickMoveThreshold * k_ClickMoveThreshold)
//            {
//                CancelContextClick();
//            }
//        }

//        private void OnPointerUp(PointerUpEvent evt)
//        {
//            if (!CanStopManipulation(evt) || !m_IsPotentialContextClick) return;

//            ShowContextMenu(evt.position);
//            Cleanup();
//        }

//        private void OnPointerCaptureLost(PointerCaptureOutEvent evt)
//        {
//            CancelContextClick();
//        }

//        private void ShowContextMenu(Vector2 screenPosition)
//        {
//            var menu = new DropdownMenu();
//            m_MenuBuilder?.Invoke(menu);

//            // 将坐标转换为面板的全局坐标系
//            var panelPosition = target.LocalToWorld(screenPosition);
//            target.panel.contextualMenuManager?.DisplayMenu(menu, panelPosition);
//        }

//        private void CancelContextClick()
//        {
//            m_IsPotentialContextClick = false;
//            if (target.HasMouseCapture())
//            {
//                target.ReleaseMouse();
//            }
//        }

//        private void Cleanup()
//        {
//            m_IsPotentialContextClick = false;
//            if (target.HasMouseCapture())
//            {
//                target.ReleaseMouse();
//            }
//        }
//    }
//}