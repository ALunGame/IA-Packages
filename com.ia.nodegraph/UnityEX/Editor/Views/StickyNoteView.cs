using IANodeGraph.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using IAToolkit;

namespace IANodeGraph.View
{
    public class StickyNoteView : UnityEditor.Experimental.GraphView.StickyNote, IGraphElementView<StickyNoteProcessor>
    {
        public StickyNoteProcessor ViewModel { get; private set; }
        public IGraphElementProcessor V => ViewModel;

        public BaseGraphView Owner { get; private set; }

        public StickyNoteView()
        {
        }

        public void SetUp(StickyNoteProcessor note, BaseGraphView graphView)
        {
            this.ViewModel = note;
            this.Owner = graphView;
            // 初始化
            base.SetPosition(new Rect(ViewModel.Position.ToVector2(), ViewModel.Size.Value.ToVector2()));
            this.title = note.Title.Value;
            this.contents = note.Content.Value;
        }

        public void OnCreate()
        {
            ViewModel.PositionBindable.RegisterChanged(OnPositionChanged);
            ViewModel.Size.RegisterChanged(OnSizeChanged);
            ViewModel.Title.RegisterChanged(OnTitleChanged);
            ViewModel.Content.RegisterChanged(OnContentsChanged);

            this.RegisterCallback<StickyNoteChangeEvent>(OnChanged);
        }

        public void OnDestroy()
        {
            ViewModel.PositionBindable.UnregisterChanged(OnPositionChanged);
            ViewModel.Size.UnregisterChanged(OnSizeChanged);
            ViewModel.Title.UnregisterChanged(OnTitleChanged);
            ViewModel.Content.UnregisterChanged(OnContentsChanged);

            this.UnregisterCallback<StickyNoteChangeEvent>(OnChanged);
        }

        private void OnChanged(StickyNoteChangeEvent evt)
        {
            switch (evt.change)
            {
                case StickyNoteChange.Title:
                    {
                        var oldTitle = ViewModel.Title.Value;
                        var newTitle = this.title;
                        Owner.CommandDispatcher.Do(() => { ViewModel.Title.Value = newTitle; }, () => { ViewModel.Title.Value = oldTitle; });
                        break;
                    }
                case StickyNoteChange.Contents:
                    {
                        var oldContent = ViewModel.Content.Value;
                        var newContent = this.contents;
                        Owner.CommandDispatcher.Do(() => { ViewModel.Content.Value = newContent; }, () => { ViewModel.Content.Value = oldContent; });
                        break;
                    }
                case StickyNoteChange.Theme:
                    break;
                case StickyNoteChange.FontSize:
                    break;
                case StickyNoteChange.Position:
                    {
                        var oldPosition = ViewModel.Position;
                        var oldSize = ViewModel.Size.Value;
                        this.schedule.Execute(() =>
                        {
                            var newPosition = GetPosition().position;
                            var newSize = GetPosition().size;
                            Owner.CommandDispatcher.Do(() =>
                            {
                                ViewModel.Position = newPosition.ToVector2Int();
                                ViewModel.Size.Value = newSize.ToVector2Int();
                            }, () =>
                            {
                                ViewModel.Position = oldPosition;
                                ViewModel.Size.Value = oldSize;
                            });
                        }).ExecuteLater(20);

                        break;
                    }
            }
        }

        void OnPositionChanged(Vector2Int pNewPos, Vector2Int pOldPos)
        {
            base.SetPosition(new Rect(pNewPos.ToVector2(), GetPosition().size));
            Owner.SetDirty();
        }

        void OnSizeChanged(Vector2Int pNewSize, Vector2Int pOldSize)
        {
            base.SetPosition(new Rect(GetPosition().position, pNewSize.ToVector2()));
            Owner.SetDirty();
        }

        private void OnContentsChanged(string pNewContents, string pOldContents)
        {
            this.contents = pNewContents;
            Owner.SetDirty();
        }

        private void OnTitleChanged(string pNewTitle, string pOldTitle)
        {
            this.title = pNewTitle;
            Owner.SetDirty();
        }
    }
}
