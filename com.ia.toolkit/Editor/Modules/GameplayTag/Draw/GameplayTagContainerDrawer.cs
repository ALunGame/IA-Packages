using System.Collections.Generic;
using IAEngine;
using IAToolkit.Core;
using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    [CustomObjectDrawer(typeof(GameplayTagContainer))]
    public class GameplayTagContainerDrawer : ObjectDrawer
    {
        private GUIStyle lableStyle;
        private List<GameplayTagNode> nodes;
        private List<GameplayTagNode> rootNodes;
        private float height = 20;

        public override void OnInit()
        {
            GameplayTags tags = GameplayTagsSetting.LoadTags();
            nodes = GameplayTagNode.GameplayTagsToNodes(tags);
            rootNodes = GameplayTagNode.GameplayTagsToRootNodes(tags);
            
            lableStyle = new GUIStyle(EditorStyles.label);
            lableStyle.alignment = TextAnchor.MiddleLeft;
            lableStyle.fontStyle = FontStyle.Bold;
            lableStyle.fontSize = 15;
        }
        
        public override object OnGUI(Rect _position, GUIContent _label)
        {
            GameplayTagContainer container = (GameplayTagContainer)Target;
            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayoutExtension.HorizontalGroup(() =>
                {
                    EditorGUILayout.LabelField(_label,lableStyle,GUILayout.Height(30));
                    MiscHelper.Btn("添加",100,30, () =>
                    {
                        List<string> menuList = CreateDropMenu(null);
                        MiscHelper.Menu(menuList, (int selIndex) =>
                        {
                            string menuStr = menuList[selIndex];
                            string tagName = menuStr.Replace("/", ".");
                            if (!container.tags.Contains(tagName))
                            {
                                container.tags.Add(tagName);
                            }
                        });
                    });
                });
                
                height += 30;

                for (int i = 0; i < container.tags.Count; i++)
                {
                    DrawTagBox(container.tags[i]);
                    height += 20;
                }
            });
            
            return container;
        }

        public override float GetHeight()
        {
            return 0;
        }

        private float btnWidth = 80;
        private float btnHeight = 20;
        private void DrawTagBox(string pTagName)
        {
            string[] tags = pTagName.Split(".");

            GUILayoutExtension.HorizontalGroup(() =>
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    int index = i;
                    string btnName = tags[index];
                    MiscHelper.Btn(tags[index],btnWidth,btnHeight,(() =>
                    {
                        ClickChangeTag(pTagName, btnName, index, tags);
                    }));
                }   
            });
        }


        private void ClickChangeTag(string pTagName, string pBtnName, int pIndex, string[] pTags)
        {
            string parentTagName = "";
            if (pIndex > 0)
            {
                for (int i = 0; i < pIndex; i++)
                {
                    if (i>=pIndex-1)
                    {
                        parentTagName += pTags[i];
                    }
                    else
                    {
                        parentTagName += pTags[i] + ".";
                    }
                }
            }
            
            GameplayTagContainer container = (GameplayTagContainer)Target;
            
            List<string> menuList = CreateDropMenu(parentTagName);
            menuList.Add("删除");
            MiscHelper.Menu(menuList, (int selIndex) =>
            {
                if (selIndex == menuList.Count - 1)
                {
                    container.tags.Remove(pTagName);
                }
                else
                {
                    string menuStr = menuList[selIndex];
                    string tagName = menuStr.Replace("/", ".");
                    
                    if (container.tags.Contains(tagName))
                    {
                        return;
                    }
                    for (int i = 0; i < container.tags.Count; i++)
                    {
                        if (container.tags[i] == pTagName)
                        {
                            container.tags[i] = tagName;
                        }
                    }
                }
            });
        }

        private List<string> CreateDropMenu(string pTagName)
        {
            string[] tags;
            if (!string.IsNullOrEmpty(pTagName))
                tags = pTagName.Split(".");

            List<GameplayTagNode> nodes = new List<GameplayTagNode>();
            if (!string.IsNullOrEmpty(pTagName))
            {
                foreach (GameplayTagNode node in nodes)
                {
                    if (node.tag == pTagName)
                    {
                        nodes.Add(node);
                        break;
                    }
                }
            }
            else
            {
                nodes = rootNodes;
            }

            List<string> menuList = new List<string>();
            foreach (GameplayTagNode node in nodes)
            {
                CollectNodeDropMenuStr(node, menuList);
            }

            return menuList;
        }

        private void CollectNodeDropMenuStr(GameplayTagNode pNode, List<string> pMenuList)
        {
            List<string> menuList = new List<string>();
            string rootMenuStr = pNode.tag.Replace(".", "/");
            if (!pMenuList.Contains(rootMenuStr))
            {
                pMenuList.Add(rootMenuStr);
            }

            void AddMenuStr(GameplayTagNode pCheckNode,List<string> pResMenuList)
            {
                string tTag = pCheckNode.tag;
                string menuStr = tTag.Replace(".", "/");
                if (!pResMenuList.Contains(menuStr))
                {
                    pResMenuList.Add(menuStr);
                }
                
                if (!pCheckNode.childNodes.IsLegal())
                {
                    return;
                }
                for (int i = 0; i < pCheckNode.childNodes.Count; i++)
                {
                    GameplayTagNode childNode = pCheckNode.childNodes[i];
                    AddMenuStr(childNode, pResMenuList);
                }
            }

            AddMenuStr(pNode, pMenuList);
        }
    }
}