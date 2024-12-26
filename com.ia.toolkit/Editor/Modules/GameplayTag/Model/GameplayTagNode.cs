using System.Collections.Generic;
using IAEngine;

namespace IAToolkit
{
    public class GameplayTagNode
    {
        public string tag = "";
        public GameplayTagNode parentNode;
        public List<GameplayTagNode> childNodes = new List<GameplayTagNode>();
        
        
        public string ShortName()
        {
            string[] tags = tag.Split(".");
            return tags[tags.Length - 1];
        }

        public void ChangeShortName(string pName)
        {
            string shortName = ShortName();
            tag = tag.Replace(shortName, pName);
        }

        public override string ToString()
        {
            string str = $"{tag}:";
            for (int i = 0; i < childNodes.Count; i++)
            {
                str += childNodes[i].tag + " ";
            }
            return str;
        }

        public void AddChildNode(GameplayTagNode pChildNode)
        {
            pChildNode.parentNode = this;

            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].tag == pChildNode.tag)
                {
                    return;
                }
            }
            
            
            childNodes.Add(pChildNode);
        }

        #region Static

        public static List<GameplayTagNode> GameplayTagsToNodes(GameplayTags pTags)
        {
            List<GameplayTagNode> nodes = new List<GameplayTagNode>();

            for (int i = 0; i < pTags.tags.Count; i++)
            {
                CollectNodes(pTags.tags[i], nodes);
            }
            
            return nodes;
        }
        
        public static List<GameplayTagNode> GameplayTagsToRootNodes(GameplayTags pTags)
        {
            List<GameplayTagNode> nodes = new List<GameplayTagNode>();

            for (int i = 0; i < pTags.tags.Count; i++)
            {
                CollectNodes(pTags.tags[i], nodes);
            }

            List<string> rootNames = new List<string>();
            for (int i = 0; i < pTags.tags.Count; i++)
            {
                string[] tags = pTags.tags[i].Split(".");
                if (!rootNames.Contains(tags[0]))
                {
                    rootNames.Add(tags[0]);
                }
            }
             
            List<GameplayTagNode> rootNodes = new List<GameplayTagNode>();
            for (int i = 0; i < rootNames.Count; i++)
            {
                rootNodes.Add(GetOrAddNode(rootNames[i],nodes));
            }
            return rootNodes;
        }

        private static void CollectNodes(string pTagName,List<GameplayTagNode> pNodes)
        {
            List<GameplayTagNode> tNodes = new List<GameplayTagNode>();
            string[] tags = pTagName.Split(".");
            for (int i = 0; i < tags.Length; i++)
            {
                string tTag = "";
                for (int j = 0; j < i; j++)
                {
                    tTag += tags[j] + ".";
                }
                tTag += tags[i];

                GameplayTagNode node = GetOrAddNode(tTag, pNodes);
                tNodes.Add(node);
            }

            for (int i = 0; i < tNodes.Count; i++)
            {
                int lastIndex = i - 1;
                if (lastIndex >= 0)
                {
                    tNodes[lastIndex].AddChildNode(tNodes[i]);
                }
            }
        }

        private static GameplayTagNode GetOrAddNode(string pTag,List<GameplayTagNode> pNodes)
        {
            for (int i = 0; i < pNodes.Count; i++)
            {
                GameplayTagNode tNode = pNodes[i];
                if (tNode.tag == pTag)
                {
                    return tNode;
                }
            }

            GameplayTagNode newTag = new GameplayTagNode();
            newTag.tag = pTag;
            pNodes.Add(newTag);
            return newTag;
        }

        public static GameplayTags GameplayNodesToTags(List<GameplayTagNode> rootNodes)
        {
            List<string> resTags = new List<string>();

            List<GameplayTagNode> noChildNodes = new List<GameplayTagNode>();
            foreach (GameplayTagNode rootNode in rootNodes)
            {
                CollectNoChildNode(rootNode, noChildNodes);
            }

            foreach (GameplayTagNode noChildNode in noChildNodes)
            {
                if (!resTags.Contains(noChildNode.tag))
                {
                    resTags.Add(noChildNode.tag);
                }
            }
            
            resTags.Sort();

            GameplayTags gameplayTags = new GameplayTags();
            gameplayTags.tags = resTags;
            return gameplayTags;
        }

        private static void CollectNoChildNode(GameplayTagNode pParentNode, List<GameplayTagNode> pResNodes)
        {
            if (!pParentNode.childNodes.IsLegal())
            {
                if (!pResNodes.Contains(pParentNode))
                {
                    pResNodes.Add(pParentNode);
                }
                return;
            }

            foreach (GameplayTagNode childNode in pParentNode.childNodes)
            {
                CollectNoChildNode(childNode, pResNodes);
            }
        }

        #endregion

    }
}