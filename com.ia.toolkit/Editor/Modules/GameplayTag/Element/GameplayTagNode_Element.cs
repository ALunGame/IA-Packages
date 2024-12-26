using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit
{
    public class GameplayTagNode_Element
    {
        public Foldout RootElement { get; protected set; }
        public GameplayTagNode Model { get; private set; }
        
        public GameplayTagNode_Element ParentNode;
        public List<GameplayTagNode_Element> ChildNodes = new List<GameplayTagNode_Element>();
        
        public GameplayTagsInspector Inspector { get; set; }
        
        public void SetUp(GameplayTagNode pModel,GameplayTagNode_Element pParentRoot)
        {
            Model = pModel;
            ParentNode = pParentRoot;
            
            CreateElement();
    
            InitUIEvent();
        }
    
        public void CreateElement()
        {
            RootElement = new Foldout();
            RootElement.text = Model.ShortName();
    
            Toggle toggle = RootElement.Q<Toggle>(null, Toggle.ussClassName);
            toggle.style.marginLeft = 0;
            toggle.style.backgroundColor = new StyleColor(new Color(0,0,0,0.25f));
            toggle.style.height = 30;
    
            VisualElement checkMark = toggle.Q<VisualElement>(null, Toggle.checkmarkUssClassName);
            //checkMark.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;

            checkMark.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
            checkMark.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
            checkMark.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
            checkMark.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);

            checkMark.style.unitySliceLeft = 0;
            checkMark.style.unitySliceTop = 0;
            checkMark.style.unitySliceBottom = 0;
            checkMark.style.unitySliceRight = 0;
            checkMark.style.height = 30;
            checkMark.style.width = 30;
    
            Label labelElement = toggle.Q<Label>(null, Label.ussClassName);
            labelElement.style.fontSize = 25;
            labelElement.style.unityFontStyleAndWeight = FontStyle.Bold;
            labelElement.style.color = Color.white;
            
            for (int i = 0; i < Model.childNodes.Count; i++)
            {
                GameplayTagNode childModel = Model.childNodes[i];
                GameplayTagNode_Element childNodeElement = new GameplayTagNode_Element();
                childNodeElement.SetUp(childModel,this);
                RootElement.Add(childNodeElement.RootElement);
                ChildNodes.Add(childNodeElement);
            }
        }
    
        public void InitUIEvent()
        {
            Toggle toggle = RootElement.Q<Toggle>(null, Toggle.ussClassName);
            toggle.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
        }

        public void SetExpandState(bool pExpandState)
        {
            RootElement.value = pExpandState;
            foreach (GameplayTagNode_Element childNode in ChildNodes)
            {
                childNode.SetExpandState(pExpandState);
            }
        }
        
        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction($"重命名",(action) =>
            {
                MiscHelper.Input("输入新的标签名", (newName) =>
                {
                    Model.ChangeShortName(newName);
                    RootElement.text = Model.ShortName();
                });
            });
            evt.menu.AppendAction($"添加",(action) =>
            {
                MiscHelper.Input("输入子标签名", (newName) =>
                {
                    foreach (GameplayTagNode_Element childNode in ChildNodes)
                    {
                        if (childNode.Model.ShortName() == newName)
                        {
                            Debug.LogError($"子标签重复:{newName}");
                            return;
                        }
                    }
                    GameplayTagNode addChildModel = new GameplayTagNode();
                    addChildModel.tag = Model.tag + "." + newName;
                    addChildModel.parentNode = Model;
                    Model.childNodes.Add(addChildModel);
                    
                    GameplayTagNode_Element addChildNode = new GameplayTagNode_Element();
                    addChildNode.SetUp(addChildModel,this);
                    
                    RootElement.Add(addChildNode.RootElement);
                    ChildNodes.Add(addChildNode);
                });
            });
            evt.menu.AppendAction($"删除",(action) =>{
                //删除子
                foreach (GameplayTagNode_Element childNode in ChildNodes)
                {
                    RootElement.Remove(childNode.RootElement);
                }
                //父删除
                if (ParentNode != null)
                {
                    ParentNode.RootElement.Remove(RootElement);
                }
                //Window根节点
                if (Inspector!=null)
                {
                    Inspector.RemoveRootTag(this);
                }
            });
        }
    }
}