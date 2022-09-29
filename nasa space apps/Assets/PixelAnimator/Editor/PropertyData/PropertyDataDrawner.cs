using System;
using binc.PixelAnimator.PropertyDataProvider;
using UnityEditor;
using UnityEngine;
using binc.PixelAnimator.Utility;

namespace binc.PixelAnimator.Editor.PropertyDataProvider{


    
    [CustomPropertyDrawer(typeof(PropertyValue))]
    public class PropertyValueDrawer : PropertyDrawer{
        private SerializedProperty mType;
        private SerializedProperty mData;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            mType = property.FindPropertyRelative("dataType");
            mData = property.FindPropertyRelative("baseData");

            property.serializedObject.Update();

            var typePos = new Rect(position.x, position.y, 70, EditorGUIUtility.singleLineHeight);
            var dataPos = new Rect(position.x + 80, position.y, 80, 20);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(typePos, mType, GUIContent.none);
            if (EditorGUI.EndChangeCheck()) {
                mData.managedReferenceValue = PixelAnimatorUtility.CreateBlankBaseData((DataType)mType.intValue);
            }

            if (mData.managedReferenceValue != null) {
                EditorGUI.PropertyField(dataPos, mData, GUIContent.none);
            }
            else {
                mData.managedReferenceValue = PixelAnimatorUtility.CreateBlankBaseData(DataType.IntData);
            }
            property.serializedObject.ApplyModifiedProperties();
            
        }






        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(BaseData))]
    public class BaseDataPropertyDrawer : PropertyDrawer{
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Keyboard), GUIContent.none);
            var mData = property.FindPropertyRelative("data");
            var mGuid = property.FindPropertyRelative("guid");
            
            var widthSize = position.width / 2;
            
            
            var dataLabel = new Rect(position.x, position.y, 50, position.height);
            var dataRect = new Rect(position.x + dataLabel.width, position.y, widthSize * 2, position.height );

            var guidLabel = new Rect(dataRect.x + dataRect.width, position.y, 30, position.height);
            var guidRect = new Rect(guidLabel.x + guidLabel.width, position.y, widthSize * 2, position.height);
            
            EditorGUI.LabelField(dataLabel, "Data :");
            EditorGUI.PropertyField(dataRect, mData, GUIContent.none);
            EditorGUI.LabelField(guidLabel, "Guid :");
            EditorGUI.PropertyField(guidRect, mGuid, GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();
        }

    }
    
}


