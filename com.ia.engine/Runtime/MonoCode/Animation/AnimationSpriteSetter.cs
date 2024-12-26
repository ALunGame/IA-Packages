using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteAlways]
    public class AnimationSpriteSetter : MonoBehaviour
    {
        public int spriteIndex = 0;
        private int currIndex = 0;

        [Header("动画精灵列表")]
        [SerializeField]
        private List<Sprite> animSprites = new List<Sprite>();
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            UpdateSprite();
        }

        void Update()
        {
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (animSprites.Count == 0)
                return;
            if (currIndex == spriteIndex)
                return;
            if (spriteIndex < 0 || spriteIndex >= animSprites.Count)
                return;
            spriteRenderer.sprite = animSprites[spriteIndex];
            currIndex = spriteIndex;
        }
    }
}
