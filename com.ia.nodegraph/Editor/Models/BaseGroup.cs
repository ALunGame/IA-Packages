using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph
{
    public class BaseGroup
    {
        public int id;
        public string groupName;
        public Vector2Int position;
        public Vector2Int size;
        public Color backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        public List<int> nodes = new List<int>();
    }
}