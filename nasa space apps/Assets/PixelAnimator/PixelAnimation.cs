using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using binc.PixelAnimator.Common;


namespace binc.PixelAnimator{

    [CreateAssetMenu(menuName = "Pixel Animation/ New Animation")]
    public class PixelAnimation : ScriptableObject {

        public bool loop = true;
        public float frameRate;
        [SerializeField] private List<PixelSprite> pixelSprites;
        public List<PixelSprite> PixelSprites => pixelSprites;
        [SerializeField] private List<Layer> layers;
        public List<Layer> Layers => layers;
        

        public void AddLayer(string groupId){
            
            layers.Add(new Layer(groupId));
            var index = layers.Count -1;

            var frames = layers[index].frames ?? new List<Frame>();
            
            foreach (var pixelSprite in pixelSprites) {
                frames.Add(new Frame(pixelSprite.spriteId));
            }
        }

        public List<Sprite> GetSpriteList(){
            return pixelSprites.Select(pixelSprite => pixelSprite.sprite).ToList();
        }
        
        
    }
    
    
    



}



