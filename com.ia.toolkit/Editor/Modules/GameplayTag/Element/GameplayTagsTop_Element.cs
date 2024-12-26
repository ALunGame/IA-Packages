using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit
{
    public class GameplayTagsTop_Element
    {
        public GameplayTagsInspector Inspector { get; set; }
        
        public VisualElement RootElement { get; protected set; }

        private Dictionary<GameplayTagsDisplayType, string> typeNameDict =
            new Dictionary<GameplayTagsDisplayType, string>()
            {
                {GameplayTagsDisplayType.Tree,"树显示"},
                {GameplayTagsDisplayType.Row,"行显示"},
            };

        public void Init(GameplayTagsInspector pInspector)
        {
            Inspector = pInspector;
            RootElement = Inspector.rootVisualElement.Q<VisualElement>("Top");

            CreateUIElement();
            
            InitUIEvent();
        }

        private void CreateUIElement()
        {

        }
        
        private void InitUIEvent()
        {

            InitBtnEvent();
            InitMenuEvent();
        }

        private void InitBtnEvent()
        {
            Button expandBtn = RootElement.Q<Button>("ExpandAllBtn");
            expandBtn.text = Inspector.ExpandState ? "折叠所有" : "展开所有";
            expandBtn.clicked += () =>
            {
                Inspector.SetExpandState(!Inspector.ExpandState);
                expandBtn.text = Inspector.ExpandState ? "折叠所有" : "展开所有";
            };
            
            Button createBtn = RootElement.Q<Button>("CreateBtn");
            createBtn.clicked += () =>
            {
                MiscHelper.Input("输入标签名", (newName) =>
                {
                    Inspector.CreateRootTag(newName);
                });
            };
            
            Button saveBtn = RootElement.Q<Button>("SaveBtn");
            saveBtn.clicked += () =>
            {
                Inspector.Save();
            };
        }

        private void InitMenuEvent()
        {
            ToolbarMenu menu = RootElement.Q<ToolbarMenu>("DisplayChoose");
            menu.text = typeNameDict[Inspector.DisplayType];
            menu.menu.AppendAction("树显示", a =>
            {
                Inspector.ChangeDisplayType(GameplayTagsDisplayType.Tree);
                menu.text = typeNameDict[GameplayTagsDisplayType.Tree];
            }, a =>
            {
                if (Inspector.DisplayType == GameplayTagsDisplayType.Tree)
                {
                    return DropdownMenuAction.Status.Checked;
                }
                return DropdownMenuAction.Status.Normal;
            });
            menu.menu.AppendAction("行显示", a =>
            {
                Inspector.ChangeDisplayType(GameplayTagsDisplayType.Row);
                menu.text = typeNameDict[GameplayTagsDisplayType.Row];
            }, a =>
            {
                if (Inspector.DisplayType == GameplayTagsDisplayType.Row)
                {
                    return DropdownMenuAction.Status.Checked;
                }
                return DropdownMenuAction.Status.Normal;
            });
        }
    }
}