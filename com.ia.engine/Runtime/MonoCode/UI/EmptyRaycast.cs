using UnityEngine.UI;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 空的事件接收器
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class EmptyRaycast : MaskableGraphic
    {
        protected EmptyRaycast()
        {
            useLegacyMeshGeneration = false;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    } 
}
