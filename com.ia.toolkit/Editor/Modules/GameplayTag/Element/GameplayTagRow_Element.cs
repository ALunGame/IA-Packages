using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit
{
    public class GameplayTagRow_Element
    {
        public string Model { get; private set; }
        public VisualElement RootElement { get; protected set; }
        
        public GameplayTagsInspector Inspector { get; set; }
        
        public void SetUp(string pModel,GameplayTagsInspector pInspector)
        {
            Model = pModel;
            Inspector = pInspector;
            
            CreateElement();
    
            InitUIEvent();
        }
        
        private void CreateElement()
        {
            RootElement = new VisualElement();
            RootElement.name = "tagRow";
            RootElement.style.height = 30;
            RootElement.style.flexGrow = 1;

            TextField tagField = new TextField();
            tagField.style.fontSize = 25;
            tagField.style.unityFontStyleAndWeight = FontStyle.Bold;
            tagField.style.color = Color.white;
            tagField.style.flexGrow = 1;
            tagField.value = Model;
            tagField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                string newTag = evt.newValue;
                if (string.IsNullOrEmpty(newTag))
                {
                    Inspector.RemoveRowTag(Model);
                }
                else
                {
                    Char startChar = newTag[0];
                    if (startChar == '.')
                    {
                        //Debug.LogError($"标签名非法,起始字符为.:{newTag}");
                        tagField.value = Model;
                        return;
                    }
                    
                    Char endChar = newTag[newTag.Length-1];
                    if (endChar == '.')
                    {
                        //Debug.LogError($"标签名非法,结束字符为.:{newTag}");
                        tagField.value = Model;
                        return;
                    }

                    Inspector.UpdateRowTag(Model, newTag);
                    Model = newTag;
                }
            });
            
            RootElement.Add(tagField);
        }
        
        private void InitUIEvent()
        {
        }
    }
}