using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Memory
{
    public class AnimationEvent : MonoBehaviour
    {
        [SerializeField] private Sprite _replacementSprite;
        [SerializeField] private Sprite _originalSprite;
        private float scaleX;
        private float scaleY;

        private void Start()
        {
            // Cache original sprite
            _originalSprite = GetComponent<SpriteRenderer>().sprite;
        }

        public void ReplaceSprite()
        {
            // Then replace the with the new sprite
            GetComponent<SpriteRenderer>().sprite = _replacementSprite;
        }

        public void RestoreSprite()
        {
            GetComponent<SpriteRenderer>().sprite = _originalSprite;
        }
    }
}
