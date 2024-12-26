using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model
{
    public partial class BaseGroup
    {
        [UnityEngine.HideInInspector] public string groupName;
        [UnityEngine.HideInInspector] public Vector2 position;
        [UnityEngine.HideInInspector] public Color backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        [UnityEngine.HideInInspector] public List<string> nodes = new List<string>();
    }
}