using IANodeGraph.Editors;
using IANodeGraph.Model;
using IAToolkit.UnityEditors;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    public abstract partial class BaseNodeView
    {
        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnBindingProperties()
        {
        }

        protected virtual void OnUnBindingProperties()
        {
        }

        protected virtual BasePortView NewPortView(BasePortProcessor port)
        {
            return Activator.CreateInstance(GraphProcessorEditorUtil.GetViewType(port.ModelType), port, new EdgeConnectorListener(Owner)) as BasePortView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var script in EditorUtilityExtension.FindAllScriptFromType(ViewModel.GetType()))
            {
                evt.menu.AppendAction($"Open Script/" + script.name, _ => { AssetDatabase.OpenAsset(script); });
            }

            foreach (var script in EditorUtilityExtension.FindAllScriptFromType(ViewModel.Model.GetType()))
            {
                evt.menu.AppendAction($"Open Script/" + script.name, _ => { AssetDatabase.OpenAsset(script); });
            }

            evt.menu.AppendSeparator();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            BringToFront();
        }

        public virtual bool DrawingControls()
        {
            return controls.childCount > 0;
        }

        public void HighlightOn()
        {
            nodeBorder.AddToClassList("highlight");
        }

        public void HighlightOff()
        {
            nodeBorder.RemoveFromClassList("highlight");
        }

        public void Flash()
        {
            HighlightOn();
            schedule.Execute(_ => { HighlightOff(); }).ExecuteLater(2000);
        }

        public void AddBadge(IconBadge badge)
        {
            Add(badge);
            badges.Add(badge);
            badge.AttachTo(topContainer, SpriteAlignment.TopRight);
        }

        public void RemoveBadge(Func<IconBadge, bool> callback)
        {
            badges.RemoveAll(b =>
            {
                if (callback(b))
                {
                    b.Detach();
                    b.RemoveFromHierarchy();
                    return true;
                }

                return false;
            });
        }
    }

    public class BaseNodeView<T> : BaseNodeView where T : BaseNodeProcessor
    {
        public T T_ViewModel
        {
            get { return base.ViewModel as T; }
        }
    }
}