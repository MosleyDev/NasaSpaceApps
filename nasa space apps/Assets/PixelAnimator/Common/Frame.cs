using System.Collections.Generic;
using UnityEngine;
using binc.PixelAnimator.PropertyDataProvider;
using Unity.VisualScripting;


namespace binc.PixelAnimator.Common{

    [System.Serializable]
    public class Frame{
        
        [SerializeField, ReadOnly]
        public string spriteId;

        public enum ColliderTypes{NoTrigger, Trigger}
        
        public ColliderTypes colliderType;
        public Rect hitBoxRect = new(16, 16, 16, 16);
        
        
        [SerializeField] private PropertyData hitBoxData;
        public PropertyData HitBoxData => hitBoxData;
        

        public Frame(string guid){
            spriteId = guid;
            hitBoxData = new PropertyData();

        }


    }


    [System.Serializable]
    public class PixelSprite{
        [field: ReadOnly] [field: SerializeField]
        public string spriteId;
        
        public Sprite sprite;
        

        [SerializeField] private PropertyData spriteData;
        public PropertyData SpriteData => spriteData;
        
        public PixelSprite(Sprite sprite, string gUid){
            this.sprite = sprite;
            spriteData = new PropertyData();
            spriteId = gUid;
        }
    }





}
