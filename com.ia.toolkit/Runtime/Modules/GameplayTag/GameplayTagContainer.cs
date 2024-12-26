using System;
using System.Collections.Generic;

namespace IAToolkit
{
    [Serializable]
    public class GameplayTagContainer
    {
        public List<string> tags = new List<string>();

        public GameplayTagContainer()
        {
            
        }

        public GameplayTagContainer(List<string> pTags)
        {
            tags = pTags;
        }
        
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < tags.Count; i++)
            {
                str += $" {tags[i]} ";
            }

            return str;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if ((obj is GameplayTagContainer) == false)
            {
                return false;
            }

            return HasAllExact(obj as GameplayTagContainer);
        }
        
        public void Add(GameplayTagContainer pContainer)
        {
            if (pContainer == null)
            {
                return;
            }

            for (int i = 0; i < pContainer.tags.Count; i++)
            {
                if (!tags.Contains(pContainer.tags[i]))
                {
                    tags.Add(pContainer.tags[i]);
                }
            }
        }
        
        public void Remove(GameplayTagContainer pContainer)
        {
            if (pContainer == null)
            {
                return;
            }

            for (int i = 0; i < tags.Count; i++)
            {
                if (pContainer.tags.Contains(tags[i]))
                {
                    tags.RemoveAt(i);   
                }
            }
        }
        
        #region Check

        /// <summary>
        /// 有没有该标签 包含检测
        /// </summary>
        /// <param name="pCheckTag"></param>
        /// <returns></returns>
        public bool Contain(string pCheckTag)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i].Contains(pCheckTag))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 有没有该标签 相等检测
        /// </summary>
        /// <param name="pCheckTag"></param>
        /// <returns></returns>
        public bool Equal(string pCheckTag)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                if (tags[i] == pCheckTag)
                {
                    return true;
                }
            }

            return false;
        }
        

        /// <summary>
        /// 有没有该标签 包含检查
        /// </summary>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public bool HasAny(GameplayTagContainer pContainer)
        {
            if (pContainer == null)
            {
                return false;
            }
            for (int i = 0; i < pContainer.tags.Count; i++)
            {
                if (Contain(pContainer.tags[i]))
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 有没有该标签 相等检查
        /// </summary>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public bool HasAnyExact(GameplayTagContainer pContainer)
        {
            for (int i = 0; i < pContainer.tags.Count; i++)
            {
                if (Equal(pContainer.tags[i]))
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// 所有是不是都是包含关系 包含检查
        /// </summary>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public bool HasAll(GameplayTagContainer pContainer)
        {
            for (int i = 0; i < pContainer.tags.Count; i++)
            {
                if (!Contain(pContainer.tags[i]))
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// 所有是不是都是包含关系 相等检查
        /// </summary>
        /// <param name="pContainer"></param>
        /// <returns></returns>
        public bool HasAllExact(GameplayTagContainer pContainer)
        {
            for (int i = 0; i < pContainer.tags.Count; i++)
            {
                if (!Equal(pContainer.tags[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}