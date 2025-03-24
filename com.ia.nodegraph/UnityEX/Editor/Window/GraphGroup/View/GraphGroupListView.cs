using IAEngine;
using IAGAS.Editors;
using IANodeGraph.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace IANodeGraph
{
    internal class GraphGroupListView : VisualElement
    {
        public GraphGroupWindow Window { get; set; }

        private ListView GroupList;
        private Dictionary<string, Type> GroupDictMap = new Dictionary<string, Type>();

        public GraphGroupListView(GraphGroupWindow window)
        {
            InitGroupDict();

            Window = window;

            GroupList = new ListView();
            GroupList.fixedItemHeight = 55;
            GroupList.makeItem = MakeItem;
            GroupList.bindItem = BindItem;
            GroupList.itemsSource = GroupDictMap.Keys.ToList();
            GroupList.selectionChanged += (selectedItems) =>
            {
                foreach (var groupName in selectedItems)
                {
                    Window.SelGraphGroup((string)groupName, GroupDictMap[(string)groupName]);
                    break;
                }
            };


            this.Add(GroupList);

            this.style.flexGrow = 1;
        }

        private void InitGroupDict()
        {
            GroupDictMap.Clear();

            foreach (var type in TypeCache.GetTypesWithAttribute<GraphGroupDisplayAttribute>())
            {
                if (type.IsAbstract)
                    continue;

                foreach (var attribute in type.GetCustomAttributes(false))
                {
                    if (!(attribute is GraphGroupDisplayAttribute graphGroupDisplay))
                        continue;
                    GroupDictMap.Add(graphGroupDisplay.displayName, type);
                }
            }
        }

        private VisualElement MakeItem()
        {
            return GASEditorStyles.SettingTrees.MenuItemView.CloneTree();
        }

        private void BindItem(VisualElement pElement, int pIndex)
        {
            string groupName = (string)GroupList.viewController.GetItemForIndex(pIndex);

            var label = pElement.Q<Label>();
            label.text = groupName;
        }
    }
}
