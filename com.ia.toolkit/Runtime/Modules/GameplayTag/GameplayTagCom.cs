using UnityEngine;

namespace IAToolkit
{
    public class GameplayTagCom : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public GameplayTagContainer tags = new GameplayTagContainer();
    }
}