using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using binc.PixelAnimator.PropertyDataProvider;
using binc.PixelAnimator.Common;
using binc.PixelAnimator.Preferences;
using binc.PixelAnimator.Utility;


namespace binc.PixelAnimator.Editor.Window{ 
    
    [Serializable]
    public class PixelAnimatorWindow : EditorWindow{

        
        #region Variable

        private const string PixelAnimatorPath = "Assets/PixelAnimator/";
        private const string ResourcesPath = PixelAnimatorPath + "Editor Default Resources/";
        private Rect timelineRect, partitionHandleRect, burgerRect, backRect, playRect, frontRect, yLayoutLine, xLayoutLine;
        private Rect propertyWindowRect = new(10, 6, 120, 20);
        private Rect[] yLineRects;
        
        private Rect[,] selectFrameRect;
        private List<GroupRects> groupRects;
        private List<Rect> frameRects;

        private static Vector2 _clickedMousePos;
        
        [SerializeField] private Rect spriteWindowRect;
        [SerializeField] private Vector2 spriteOrigin;
        private Vector2 viewOffset;
        
        
        private int activeFrameIndex, activeLayerIndex;
        private PixelAnimation selectedAnimation;
        
        //Editor Delta Time
        private float timer, editorDeltaTime;
        private float lastTimeSinceStartup;


        private readonly Color blackColor = new(0.15f, 0.15f, 0.15f, 1);
        private static readonly Color WhiteGridColour = new(0.75f, 0.75f, 0.75f);
        private static readonly Color BlackGridColour = new(0.5f, 0.5f, 0.5f);
        
        private bool isPlaying, draggableTimeline;
        private Vector2 timeLinePosition;

        private PixelAnimatorPreferences preferences;

        private Texture2D backTex, playTex, frontTex, burgerTex, onMouseAddGroupsTex, singleFrameTex, stopTex, durationTex;
        
        [SerializeField] private Texture2D spritePreview;
        [SerializeField] private Texture2D gridWhiteTex;
        [SerializeField] private Texture2D gridBlackTex;

        private SerializedObject targetAnimation;
        

        private float thumbNailScale;
        [SerializeField] private int spriteScale;

        private Vector2 greatestSpriteSize;
        private GenericMenu boxTypePopup, settingsPopup;
        
        private Vector2 propertyXScrollPos = Vector2.one;
        private Vector2 propertyYScrollPos = Vector2.one;
        private bool reSizingBox;
        
        private bool methodNameFoldout;
        private enum PropertyFocus{HitBox, Sprite}

        private enum WindowFocus{
            Property,
            TimeLine,
            SpriteWindow
        }

        private enum HandleTypes{TopLeft, TopCenter, TopRight, LeftCenter, BottomRight, BottomCenter, BottomLeft, RightCenter, Middle, None}

        [SerializeField] private HandleTypes editingHandle; 
        
        private WindowFocus windowFocus;
        private PropertyFocus propertyFocus;
        private Layer ActiveLayer{
            get{
                if (selectedAnimation != null && selectedAnimation.Layers is{ Count: > 0 }) {
                    return selectedAnimation.Layers[activeLayerIndex];
                    
                }
                return null;
            } 
        }
        #endregion



        [MenuItem("Window/Pixel Animator")]
        private static void InitWindow(){
            var window = GetWindow<PixelAnimatorWindow>("Pixel Animator");
            window.minSize = new Vector2(150, 450);
            window.Show();
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath + "PixelAnimatorIcon.png");
            window.titleContent = new GUIContent("Pixel Animator", icon);
        }
    
        
        private void OnEnable(){
            //Set Rect
            SetInitRect();
            SelectedObject();
            //Load textures
            LoadInitResources();
            gridWhiteTex = new Texture2D(1, 1);
            gridWhiteTex.SetPixel(0, 0, WhiteGridColour);
            gridWhiteTex.Apply();

            gridBlackTex = new Texture2D(1, 1);
            gridBlackTex.SetPixel(0, 0, BlackGridColour);
            gridBlackTex.Apply();
            
            // editingHandle = HandleTypes.None;

        }

        private void SetInitRect(){
            const float buttonSize = 32;// button rect set
            burgerRect = new Rect(15, 15, 48, 48);
            backRect = new Rect(200, 20, buttonSize, buttonSize);
            playRect = new Rect(backRect.width + backRect.xMin + 2, backRect.yMin, buttonSize, buttonSize); 
            frontRect = new Rect(playRect.width + playRect.xMin + 2, backRect.yMin, buttonSize, buttonSize);

            groupRects = new List<GroupRects>();
            frameRects = new List<Rect>();
        }
        private void LoadInitResources(){
            preferences = Resources.Load<PixelAnimatorPreferences>("PixelAnimatorPreferences");
            backTex = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "Back.png", typeof(Texture2D)) ;
            frontTex = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "Front.png", typeof(Texture2D));
            burgerTex = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "AddBoxes.png", typeof(Texture2D));
            onMouseAddGroupsTex = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "AddBoxes2.png", typeof(Texture2D));
            singleFrameTex = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "frame.png", typeof(Texture2D));
            stopTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath + "Stop.png");
            playTex = AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath + "Play.png");
            durationTex = playTex;


        }
 
        private void OnGUI(){
            
            SelectedObject();
            SetWindows();
            
            if (selectedAnimation == null) return;
            

            if (selectedAnimation.Layers != null && activeLayerIndex >= selectedAnimation.Layers.Count || activeLayerIndex < 0) {
                activeLayerIndex = 0;    
            }
            
            if (activeFrameIndex >= selectedAnimation.GetSpriteList().Count || activeFrameIndex < 0) {
                activeFrameIndex = 0;
            }

            if (selectedAnimation.Layers.Count > 0)
                reSizingBox = editingHandle != HandleTypes.None;
            
            
            SetAddLayerPopup();
            SetFrameCopyPaste();

            if (GUI.GetNameOfFocusedControl() == "") {
                SetLayerKeys();
                SetPlayKeys();
            }
            
        }
        
        private void SetWindows(){

            BeginWindows();
            
            CreateTimeline();
            DrawPropertyWindow();
            DrawSpriteWindow(Event.current);
            
            EndWindows();
            
            CheckFocus();
            GUI.BringWindowToFront(4);
            GUI.BringWindowToFront(5);
            GUI.BringWindowToFront(2);
            GUI.BringWindowToBack(1);

            
        }

        private void CheckFocus(){
            switch (windowFocus) {
                case WindowFocus.TimeLine or WindowFocus.SpriteWindow :
                    if(Event.current.type == EventType.MouseDown){ 
                        GUI.FocusControl(null);    
                    }
                    break;
            }
        }

        
        #region SpriteWindow
        private void DrawSpriteWindow(Event evtCurrent){
            if (selectedAnimation == null) return;
            
            spritePreview = AssetPreview.GetAssetPreview(selectedAnimation.GetSpriteList()[activeFrameIndex]);
            if (spritePreview == null) return;
            
            if (evtCurrent.button == 2) viewOffset += evtCurrent.delta * 0.5f;
            if (evtCurrent.type == EventType.ScrollWheel) {
                var inversedDelta = Mathf.Sign(evtCurrent.delta.y) < 0 ? 1: -1;
                spriteScale += inversedDelta;
            }

            spriteScale = Mathf.Clamp(spriteScale, 1, (int)(position.height / spritePreview.height));

            spriteWindowRect.position = new Vector2(viewOffset.x + spriteOrigin.x, viewOffset.y + spriteOrigin.y);
            spriteWindowRect.size = new Vector2(spritePreview.width * spriteScale, spritePreview.width * spriteScale);


            if (spriteWindowRect.Contains(evtCurrent.mousePosition) && evtCurrent.type == EventType.MouseDown) 
                windowFocus = WindowFocus.SpriteWindow;
            
                
            GUI.Window(1, spriteWindowRect, SpriteWindowFunc, GUIContent.none, GUIStyle.none);
            UpdateScale();
            
            //Drawing outline
            const float outLineWidth = 3f; 
            var outLinePos = spriteWindowRect.position - Vector2.one * outLineWidth;
            EditorGUI.DrawRect(new Rect(outLinePos, new Vector2( spriteWindowRect.width + outLineWidth * 2, spriteWindowRect.size.y + outLineWidth * 2)), Color.white);

        }
        
        private void SpriteWindowFunc(int windowID){
            spritePreview = AssetPreview.GetAssetPreview(selectedAnimation.GetSpriteList()[activeFrameIndex]);
            if (spritePreview == null) return;
            var spriteRect = new Rect(0, 0, spritePreview.width * spriteScale, spritePreview.height * spriteScale );
            
            DrawGrid(spriteRect);
            GUI.DrawTexture(spriteRect, spritePreview, ScaleMode.ScaleToFit); //our sprite
            spritePreview.filterMode = FilterMode.Point;

            if(selectedAnimation.Layers.Count > 0)
                foreach (var layer in selectedAnimation.Layers) {
                    DrawBox(layer, preferences.GetGroup(layer.Guid), 
                        activeFrameIndex, spriteScale, new Vector2(spritePreview.width, spritePreview.height), editingHandle);

                }

            var inAnyBox = selectedAnimation.Layers.Any(layer => layer.frames[activeFrameIndex].hitBoxRect.Contains(Event.current.mousePosition/spriteScale));
            Debug.Log(inAnyBox);
            if (!inAnyBox && spriteRect.Contains(Event.current.mousePosition) &&
                Event.current.type == EventType.MouseDown && editingHandle == HandleTypes.None) {
                propertyFocus = PropertyFocus.Sprite;
            }
            SetPlayKeys();
            SetBox();

        }

        private void DrawGrid(Rect rect){
            var grid = new Rect(rect.x, rect.y, 16 * spriteScale, 16 * spriteScale); //define a single 16x16 tile
            
            for (var i = 0; i < spritePreview.width / 16; i++) { //iterate over X
            
                for (var j = 0; j < spritePreview.height / 16; j += 2) { //iterate over j += 2
                    var tex = i % 2 == 0 ? gridWhiteTex : gridBlackTex; 
                    GUI.DrawTexture(grid, tex); //draw white box
                    grid.y += grid.height; //increment y axis
                    var texTwo = tex == gridWhiteTex ? gridBlackTex : gridWhiteTex; 
                    GUI.DrawTexture(grid, texTwo); //draw black box
                    grid.y += grid.height; 
                }

                grid.y = rect.y;
                grid.x += grid.width;
                
            }
            //Additional column if necessary
            if (!(rect.x + rect.width - grid.x > 0)) return;

            grid.width = rect.x + rect.width - grid.x; //prepare for last boxes "remainder" width
    
            for (var j = 0; j < spritePreview.height / 16; j += 2) { //iterate over Y
                GUI.DrawTexture(grid, gridBlackTex); //draw box
                grid.y += grid.height; //increment y axis
                GUI.DrawTexture(grid, gridWhiteTex);
                grid.y += grid.height;
            }
    
            grid.height = rect.y + rect.height - grid.y; //prepare for last box "remainder" height (AND width)
            if (rect.y + rect.height - grid.y > 0) GUI.DrawTexture(grid, gridBlackTex); //draw last box in column
        }
        
        private void UpdateScale(){
            if (spritePreview == null) return;

            var adjustedSpriteWidth = spritePreview.width * spriteScale;
            var adjustedSpriteHeight = spritePreview.height * spriteScale;
            var adjustedPosition = new Rect(Vector2.zero, position.size);
            adjustedPosition.width += 10;
            adjustedPosition.height -= adjustedPosition.yMax - timelineRect.y;
            spriteOrigin.x = adjustedPosition.width * 0.5f - spritePreview.width * 0.5f * spriteScale;
            spriteOrigin.y = adjustedPosition.height * 0.5f - spritePreview.height * 0.5f * spriteScale;
            
            //handle the canvas view bounds X
            if (viewOffset.x > adjustedSpriteWidth * 0.5f)
                viewOffset.x = adjustedSpriteWidth * 0.5f;
            if (viewOffset.x < -adjustedSpriteWidth * 0.5f)
                viewOffset.x = -adjustedSpriteWidth  * 0.5f;

        
            //handle the canvas view bounds Y
            if (viewOffset.y > adjustedSpriteHeight * 0.5f)
                viewOffset.y = adjustedSpriteHeight * 0.5f;
            if (viewOffset.y < -adjustedSpriteHeight * 0.5f)
                viewOffset.y = -adjustedSpriteHeight * 0.5f;
        }
        
        private void SetBox(){
            if (targetAnimation == null) return;
            var evtCurrent = Event.current;
            targetAnimation.Update();
            if(selectedAnimation.Layers.Count > 0){
                
                for(var i = 0; i < selectedAnimation.Layers.Count; i++){
                    var layer = selectedAnimation.Layers[i];
                    var frame = layer.frames[activeFrameIndex];

                    frame.hitBoxRect.width  = Mathf.Clamp( frame.hitBoxRect.width, 0, float.MaxValue );
                    frame.hitBoxRect.height = Mathf.Clamp( frame.hitBoxRect.height, 0, float.MaxValue );


                    if (frame.hitBoxRect.Contains(evtCurrent.mousePosition) && evtCurrent.button == 0
                        && evtCurrent.type == EventType.MouseDown && !reSizingBox && !ActiveLayer
                            .frames[activeFrameIndex].hitBoxRect.Contains(evtCurrent.mousePosition)) {
                        activeLayerIndex = i;
                        layer.activeBox = true;
                    }
                    else if (activeLayerIndex != i)
                        layer.activeBox = false;

                    

                }

                ActiveLayer.activeBox = true;

            }
            targetAnimation.ApplyModifiedProperties();
            
        }
        
        private Rect DrawBox(Layer layer, BoxData boxData, int index, int scale, Vector2 spriteSize, HandleTypes handleTypes){
            var eventCurrent = Event.current;
            var activeBox = selectedAnimation.Layers.IndexOf(layer) == activeLayerIndex && propertyFocus == PropertyFocus.HitBox;
            var frame = layer.frames[index];
            var rectColor = boxData.color;

            var rect = frame.hitBoxRect;
            
            rect.position *=  scale;
            rect.size *= scale;


            if (activeBox) {

                const float size = 8.5f;
                var rTopLeft = new Rect(rect.xMin - size/2, rect.yMin - size/2, size, size);
                var rTopCenter = new Rect(rect.xMin + rect.width/2 - size/2, rect.yMin - size/2, size, size);
                var rTopRight = new Rect(rect.xMax - size/2, rect.yMin - size/2, size, size);
                var rRightCenter = new Rect(rect.xMax - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
                var rBottomRight = new Rect(rect.xMax - size/2, rect.yMax - size/2, size, size);
                var rBottomCenter = new Rect(rect.xMin + rect.width/2 - size/2, rect.yMax - size/2, size, size);
                var rBottomLeft = new Rect(rect.xMin - size/2, rect.yMax - size/2, size, size);
                var rLeftCenter = new Rect(rect.xMin - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
                var rAdjustedMiddle = new Rect(rect.x + size/2, rect.y + size/2, rect.width - size, rect.height - size);

                    
                if(eventCurrent.button == 0 && eventCurrent.type == EventType.MouseDown ){
                    if(rTopLeft.Contains(eventCurrent.mousePosition))
                         editingHandle = HandleTypes.TopLeft;
                    else if(rTopCenter.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.TopCenter;
                    else if(rTopRight.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.TopRight;
                    else if(rRightCenter.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.RightCenter;
                    else if(rBottomRight.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.BottomRight;
                    else if(rBottomCenter.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.BottomCenter;
                    else if(rBottomLeft.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.BottomLeft;
                    else if(rLeftCenter.Contains(eventCurrent.mousePosition))
                        editingHandle = HandleTypes.LeftCenter;
                    else if(rAdjustedMiddle.Contains(eventCurrent.mousePosition)){
                        editingHandle = HandleTypes.Middle;
                        _clickedMousePos = eventCurrent.mousePosition;
                    }
                    else {
                        
                        editingHandle = HandleTypes.None;
                    }
                }
                
                if (eventCurrent.type == EventType.MouseDrag && eventCurrent.type != EventType.MouseUp) {
                    switch (editingHandle) {
                        case HandleTypes.TopLeft:
                            frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
                            frame.hitBoxRect.yMin = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.TopCenter:
                            frame.hitBoxRect.yMin  = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.TopRight:
                            frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
                            frame.hitBoxRect.yMin  = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.RightCenter:
                            frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
                            break;
                        case HandleTypes.BottomRight:
                            frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
                            frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.BottomCenter:
                            frame.hitBoxRect.yMax  = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.BottomLeft:
                            frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
                            frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y/scale;
                            break;
                        case HandleTypes.LeftCenter:
                            frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
                            break;
                        case HandleTypes.Middle:
                            
                            var deltaX = (_clickedMousePos.x - rect.xMin)/scale;
                            var deltaY = (_clickedMousePos.y - rect.yMin)/scale;
                            
                            frame.hitBoxRect.position = new Vector2((int)eventCurrent.mousePosition.x/scale - (int)deltaX, (int)eventCurrent.mousePosition.y/scale - (int)deltaY);    
                            frame.hitBoxRect.size = new Vector2((int)rect.size.x/scale, (int)rect.size.y/scale);          
                            _clickedMousePos = eventCurrent.mousePosition;
                            break;
                        case HandleTypes.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(handleTypes), handleTypes, null);
                    }
                }
                
                EditorGUI.DrawRect( rTopLeft , rectColor );
                EditorGUI.DrawRect( rTopCenter , rectColor );
                EditorGUI.DrawRect( rTopRight , rectColor );
                EditorGUI.DrawRect( rRightCenter , rectColor );
                EditorGUI.DrawRect( rBottomRight , rectColor );
                EditorGUI.DrawRect( rBottomCenter , rectColor );
                EditorGUI.DrawRect( rBottomLeft , rectColor );
                EditorGUI.DrawRect( rLeftCenter , rectColor );

                
                EditorGUIUtility.AddCursorRect(rTopLeft, MouseCursor.ResizeUpLeft);
                EditorGUIUtility.AddCursorRect(rTopCenter, MouseCursor.ResizeVertical);
                EditorGUIUtility.AddCursorRect(rTopRight, MouseCursor.ResizeUpRight);
                EditorGUIUtility.AddCursorRect(rRightCenter, MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(rBottomRight, MouseCursor.ResizeUpLeft);
                EditorGUIUtility.AddCursorRect(rBottomCenter, MouseCursor.ResizeVertical);
                EditorGUIUtility.AddCursorRect(rBottomLeft, MouseCursor.ResizeUpRight);
                EditorGUIUtility.AddCursorRect(rLeftCenter, MouseCursor.ResizeHorizontal);
                EditorGUIUtility.AddCursorRect(rAdjustedMiddle, MouseCursor.MoveArrow);

                rect.width = Mathf.Clamp(rect.width, 0, int.MaxValue);
                rect.height = Mathf.Clamp(rect.height, 0, int.MaxValue);

                frame.hitBoxRect.x = Mathf.Clamp(frame.hitBoxRect.x, 0, spriteSize.x - frame.hitBoxRect.width);
                frame.hitBoxRect.y = Mathf.Clamp(frame.hitBoxRect.y, 0, spriteSize.y - frame.hitBoxRect.height);
                frame.hitBoxRect.width = Mathf.Clamp(frame.hitBoxRect.width, 0, spriteSize.x - frame.hitBoxRect.x);
                frame.hitBoxRect.height = Mathf.Clamp(frame.hitBoxRect.height, 0, spriteSize.y - frame.hitBoxRect.y);
                
                if(eventCurrent.type == EventType.MouseUp) {
                    editingHandle = HandleTypes.None;
                }
                
            }
            
            var activeFrame = ActiveLayer.frames[activeFrameIndex];
            var activeRect = new Rect(activeFrame.hitBoxRect.position * scale, activeFrame.hitBoxRect.size * scale);
            
            if (!activeBox && rect.Contains(eventCurrent.mousePosition) && eventCurrent.button == 0 &&
                eventCurrent.type == EventType.MouseDown && editingHandle == HandleTypes.None && !activeRect.Contains(eventCurrent.mousePosition)) {
                propertyFocus = PropertyFocus.HitBox;
                activeLayerIndex = selectedAnimation.Layers.IndexOf(layer);
            }


            var color = activeBox ? new Color(rectColor.r, rectColor.g, rectColor.b, 0.2f) : Color.clear;
            Handles.DrawSolidRectangleWithOutline( rect, color, rectColor );
            
            return rect;

        }
        
        #endregion
        
        #region Property
        
        private void DrawProperties(PropertyType propertyType, string header){
            if (targetAnimation == null || selectedAnimation == null) return;
            
            using var yScroll = new EditorGUILayout.ScrollViewScope(propertyYScrollPos);
            using var xScroll = new EditorGUILayout.ScrollViewScope(propertyXScrollPos);
            targetAnimation.Update();
            
            EditorGUI.LabelField(propertyWindowRect, header, EditorStyles.boldLabel);
            EditorGUI.DrawRect( new Rect(7, 30, 300, 2f), new Color(0.3f, 0.3f, 0.3f, 0.6f) ); //Drawing background.
            
            GUILayout.Space(30);
            propertyXScrollPos = xScroll.scrollPosition;
            
            using(new GUILayout.HorizontalScope()){        
                GUILayout.Space(20);
                
                using(new GUILayout.VerticalScope()){
                    GUILayout.Space(20);
                    switch (propertyType) {
                        case PropertyType.Sprite:
                            
                            var propPixelSprite = targetAnimation.FindProperty("pixelSprites")
                                .GetArrayElementAtIndex(activeFrameIndex);
                            
                            var propSpriteData = propPixelSprite.FindPropertyRelative("spriteData");
                            var propSpriteEventNames = propSpriteData.FindPropertyRelative("eventNames");
                            var propSpriteDataValues = propSpriteData.FindPropertyRelative("propertyValues");
                            var spriteDataValues = selectedAnimation.PixelSprites[activeFrameIndex].SpriteData.propertyValues;
                            
                            foreach (var prop in preferences.SpriteProperties) {
                                var single = spriteDataValues.FirstOrDefault(x => x.baseData.Guid == prop.Guid).baseData;
                                var selectedIndex = single == null ? -1 : spriteDataValues.FindIndex(x => x.baseData == single); 
                                SetPropertyField(prop, propSpriteDataValues, single, selectedIndex);
                            }
                            SetMethodField(propSpriteEventNames);
                            targetAnimation.ApplyModifiedProperties();

                            break;
                        case PropertyType.HitBox:
                            if(selectedAnimation.Layers.Count <= 0) return;
                            var layer = targetAnimation.FindProperty("layers").GetArrayElementAtIndex(activeLayerIndex);
                            var frame = layer.FindPropertyRelative("frames").GetArrayElementAtIndex(activeFrameIndex);

                            var propColliderType = frame.FindPropertyRelative("colliderType");
                            EditorGUILayout.PropertyField(propColliderType);
                            
                            using (new GUILayout.HorizontalScope()) {
                                EditorGUILayout.LabelField("Box", GUILayout.Width(70));
                                var propHitBoxRect = frame.FindPropertyRelative("hitBoxRect");
                                EditorGUILayout.PropertyField(propHitBoxRect, GUIContent.none, GUILayout.Width(140),
                                    GUILayout.MaxHeight(60));
                            }

                            targetAnimation.ApplyModifiedProperties();

                            var propHitBoxData = frame.FindPropertyRelative("hitBoxData");
                            var propHitBoxEventNames = propHitBoxData.FindPropertyRelative("eventNames");
                            var propHitBoxDataValues = propHitBoxData.FindPropertyRelative("propertyValues");
                            
                            var hitBoxDataValues = selectedAnimation.Layers[activeLayerIndex].frames[activeFrameIndex]
                                .HitBoxData.propertyValues;
                            foreach (var prop in preferences.HitBoxProperties) {
                                var single = hitBoxDataValues.FirstOrDefault(x => x.baseData.Guid == prop.Guid).baseData;
                                var selectedIndex = single == null
                                    ? -1
                                    : hitBoxDataValues.FindIndex(x => x.baseData == single);
                                SetPropertyField(prop, propHitBoxDataValues, single, selectedIndex);
                            }
                            SetMethodField(propHitBoxEventNames);
                            break;
                        
                        default:
                            throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, null);
                    }


                    targetAnimation.ApplyModifiedProperties();
                }
            }
            
        }
        private void DrawPropertyWindow(){
            var tempColor = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.2f);

            switch (propertyFocus) {
                case PropertyFocus.HitBox:
                    GUI.Window(4, new Rect(10, 10, 360, 280), 
                        _ => { DrawProperties(PropertyType.HitBox, "HitBox Properties"); }, GUIContent.none);
                    break;
                case PropertyFocus.Sprite:
                    GUI.Window(5, new Rect(10, 10, 360, 280), 
                        _ => { DrawProperties(PropertyType.Sprite, "Sprite Properties"); }, GUIContent.none);
                        
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (propertyWindowRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                windowFocus = WindowFocus.Property;
            }
            
            GUI.color = tempColor;

        }
        
        private static void AddPropertyValue(SerializedProperty propertyValues, PropertyDataWarehouse propDataWarehouse){
            propertyValues.InsertArrayElementAtIndex(propertyValues.arraySize);
            propertyValues.serializedObject.ApplyModifiedProperties();
            propertyValues.serializedObject.Update();
            var propPropertyValue = propertyValues.GetArrayElementAtIndex(propertyValues.arraySize - 1);
            var propBaseData = propPropertyValue.FindPropertyRelative("baseData");
            var propDataType = propPropertyValue.FindPropertyRelative("dataType");
            propDataType.intValue = (int)propDataWarehouse.dataType;
            propBaseData.managedReferenceValue =
                PixelAnimatorUtility.CreateBlankBaseData(propDataWarehouse.dataType);

            propBaseData.FindPropertyRelative("guid").stringValue = propDataWarehouse.Guid;
        }
        
        
        private static void SetPropertyField(PropertyDataWarehouse propDataWarehouse, SerializedProperty propertyValues, BaseData baseData, int index){
            propertyValues.serializedObject.Update();
            var alreadyExist = baseData != null;
            
            using(new GUILayout.HorizontalScope()){
                EditorGUILayout.LabelField(propDataWarehouse.Name, GUILayout.MaxWidth(70));
                

                if (alreadyExist) {
                    var propertyData = propertyValues.GetArrayElementAtIndex(index).FindPropertyRelative("baseData").FindPropertyRelative("data");
                    EditorGUILayout.PropertyField(propertyData, GUIContent.none, GUILayout.Width(90));
                    propertyValues.serializedObject.ApplyModifiedProperties();
                }
                else {
                    PixelAnimatorUtility.SystemObjectPreviewField(PixelAnimatorUtility.DataTypeToSystemObject(propDataWarehouse.dataType), GUILayout.Width(90));
                }
                
                
                GUILayout.Space(10);
                if(GUILayout.Button("X", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15))){
                    if (alreadyExist) {
                        propertyValues.DeleteArrayElementAtIndex(index);
                    }
                    else {
                        AddPropertyValue(propertyValues, propDataWarehouse);
                        
                    }

                    propertyValues.serializedObject.ApplyModifiedProperties();


                }
            }

        }

        private void SetMethodField(SerializedProperty eventNames){
            GUILayout.Space(20);

            methodNameFoldout = EditorGUILayout.Foldout(methodNameFoldout, "Event Names", true);
            if (methodNameFoldout == false) return;
            for (var i = 0; i < eventNames.arraySize; i ++) {
                var methodName = eventNames.GetArrayElementAtIndex(i);
                using (new GUILayout.HorizontalScope()) {
                    EditorGUILayout.PropertyField(methodName, GUIContent.none, GUILayout.MaxWidth(140));
                    if (GUILayout.Button("X", GUILayout.MaxWidth(15), GUILayout.MaxHeight(15))) {
                        eventNames.DeleteArrayElementAtIndex(i);
                        eventNames.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            if (GUILayout.Button("Add Event", GUILayout.MaxWidth(100))) {
                eventNames.arraySize++;
                eventNames.serializedObject.ApplyModifiedProperties();
            }
            eventNames.serializedObject.ApplyModifiedProperties();

        }
        
        #endregion

        #region Timeline

        private void SetTimelineRect(){
            var eventCurrent = Event.current;

            const float partitionHeight = 8;
            var topTimelineRect = new Rect( timelineRect.xMin, timelineRect.y - partitionHeight, timelineRect.xMax - timelineRect.xMin, partitionHeight );
            partitionHandleRect = new Rect(0, topTimelineRect.y + partitionHeight/2, topTimelineRect.width, partitionHeight);

            timelineRect.size = new Vector2(position.width + 10, position.height + 100 );
            var clampYPosition = Mathf.Clamp(timelineRect.position.y, 200, position.height - 200);
            timelineRect.position = new Vector2(0, clampYPosition);
            
            EditorGUIUtility.AddCursorRect( partitionHandleRect, MouseCursor.ResizeVertical );
            


            if(draggableTimeline && eventCurrent.type == EventType.MouseDrag &&  !reSizingBox && eventCurrent.button == 0){
                
                if(eventCurrent.mousePosition.y < timelineRect.position.y && Math.Sign(eventCurrent.delta.y) == -1 ) 
                    timeLinePosition = new Vector2(timelineRect.x, clampYPosition + eventCurrent.delta.y);
                
                if(eventCurrent.mousePosition.y > timelineRect.position.y && Mathf.Sign(eventCurrent.delta.y) == 1)
                    timeLinePosition = new Vector2(timelineRect.x, clampYPosition + eventCurrent.delta.y);
                
                timelineRect.position = timeLinePosition;

            }
            
            
            switch (eventCurrent.type) {
                // drag timeline
                case EventType.MouseDrag:{
                    if(partitionHandleRect.Contains(eventCurrent.mousePosition)) draggableTimeline = true;
                    Repaint();
                    break;
                }
                case EventType.MouseUp:
                    draggableTimeline = false;
                    break;
            }

        }
        
        private void CreateTimeline(){
            
            SetTimelineRect();
            timelineRect = GUILayout.Window( 2, timelineRect, TimelineFunction, GUIContent.none, GUIStyle.none);
            if (timelineRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown) {
                windowFocus = WindowFocus.TimeLine;
            }
            
        }

        private void TimelineFunction(int windowID){

            yLayoutLine = new Rect(frontRect.xMax + 15, 10, 4, timelineRect.height);
            
            EditorGUI.DrawRect(new Rect(0,0, timelineRect.width, timelineRect.height), new Color(0.1f,0.1f,0.1f,1));
            EditorGUI.DrawRect(new Rect(0,0, timelineRect.width, 10), blackColor);
            EditorGUI.DrawRect(new Rect(yLayoutLine.xMax, xLayoutLine.yMax, timelineRect.width, timelineRect.height), 
                new Color(0, 0, 0.03f, 1)
                );

            
            // Thumbnail Scale 
            using(var change = new EditorGUI.ChangeCheckScope()){
                thumbNailScale = EditorGUI.FloatField(new Rect(10, 250, 120, 30),thumbNailScale);
                if(change.changed){
                    while (thumbNailScale * greatestSpriteSize.y < 64) {
                        thumbNailScale += 0.1f;
                    }
                    greatestSpriteSize *= thumbNailScale;
                }
            }
            
            xLayoutLine = new Rect(0, greatestSpriteSize.y + 10, timelineRect.width, 4);

            EditorGUI.DrawRect(xLayoutLine, blackColor );
            EditorGUI.DrawRect(yLayoutLine, blackColor);
            

            DrawTimelineButtons();
            SetPlayKeys();
            if (selectedAnimation == null) return;
            if (selectedAnimation.GetSpriteList().Count > 0) {
                if (selectedAnimation.Layers.Count > 0 ) {
                    DrawLayer();
                    SetFrames();
                    SetLayerMenu();

                }
                SetFrameThumbnail();
                SetSelectedFrame();
            }
            
            SetLayerKeys();
        }

        private void DrawTimelineButtons(){
            if (!isPlaying) durationTex = playTex;
            else durationTex = stopTex;
            using(new GUILayout.HorizontalScope()){

                if(PixelAnimatorUtility.Button(burgerTex, onMouseAddGroupsTex, burgerRect )) {
                    boxTypePopup?.ShowAsContext();
                }
                else if(PixelAnimatorUtility.Button(backTex, backRect)) {
                    if (selectedAnimation == null) return;
                        switch (activeFrameIndex) {
                            case 0:
                                var index = selectedAnimation.GetSpriteList().Count - 1; 
                                activeFrameIndex = index;
                                break;
                            case >0:
                                activeFrameIndex--;
                                break;
                        }
                        Repaint();
                }
                else if(PixelAnimatorUtility.Button(durationTex, playRect)){
                    if (selectedAnimation == null) return;
                    isPlaying = !isPlaying;
                    durationTex = isPlaying ? stopTex : playTex;
                    if(isPlaying && selectedAnimation.frameRate == 0) Debug.Log("Frame rate is zero");
                    Repaint();
                }
                else if(PixelAnimatorUtility.Button(frontTex, frontRect)){
                    if (selectedAnimation == null) return;
                    activeFrameIndex = (activeFrameIndex + 1) % selectedAnimation.GetSpriteList().Count;
                    Repaint();
                }

            }
        } 

        
        private void SetAddLayerPopup(){
            settingsPopup = new GenericMenu{allowDuplicateNames = true};
            boxTypePopup = new GenericMenu{allowDuplicateNames = true};
    
            
            boxTypePopup.AddItem(new GUIContent("Go to Preferences"), false, () => {
                Selection.activeObject = preferences;

            });
            
            boxTypePopup.AddItem(new GUIContent("Update Animation"), false, () => {
                foreach (var frame in selectedAnimation.Layers.SelectMany(layer => layer.frames)) {
                    var hitBoxData = frame.HitBoxData;
                    for (var i = 0; i < hitBoxData.propertyValues.Count; i++) {
                        
                        var propertyValue = hitBoxData.propertyValues[i];
                        var myGuid = propertyValue.baseData.Guid;
                        var exist = preferences.HitBoxProperties.Any(x => x.Guid == myGuid);
                        
                        if (exist) continue;
                        hitBoxData.propertyValues.Remove(propertyValue);
                        i = -1;
                    }
                }
            });
            
            boxTypePopup.AddSeparator("");
            var groups = preferences.BoxData;


            settingsPopup.AddItem(new GUIContent("Settings/Delete"), false, () => {
                    targetAnimation.Update();
                    targetAnimation.FindProperty("layers").DeleteArrayElementAtIndex(activeLayerIndex);
                    targetAnimation.ApplyModifiedProperties();
                });

            for(var i = 0; i < preferences.BoxData.Count; i ++) {


                boxTypePopup.AddItem(new GUIContent(groups[i].boxType), false, (userData) => {
                    targetAnimation.Update();
                    var group = (BoxData)userData;

                    if (selectedAnimation.Layers.Any(x => x.Guid == group.Guid)) {
                        Debug.LogError("This boxData has already been added! Please add another boxData.");
                        return;
                    }
                    selectedAnimation.AddLayer(group.Guid);

                }, groups[i]);
                

            }

        }

        private void SetSelectedFrame(){
            var topLeft = new Rect();
            var topRight = new Rect();
            var bottomLeft = new Rect();
            var bottomRight = new Rect();
            
            switch (propertyFocus) {
                case PropertyFocus.HitBox:
                    if (selectedAnimation.Layers.Count <= 0) break;
                    topLeft = new Rect( yLineRects[activeFrameIndex].xMin - greatestSpriteSize.x, groupRects[activeLayerIndex].bodyRect.yMin, 15, 15 );
                    topRight = new Rect( yLineRects[activeFrameIndex].xMin - 15, groupRects[activeLayerIndex].bodyRect.yMin, 15, 15 );
                    bottomLeft = new Rect( yLineRects[activeFrameIndex].xMin - greatestSpriteSize.x, groupRects[activeLayerIndex].bottomLine.yMin - 15, 15, 15 );
                    bottomRight = new Rect( yLineRects[activeFrameIndex].xMin - 15, groupRects[activeLayerIndex].bottomLine.yMin - 15, 15, 15 );

                    break;
                case PropertyFocus.Sprite:
                    const float size = 15;
                    var yLineRect = yLineRects[activeFrameIndex];
                    var leftX = yLineRect.x - greatestSpriteSize.x;
                    var rightX = yLineRect.x - size;
                    topLeft = new Rect(leftX, yLineRect.yMin, size, size);
                    topRight = new Rect(rightX, yLineRect.yMin, size, size);
                    bottomLeft = new Rect(leftX, xLayoutLine.y - 15, size, size);
                    bottomRight = new Rect(rightX, xLayoutLine.y - 15, size, size);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            GUI.DrawTexture(topLeft, (Texture2D)AssetDatabase.LoadAssetAtPath( ResourcesPath + "Top Left.png", typeof(Texture2D)) );
            GUI.DrawTexture(topRight, (Texture2D)AssetDatabase.LoadAssetAtPath( ResourcesPath + "Top Right.png", typeof(Texture2D)));
            GUI.DrawTexture(bottomLeft, (Texture2D)AssetDatabase.LoadAssetAtPath( ResourcesPath + "Bottom Left.png", typeof(Texture2D)));
            GUI.DrawTexture(bottomRight, (Texture2D)AssetDatabase.LoadAssetAtPath( ResourcesPath + "Bottom Right.png", typeof(Texture2D)));

        }
        private void SetFrameThumbnail(){

            var eventCurrent = Event.current;
            
            greatestSpriteSize = GetGreatestSpriteSize();
            if (yLineRects.Length != selectedAnimation.GetSpriteList().Count) {
                yLineRects = new Rect[selectedAnimation.GetSpriteList().Count];
            }
            // First y axis line rect
            yLineRects[0] = new Rect(yLayoutLine.xMax + greatestSpriteSize.x, 10, yLayoutLine.width, greatestSpriteSize.y + xLayoutLine.height);
            for(var i = 0; i < selectedAnimation.GetSpriteList().Count; i++){
                
                if(i>0 && i != yLineRects.Length-1)
                    yLineRects[i] = new Rect(yLineRects[i-1].xMax + greatestSpriteSize.x, 10, yLayoutLine.width, greatestSpriteSize.y + xLayoutLine.height);

                if (i == yLineRects.Length - 1 && yLineRects.Length != 1)
                    yLineRects[i] = new Rect(yLineRects[i - 1].xMax + greatestSpriteSize.x, 10, yLayoutLine.width,
                        greatestSpriteSize.y + xLayoutLine.height + selectedAnimation.Layers.Count * 39);
                    
                if(i != selectedAnimation.GetSpriteList().Count){
                    var sprite = selectedAnimation.GetSpriteList()[i];

                    var width = sprite.rect.width;
                    var height = sprite.rect.height;
                    var adjustedSpriteWidth  = width* thumbNailScale;
                    var adjustedSpriteHeight = height * thumbNailScale ;
                    
                    // Set sprite x pos
                    var adjustedSpriteXPos   = yLineRects[i].xMin - greatestSpriteSize.x + (greatestSpriteSize.x/2 - adjustedSpriteWidth/2) ;
                    // Set sprite y pos
                    var adjustedSpriteYPos   = greatestSpriteSize.x/2 - adjustedSpriteHeight/2 + 10;

                    
                    
                    var spriteRect = new Rect(adjustedSpriteXPos, adjustedSpriteYPos, adjustedSpriteWidth, adjustedSpriteHeight);
                    var transparentRect = new Rect( yLineRects[i].xMin - greatestSpriteSize.x , yLineRects[i].yMin, greatestSpriteSize.x, timelineRect.height);
                    var spriteSelectableRect = new Rect(adjustedSpriteXPos, adjustedSpriteYPos, greatestSpriteSize.x, greatestSpriteSize.y);


                
                    GUI.DrawTexture(spriteRect, AssetPreview.GetAssetPreview(sprite));
                    
                    if(activeFrameIndex == i){
                        EditorGUI.DrawRect(transparentRect, new Color(255, 255, 255, 0.2f));
                        Repaint();
                    }
                    if (spriteSelectableRect.Contains(eventCurrent.mousePosition) && eventCurrent.button == 0 &&
                        eventCurrent.type == EventType.MouseDown) {
                        propertyFocus = PropertyFocus.Sprite;
                        activeFrameIndex = i;
                        Repaint();
                    } 
                    
                }
                EditorGUI.DrawRect(yLineRects[i], blackColor);
            }

        }
        
        
        private void DrawLayer(){
            
            var width = yLayoutLine.xMin - xLayoutLine.xMin;
            const int height = 40;
            const int bottomLineHeight = 4;
            
            var layers = selectedAnimation.Layers;
            groupRects ??= new List<GroupRects>();
            
            for(var i = 0; i < layers.Count; i++) {
                
                var group = preferences.GetGroup(layers[i].Guid);

                if (i == groupRects.Count) {
                    groupRects.Add(new GroupRects());
                }

                var offColor = group.color * Color.gray;

                var moreOffColor = new Color(group.color.r/3, group.color.g/3, group.color.b/3 , 1f);

                const float a = 5;
                var yPos = i == 0 ? xLayoutLine.yMax : xLayoutLine.yMax + i * height;
                var bodyRect = new Rect(xLayoutLine.xMin, yPos, width, height - a);
                var settingsRect = new Rect(xLayoutLine.xMin, yPos, height - a, height - a);
                var bottomLine = new Rect(yLayoutLine.xMax, yPos + height - a, yLineRects[^1].xMin - yLayoutLine.xMax, bottomLineHeight);
                var boxTypeNameRect = new Rect(groupRects[i].settingsRect.xMax + 10,
                    groupRects[i].settingsRect.yMin + bottomLineHeight * 0.5f + a, width, 30);
                groupRects[i] = new GroupRects(bodyRect, settingsRect, bottomLine);


                EditorGUI.DrawRect(bodyRect, offColor);
                EditorGUI.DrawRect(settingsRect, group.color);
                EditorGUI.DrawRect(bottomLine, blackColor);
                EditorGUI.DrawRect(groupRects[i].Parting, moreOffColor);
                var settingsTexture =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "Settings.png", typeof(Texture2D));
                
                GUI.DrawTexture(settingsRect, settingsTexture );
                var evtCurrent = Event.current;
                if (evtCurrent.button is 0 or 1 && evtCurrent.type is EventType.MouseDown) {
                    if (groupRects[i].bodyRect.Contains(evtCurrent.mousePosition)) {
                        activeLayerIndex = i;
                        Repaint();
                    }
                    
                }
                
                var tempColor = GUI.color;
                GUI.color = group.color * 1.5f;

                EditorGUI.LabelField(boxTypeNameRect, group.boxType);
                GUI.color = tempColor;



            }

        }

        
        private void SetLayerMenu(){
            var eventCurrent = Event.current;
            var layers = selectedAnimation.Layers;

            for (var i = 0; i < layers.Count; i++) {
                var group = preferences.GetGroup(layers[i].Guid);
                
                // Set popup
                if(PixelAnimatorUtility.Button(groupRects[i].settingsRect, group.color)){
                    settingsPopup.ShowAsContext();
                    
                }
                if (groupRects[i].bodyRect.Contains(eventCurrent.mousePosition) &&
                    eventCurrent.type == EventType.MouseDown && eventCurrent.button == 1) {
                    settingsPopup.ShowAsContext();
                }
            }
            
            
        }

        

        private void SetFrames(){
            var eventCurrent = Event.current; 
            var layers = selectedAnimation.Layers;
            selectFrameRect = new Rect[layers.Count, selectedAnimation.GetSpriteList().Count];
            const int frameTextureSize = 16;

            while(frameRects.Count < selectedAnimation.GetSpriteList().Count){
                frameRects.Add(new Rect());
            }
        
            for(var f = 0; f < selectedAnimation.GetSpriteList().Count; f ++){
                for(var i = 0; i < layers.Count; i++) {

                    var yLineXMin = yLineRects[f].xMin;
                    var bottomLine = groupRects[i].bottomLine;
                    var bodyRect = groupRects[i].bodyRect;
                    
                    var width =  yLineXMin - (yLineXMin - greatestSpriteSize.x);
                    var height = bottomLine.yMin - bodyRect.yMin;


                    var yHalfPos = bodyRect.yMin + height/2 - frameTextureSize * 0.5f;           
                    var xHalfPos = yLineXMin - greatestSpriteSize.x  + (yLineXMin - (yLineXMin - greatestSpriteSize.x))/2 - frameTextureSize * 0.5f;

                    frameRects[f] = new Rect(xHalfPos, yHalfPos, frameTextureSize, frameTextureSize);
                    
                    selectFrameRect[i, f] = new Rect(yLineXMin - greatestSpriteSize.x, bottomLine.yMin - height, width, height);
                    GUI.DrawTexture(frameRects[f], singleFrameTex);


                    switch (eventCurrent.type) {
                        case EventType.MouseDown when eventCurrent.button == 0 && selectFrameRect[i, f].Contains(eventCurrent.mousePosition):
                            activeFrameIndex = f;
                            propertyFocus = PropertyFocus.HitBox;
                            activeLayerIndex = i;
                            Repaint();
                            break;
                        case EventType.MouseDrag when eventCurrent.button == 0 && selectFrameRect[i, f].Contains(eventCurrent.mousePosition):
                            break;
                    }
                    
                }
            }
        }

        private void SetFrameCopyPaste(){
            if (windowFocus != WindowFocus.TimeLine || propertyFocus != PropertyFocus.HitBox) return;
            var eventCurrent = Event.current;
            if (eventCurrent.type == EventType.ValidateCommand && eventCurrent.commandName == "Copy")
                eventCurrent.Use();
            

            if (eventCurrent.type == EventType.ExecuteCommand && eventCurrent.commandName == "Copy") {
                EditorGUIUtility.systemCopyBuffer =
                    JsonUtility.ToJson(selectedAnimation.Layers[activeLayerIndex].frames[activeFrameIndex]);
                Debug.Log(EditorGUIUtility.systemCopyBuffer);
            }
            
            if (eventCurrent.type == EventType.ValidateCommand && eventCurrent.commandName == "Paste")
                eventCurrent.Use();
            

            if (eventCurrent.type == EventType.ExecuteCommand && eventCurrent.commandName == "Paste") {
                var copiedFrame = JsonUtility.FromJson<Frame>(EditorGUIUtility.systemCopyBuffer);
                
                var frameProp = targetAnimation.FindProperty("layers").GetArrayElementAtIndex(activeLayerIndex)
                    .FindPropertyRelative("frames").GetArrayElementAtIndex(activeFrameIndex);
                
                var hitBoxRectProp = frameProp.FindPropertyRelative("hitBoxRect");
                var colliderType = frameProp.FindPropertyRelative("colliderType");

                colliderType.enumValueIndex = (int)copiedFrame.colliderType;
                hitBoxRectProp.rectValue = copiedFrame.hitBoxRect;
                targetAnimation.ApplyModifiedProperties();

            }
        }

        #endregion

        #region Common
        private Vector2 GetGreatestSpriteSize(){
            var greatestX = selectedAnimation.GetSpriteList().Aggregate((current, next) => 
                current.rect.size.x > next.rect.size.x ? current : next).rect.size.x;
            
            var greatestY = selectedAnimation.GetSpriteList().Aggregate((current, next) => 
                current.rect.size.y > next.rect.size.y ? current : next).rect.size.y;
            var adjustedX = greatestX * thumbNailScale;
            var adjustedY = greatestY * thumbNailScale;

            return new Vector2(adjustedX, adjustedY);
        }
        private void SetPlayKeys(){
            var eventCurrent = Event.current;
            if (!eventCurrent.isKey || eventCurrent.type != EventType.KeyDown) return;

            var keyCode = eventCurrent.keyCode;
            var spriteCount = selectedAnimation.GetSpriteList().Count;
            switch (keyCode) {
                case KeyCode.Return:
                    isPlaying = !isPlaying;
                    break;
                case KeyCode.RightArrow:
                    activeFrameIndex = (activeFrameIndex + 1) % spriteCount;
                    break;
                case KeyCode.LeftArrow when activeFrameIndex != 0:
                    activeFrameIndex--;
                    break;
                case KeyCode.LeftArrow when activeFrameIndex == 0:
                    activeFrameIndex = spriteCount - 1;
                    break;
            }
            
        }

        private void SetLayerKeys(){
            var eventCurrent = Event.current;
            
            var keyCode = eventCurrent.keyCode;
            if (!eventCurrent.isKey || keyCode is not KeyCode.DownArrow && eventCurrent.type == EventType.KeyUp) return;
            Debug.Log("aga?");
            propertyFocus = PropertyFocus.HitBox;
            
            switch (keyCode) {
                case KeyCode.UpArrow when activeLayerIndex == 0:
                    activeLayerIndex = selectedAnimation.Layers.Count - 1;
                    break;
                case KeyCode.UpArrow:
                    activeLayerIndex--;
                    break;
                case KeyCode.DownArrow when selectedAnimation.Layers.Count > 0:
                    activeLayerIndex = (activeLayerIndex + 1) % selectedAnimation.Layers.Count;
                    break;
            }

        }
        
        private void Play(){

            timer += editorDeltaTime * selectedAnimation.frameRate;
            
            if(timer >= 1f){
                timer -= 1f;
                activeFrameIndex = (activeFrameIndex + 1) % selectedAnimation.GetSpriteList().Count;
                
            }
            Repaint();
        }

        private void SetEditorDeltaTime(){

            if(lastTimeSinceStartup == 0f){
                lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
            }
            
            editorDeltaTime = (float)(EditorApplication.timeSinceStartup - lastTimeSinceStartup);
            lastTimeSinceStartup = (float)EditorApplication.timeSinceStartup;
            
            
        }


        private void Update() {
            if(isPlaying) Play();
            else timer = 0;
            SetEditorDeltaTime();
        }
        
        
        private void SelectedObject(){
            foreach(var obj in Selection.objects) {
                if (obj is not PixelAnimation anim) continue;
                targetAnimation = new SerializedObject(anim);

                if (selectedAnimation == anim) continue;
                    
                if (anim.GetSpriteList() != null) {
                    yLineRects = new Rect[anim.GetSpriteList().Count];
                }

                lastTimeSinceStartup = 0;
                spritePreview = AssetPreview.GetAssetPreview(anim.GetSpriteList()[activeFrameIndex]);
                timer = 0;
                selectedAnimation = anim;
                thumbNailScale = 1;
                activeFrameIndex = 0;
                spriteOrigin = new Vector2(position.width/2 - spritePreview.width * 0.5f, position.height/2);
                while (GetGreatestSpriteSize().x * thumbNailScale > 64) {
                    thumbNailScale -= 0.1f;
                }

            }

        }

            
        #endregion
    }



    public struct GroupRects{

        public Rect bodyRect;
        public Rect settingsRect;
        public Rect bottomLine;
        public Rect Parting => new(bodyRect.xMin, bodyRect.yMax, bodyRect.width, 5);


        public GroupRects(Rect bodyRect, Rect settingsRect, Rect bottomLine){
            this.bodyRect = bodyRect;
            this.settingsRect = settingsRect;
            this.bottomLine = bottomLine;

            
        }

    }
}

    


