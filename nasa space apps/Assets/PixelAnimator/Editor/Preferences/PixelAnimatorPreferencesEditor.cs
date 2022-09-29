using System.Collections.Generic;
using binc.PixelAnimator.Preferences;
using binc.PixelAnimator.PropertyDataProvider;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using binc.PixelAnimator.Utility;



namespace binc.PixelAnimator.Editor.Preferences{


    [CustomEditor(typeof(PixelAnimatorPreferences))]
    
    public class PixelAnimatorPreferencesEditor : UnityEditor.Editor{

        #region Var
        private const string ResourcesPath = "Assets/SpriteAnimator/Editor Default Resources/";

        private string groupTypeName, spritePropertyName, hitBoxPropertyName;
        private Texture2D[] textures;
        
        private SerializedObject so;
        private SerializedProperty propBoxData, propSpriteProperties, propHitBoxProperties;
        

        private int activePropIndex;
        private DataType hitBoxProp, spriteProp;
        private PropertyDataWarehouse hitBoxPropType, spritePropType;

        private ReorderableList boxDataList;
        private ReorderableList hitBoxList;
        private ReorderableList spriteList;

        private PropertyDataWarehouse lastInteractedProperty;

        #endregion
        
        private void OnEnable() {


            textures = new Texture2D[4];
            textures[0] = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "drop.png", typeof(Texture2D));
            textures[1] = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "down.png", typeof(Texture2D));
            textures[2] = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "-.png", typeof(Texture2D));
            textures[3] = (Texture2D)AssetDatabase.LoadAssetAtPath(ResourcesPath + "ok.png", typeof(Texture2D));
            
            so = serializedObject;

            propBoxData = so.FindProperty("boxData");
            propSpriteProperties = so.FindProperty("spriteProperties");
            propHitBoxProperties = so.FindProperty("hitBoxProperties");
            
            
            InitGroupList();
            InitPropertyList();

        }
        

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            so.Update();
            boxDataList?.DoLayoutList();
            so.ApplyModifiedProperties();
            spriteList?.DoLayoutList();
            so.ApplyModifiedProperties();
            hitBoxList?.DoLayoutList();
            so.ApplyModifiedProperties();


        }

        #region BoxData

        private void InitGroupList(){
            boxDataList = new ReorderableList(serializedObject, propBoxData,
                true, true, true, true){
                drawElementCallback = DrawGroups,
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = (rect) => {EditorGUI.LabelField(rect, "BoxData");},
                onAddCallback = (list => {
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize ++;
                    list.index = index;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.FindPropertyRelative("guid").stringValue = GUID.Generate().ToString();
                    element.serializedObject.ApplyModifiedProperties();
                })
            };
            

        }
        
        private void DrawGroups(Rect rect, int index, bool isActive, bool isFocused){
            var element = boxDataList.serializedProperty.GetArrayElementAtIndex(index);
            var eventCurrent = Event.current;
            so.Update();

            rect.y += 2;


            var color = element.FindPropertyRelative("color");
            var boxType = element.FindPropertyRelative("boxType");
            var activeLayer = element.FindPropertyRelative("activeLayer");
            var physicMaterial2D = element.FindPropertyRelative("physicMaterial");
            var rounded = element.FindPropertyRelative("rounded");
            var detection = element.FindPropertyRelative("colliderDetection");
            var collisionLayer = element.FindPropertyRelative("collisionLayer");

            var colorRect = new Rect(rect.x, rect.y, 140, EditorGUIUtility.singleLineHeight);
            var boxTypeRect = new Rect(colorRect.xMax + 10, rect.y, 100, EditorGUIUtility.singleLineHeight);
            var activeLayerRect = new Rect(boxTypeRect.xMax + 10, rect.y, 100, EditorGUIUtility.singleLineHeight);
            var physicMaterial2DRect = new Rect(activeLayerRect.xMax + 10, rect.y, 100, EditorGUIUtility.singleLineHeight);
            var roundedRect = new Rect(physicMaterial2DRect.xMax + 10, rect.y, 10, EditorGUIUtility.singleLineHeight);
            var detectionRect = new Rect(roundedRect.xMax + 10, rect.y, 100, EditorGUIUtility.singleLineHeight);
            var collisionLayerRect = new Rect(detectionRect.xMax + 10, rect.y, 100, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.PropertyField(
                colorRect,
                color,
                GUIContent.none
                );

            EditorGUI.PropertyField(
                    boxTypeRect,
                    boxType,
                    GUIContent.none
                );

            EditorGUI.PropertyField(
                activeLayerRect,
                activeLayer,
                GUIContent.none
                );

            EditorGUI.PropertyField(
                physicMaterial2DRect,
                physicMaterial2D,
                GUIContent.none
                );

            EditorGUI.PropertyField(
                roundedRect,
                rounded,
                GUIContent.none
                );

            EditorGUI.PropertyField(
                detectionRect,
                detection,
                GUIContent.none
                );
            
            EditorGUI.PropertyField(
                collisionLayerRect,
                collisionLayer,
                GUIContent.none
            );

            

            //Setting Tool tips
            
            PixelAnimatorUtility.CreateTooltip(activeLayerRect, LayerMask.LayerToName(activeLayer.intValue),
                eventCurrent.mousePosition);
            
            PixelAnimatorUtility.CreateTooltip(collisionLayerRect,
                LayerMask.LayerToName(collisionLayer.intValue), eventCurrent.mousePosition);
            
            PixelAnimatorUtility.CreateTooltip(roundedRect, "Rounded", eventCurrent.mousePosition);
            
            PixelAnimatorUtility.CreateTooltip(boxTypeRect, "Name", eventCurrent.mousePosition);
            
            PixelAnimatorUtility.CreateTooltip(detectionRect, "Detection Type", eventCurrent.mousePosition);
            
    
            element.serializedObject.ApplyModifiedProperties();


        }
        


        
        
        #endregion

        #region Properties
        
        private void InitPropertyList(){
            hitBoxList = new ReorderableList(serializedObject, propHitBoxProperties,
                true, true, true, true){
                drawElementCallback = (rect, index, _, _) => DrawPropertyList(hitBoxList, rect, index) ,
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = rect => {EditorGUI.LabelField(rect, "Hit Box Properties!");},
                onAddCallback = _ => PropertyListAdd(hitBoxList)
                
            };
            
            spriteList = new ReorderableList(serializedObject, propSpriteProperties,
                true, true, true, true){
                drawElementCallback = (rect, index, _, _) => DrawPropertyList(spriteList, rect, index),
                elementHeight = EditorGUIUtility.singleLineHeight,
                drawHeaderCallback = rect => {EditorGUI.LabelField(rect, "Sprite Properties!");},
                onAddCallback = _ => PropertyListAdd(spriteList)
            };
        }
        
        private static void DrawPropertyList(ReorderableList list, Rect rect, int index){
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.serializedObject.Update();
            rect.y += 2;
            DrawPropertyData(rect, element);
            element.serializedObject.ApplyModifiedProperties();
        }


        private static void DrawPropertyData(Rect rect, SerializedProperty element){
            element.serializedObject.Update();
            var nameProp = element.FindPropertyRelative("name");


            var nameRect = new Rect(rect.x, rect.y, 90, EditorGUIUtility.singleLineHeight); //Adjusted rect of property.


            EditorGUI.PropertyField(
                nameRect,
                nameProp,
                GUIContent.none
            ); //Draw name of property


            element.serializedObject.ApplyModifiedProperties();
            var dataType = element.FindPropertyRelative("dataType");

            var dataTypeRect = new Rect(nameRect.xMax + 20, nameRect.y, nameRect.width, nameRect.height);

            using (var check = new EditorGUI.ChangeCheckScope()) {
                
                EditorGUI.PropertyField(
                dataTypeRect,
                dataType,
                GUIContent.none
                );
                
                if (check.changed) {
                    element.FindPropertyRelative("guid").stringValue = GUID.Generate().ToString();
                }
            }
            PixelAnimatorUtility.CreateTooltip(nameRect, "Name", Event.current.mousePosition);
            PixelAnimatorUtility.CreateTooltip(dataTypeRect, "Data Type", Event.current.mousePosition);
        }
        
        private static void PropertyListAdd(ReorderableList list){
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize ++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("guid").stringValue = GUID.Generate().ToString();
            element.serializedObject.ApplyModifiedProperties();
        }
        

        
        #endregion 

        
    }
    
}



