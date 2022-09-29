using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using binc.PixelAnimator.Common;
using binc.PixelAnimator.Preferences;
using binc.PixelAnimator.PropertyDataProvider;
using binc.PixelAnimator.Utility;
using Frame = binc.PixelAnimator.Common.Frame;


namespace binc.PixelAnimator{

    public class AnimationManager : MonoBehaviour{
        [SerializeField] private PixelAnimation curr;
        public PixelAnimation CurrentAnimation{ get => curr; private set => curr = value; }
        private PixelAnimation previousAnimation;
        
        public int ActiveFrame => activeFrame;

        [SerializeField] private int activeFrame;

        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField]private float timer;

        private GameObject baseObject;
        [SerializeField] private List<GameObject> gameObjects;

        private PixelAnimatorPreferences preferences;

        private readonly Dictionary<string, PixelAnimatorListener> listeners = new();

        private readonly Dictionary<string, Action<object>> applyPropertyMethods = new();
        private List<BoxData> groups;
        private bool stop;
        
        private void Awake(){
            preferences = Resources.Load<PixelAnimatorPreferences>("PixelAnimatorPreferences");

            baseObject = new GameObject("Pixel Animator Colliders"){
                transform ={
                    parent = transform,
                    localPosition = Vector3.zero
                }
            };
            groups = preferences.BoxData;
            gameObjects ??= new List<GameObject>();
        }


        private void Update() {
            if(CurrentAnimation != null){
                Play();
            }
            

        }

        private void Play(){
            if (CurrentAnimation != previousAnimation) {
                previousAnimation = CurrentAnimation;
                return;
            }
            var sprites = CurrentAnimation.GetSpriteList();
            var frameRate = CurrentAnimation.frameRate;
            var loop = CurrentAnimation.loop;

            timer += Time.deltaTime;
            var secondsPerFrame = 1/ frameRate;
            
            if (timer >= secondsPerFrame) {
                foreach (var l in CurrentAnimation.Layers)
                    ApplySpriteProperty(l);
            }
            
            if (!(timer >= secondsPerFrame) && stop) return;
            timer -= secondsPerFrame;
            


            if (loop) {
                activeFrame = (activeFrame + 1) % sprites.Count;
                spriteRenderer.sprite = sprites[activeFrame];
                ApplyHitBox();
            }
            else{
                if (spriteRenderer.sprite != sprites[^1]) {
                    activeFrame = (activeFrame + 1) % sprites.Count;
                    spriteRenderer.sprite = sprites[activeFrame];
                    ApplyHitBox();
                }
            }          

            
            
        }

        public void ChangeAnimation(PixelAnimation nextAnimation){
            if (CurrentAnimation == nextAnimation) return;
            previousAnimation = CurrentAnimation;
            CurrentAnimation = nextAnimation;
            activeFrame = 0;
            timer = 0;
            stop = false;
            spriteRenderer.sprite = CurrentAnimation.GetSpriteList()[activeFrame];
            gameObjects = new List<GameObject>();

            foreach (Transform child in baseObject.transform) {
                Destroy(child.gameObject);

            }
            
            if(baseObject.transform.childCount <= 0){
                foreach (var layer in CurrentAnimation.Layers) {
                    AddGameObject(groups.First(x => x.Guid == layer.Guid));
                }
            }
            
            ApplyHitBox();

        }



        private void ApplySpriteProperty(Layer layer){
            if (spriteRenderer.sprite != CurrentAnimation.GetSpriteList()[activeFrame]) return;
            
            var pixelSprite = CurrentAnimation.PixelSprites[activeFrame];
            var spriteData = pixelSprite.SpriteData;
            var spriteMethodNames = spriteData.eventNames;
            var spriteDataValues = spriteData.propertyValues;
            
            foreach (var spriteMethodName in spriteMethodNames) {
                if (listeners.ContainsKey(spriteMethodName)) {
                    
                    foreach (var value in spriteDataValues) {
                        foreach (var pair in applyPropertyMethods.Where(pair =>
                                     preferences.GetProperty(PropertyType.Sprite,
                                         value.baseData.Guid).Name == pair.Key)) {
                            applyPropertyMethods[pair.Key].Invoke(value.baseData.GetData());
                        }
                    }
                    listeners[spriteMethodName].Invoke();
                }
                else {
                    foreach (var value in spriteDataValues) {
                        foreach (var pair in applyPropertyMethods.Where(pair =>
                                     preferences.GetProperty(PropertyType.Sprite,
                                         value.baseData.Guid).Name == pair.Key)) {
                            applyPropertyMethods[pair.Key].Invoke(value.baseData.GetData());
                        }
                    }

                    Debug.LogWarning($"Method name '{spriteMethodName}' does not exist");
                }
            }

            
            if (spriteMethodNames.Count <= 0) {
                foreach (var value in spriteDataValues) {
                    var propValue = value;
                    foreach (var pair in applyPropertyMethods.Where(pair =>
                                 preferences.GetProperty(PropertyType.Sprite,
                                     value.baseData.Guid).Name == pair.Key)){
                        applyPropertyMethods[pair.Key].Invoke(value.baseData.GetData());
                    }
                }
            }
            
        }

        private void ApplyHitBoxProperty(Layer layer){
            foreach (var frame in layer.frames) {
                foreach (var methodName in frame.HitBoxData.eventNames) {
                    if (listeners.ContainsKey(methodName)) {
                        
                        listeners[methodName].Invoke();
                    }
                }
            }
        }
        
        
        public void SetProperty(string name, Action<object> action){
            applyPropertyMethods.Add(name, action);
        }



        private void ApplyHitBox(){
            var layers = CurrentAnimation.Layers;
            var sprites = CurrentAnimation.GetSpriteList();

            if (layers.Count <= 0) return;
            
            for(var i = 0; i < layers.Count; i++){

                    //layers.FirstOrDefault(x => x.Guid.ToString() == preferences.GetGroup(x.Guid).Guid);
                    var layer = layers[i];
                    
                // Trigger
                if (layer == null || i >= gameObjects.Count) continue;
                    
                var boxCol = gameObjects[i].GetComponent<BoxCollider2D>();

                boxCol.isTrigger = layer.frames[activeFrame].colliderType switch{
                    Frame.ColliderTypes.NoTrigger => false,
                    _ => true
                };

                // Set hitBox size
                var spriteRect = sprites[activeFrame].rect;
                var size = layer.frames[activeFrame].hitBoxRect.size;
                var offset = layer.frames[activeFrame].hitBoxRect.position;
                var adjustedHitBoxPivot = new Vector2(offset.x + size.x / 2, size.y / 2 + offset.y);


                var adjustedXSize = size.x * sprites[activeFrame].bounds.size.x/spriteRect.width;
                var adjustedXOffset = (adjustedHitBoxPivot.x - sprites[activeFrame].pivot.x) *
                    sprites[activeFrame].bounds.size.x / spriteRect.width;
                
                var adjustedYOffset = (adjustedHitBoxPivot.y -  (spriteRect.height - sprites[activeFrame].pivot.y)) * -1 *
                    sprites[activeFrame].bounds.size.y / spriteRect.height;


                if (!boxCol.enabled) boxCol.enabled = true;
                boxCol.size = new Vector2(adjustedXSize, (size.y * sprites[activeFrame].bounds.size.y)/spriteRect.height );
                boxCol.offset = new Vector2( adjustedXOffset, adjustedYOffset);
            }
        }
        

        private void AddGameObject(BoxData activeBoxData){
            var alreadyExist = gameObjects.Any(x => x.name == activeBoxData.boxType);
            if (alreadyExist) return;
            gameObjects.Add(new GameObject(activeBoxData.boxType));
            var obj = gameObjects[^1];
            obj.AddComponent<BoxCollider2D>();
            obj.GetComponent<BoxCollider2D>().enabled = false;
            obj.transform.parent = baseObject.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.layer = activeBoxData.activeLayer;
            ApplyHitBox();
        }


        #region Set Collider Detect
        
        private void OnTriggerEnter2D(Collider2D other){
            OnTouch(other, Frame.ColliderTypes.Trigger, BoxData.ColliderDetection.Enter);
        }
        //
        //
        // private void OnTriggerStay2D(Collider2D other) {
        //     OnTouch(other, Frame.ColliderTypes.Trigger, BoxData.ColliderDetection.Stay);
        //
        // }
        //
        // private void OnTriggerExit2D(Collider2D other) {
        //     OnTouch(other, Frame.ColliderTypes.Trigger, BoxData.ColliderDetection.Exit);
        //     
        // }
        //
        // private void OnCollisionEnter2D(Collision2D other) {
        //     OnTouch(other, Frame.ColliderTypes.NoTrigger, BoxData.ColliderDetection.Enter);
        // }
        //
        // private void OnCollisionStay2D(Collision2D other) {
        //     OnTouch(other, Frame.ColliderTypes.NoTrigger, BoxData.ColliderDetection.Stay);
        //     
        // }
        //
        // private void OnCollisionExit2D(Collision2D other) {
        //     OnTouch(other, Frame.ColliderTypes.NoTrigger, BoxData.ColliderDetection.Exit);
        //     
        // }
        //

        private void OnTouch(Collider2D other, Frame.ColliderTypes colliderType, BoxData.ColliderDetection colliderDetection){
            var layers = CurrentAnimation.Layers;
            for(var i = 0; i < gameObjects.Count; i ++) {
                var group = GetGroupForIndex(i);
                var frame = layers[i].frames[activeFrame];
                var boxCollider2D = gameObjects[i].GetComponent<BoxCollider2D>();

                if (group.colliderDetection != colliderDetection || frame.colliderType != colliderType) continue;
                if (!boxCollider2D.IsTouching(other)) continue;
                
                if(other.gameObject.layer == group.collisionLayer){
                    ApplyHitBoxProperty(CurrentAnimation.Layers[i]);
                }

            }

        }
        private void OnTouch(Collision2D other, Frame.ColliderTypes colliderType, BoxData.ColliderDetection colliderDetection){
            var layers = CurrentAnimation.Layers;
            for(var i = 0; i < gameObjects.Count; i ++) {
                var group = GetGroupForIndex(i);
                if (group.colliderDetection != colliderDetection ||
                    layers[i].frames[activeFrame].colliderType != colliderType) continue;
                if(other.gameObject.layer == group.collisionLayer){
                    
                }

            }

        }

        #endregion
        
        

        public void AddListener(string eventName, PixelAnimatorListener listener){
            if(listeners.ContainsKey(eventName))
                listeners[eventName] += listener;
            else
                listeners[eventName] = listener;
        }

        public void RemoveListener(string eventName, PixelAnimatorListener listener){
            if(!listeners.ContainsKey(eventName)) return;

            listeners[eventName] -= listener;
            if(listeners[eventName] == null)
                listeners.Remove(eventName);
        }



        private BoxData GetGroupForIndex(int index){
            return preferences.GetGroup(CurrentAnimation.Layers[index].Guid);
        }
        
        public void Stop(){
            stop = true;
        }
        


    }
}
