using System;
using System.Collections.Generic;
using System.Linq;
using binc.PixelAnimator.Common;
using binc.PixelAnimator.PropertyDataProvider;
using UnityEngine;


namespace binc.PixelAnimator.Preferences{


    [CreateAssetMenu(menuName = "Pixel Animator/ Preferences")]
    
    public class PixelAnimatorPreferences : ScriptableObject{
        [SerializeField] private List<PropertyDataWarehouse> spriteProperties;
        [SerializeField] private List<PropertyDataWarehouse> hitBoxProperties;
        [SerializeField] private List<BoxData> boxData;
        
        public List<PropertyDataWarehouse> SpriteProperties => spriteProperties;
        public List<PropertyDataWarehouse> HitBoxProperties => hitBoxProperties;
        public List<BoxData> BoxData => boxData;
        
        
        public BoxData GetGroup(string guid){
            return boxData.First(x => x.Guid == guid);
        }

        public PropertyDataWarehouse GetProperty(PropertyType propertyType, string guid){
            return propertyType switch{
                PropertyType.Sprite => spriteProperties.FirstOrDefault(x => x.Guid == guid),
                PropertyType.HitBox => hitBoxProperties.FirstOrDefault(x => x.Guid == guid),
                _ => throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, null)
            };
        }
        
    }
    
    public enum PropertyType{Sprite, HitBox}
}