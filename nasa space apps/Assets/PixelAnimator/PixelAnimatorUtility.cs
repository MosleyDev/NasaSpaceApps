using System;
using System.Runtime.Serialization;
using binc.PixelAnimator.Common;
using binc.PixelAnimator.PropertyDataProvider;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace binc.PixelAnimator.Utility{

    
    public static class PixelAnimatorUtility{
        private static float BoxSize{get; set;} = 8.5f;
        private static Vector2 _clickedMousePos;


#if UNITY_EDITOR
        // public static void DrawBox(Layer layer, BoxData group, int index, Vector2 matrixScale){
        //
        //     var eventCurrent = Event.current;
        //     var rect = layer.frames[index].hitBoxRect;
        //     var frame = layer.frames[index];
        //     var rectColor = group.color;
        //     var rounded = group.rounded;
        //     
        //     var xScale = matrixScale.x;
        //
        //
        //     var size = BoxSize/xScale;
        //     var rTopLeft = new Rect(rect.xMin - size/2, rect.yMin - size/2, size, size);
        //     var rTopCenter = new Rect((rect.xMin + rect.width/2) - size/2, rect.yMin - size/2, size, size);
        //     var rTopRight = new Rect(rect.xMax - size/2, rect.yMin - size/2, size, size);
        //     var rRightCenter = new Rect(rect.xMax - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
        //     var rBottomRight = new Rect(rect.xMax - size/2, rect.yMax - size/2, size, size);
        //     var rBottomCenter = new Rect((rect.xMin + rect.width/2) - size/2, rect.yMax - size/2, size, size);
        //     var rBottomLeft = new Rect(rect.xMin - size/2, rect.yMax - size/2, size, size);
        //     var rLeftCenter = new Rect(rect.xMin - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
        //
        //     var adjustedMiddleRect = new Rect(rect.x + size/2, rect.y + size/2, rect.width - size, rect.height - size);
        //     if(layer.activeBox) {
        //         
        //         if(eventCurrent.button == 0 && eventCurrent.type == EventType.MouseDown){
        //             if(rTopLeft.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopLeft = true;
        //             else if(rTopCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopCenter = true;
        //             else if(rTopRight.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopRight = true;
        //             else if(rRightCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleRightCenter = true;
        //             else if(rBottomRight.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomRight = true;
        //             else if(rBottomCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomCenter = true;
        //             else if(rBottomLeft.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomLeft = true;
        //             else if(rLeftCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleLeftCenter = true;
        //             else if(adjustedMiddleRect.Contains(eventCurrent.mousePosition)){
        //                 layer.onMiddle = true;
        //                 _clickedMousePos = eventCurrent.mousePosition;
        //             }
        //         }
        //
        //
        //
        //         if(layer.onHandleTopLeft){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x;
        //             frame.hitBoxRect.yMin = (int)eventCurrent.mousePosition.y;
        //         }
        //         else if(layer.onHandleTopCenter){
        //             frame.hitBoxRect.yMin = (int)eventCurrent.mousePosition.y;
        //         }
        //         else if(layer.onHandleTopRight){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x;
        //             frame.hitBoxRect.yMin = (int)eventCurrent.mousePosition.y;
        //         }
        //         else if(layer.onHandleRightCenter){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x;
        //
        //         }
        //         else if(layer.onHandleBottomRight){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x;
        //             frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y;
        //             
        //
        //         }
        //         else if(layer.onHandleBottomCenter){
        //             frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y;
        //         }
        //         else if(layer.onHandleBottomLeft){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x;
        //             frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y;
        //             
        //         }
        //         else if(layer.onHandleLeftCenter){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x;
        //             
        //         }
        //         else if(layer.onMiddle){
        //
        //             var deltaX = _clickedMousePos.x - rect.xMin;
        //             var deltaY = _clickedMousePos.y - rect.yMin;
        //
        //             var adjustedPos =  new Vector2(eventCurrent.mousePosition.x - deltaX, eventCurrent.mousePosition.y - deltaY);
        //             frame.hitBoxRect.position = adjustedPos;
        //
        //
        //             frame.hitBoxRect.position = new Vector2((int)eventCurrent.mousePosition.x - (int)deltaX, (int)eventCurrent.mousePosition.y - (int)deltaY);    
        //             frame.hitBoxRect.size = new Vector2((int)rect.size.x, (int)rect.size.y);          
        //         
        //
        //             _clickedMousePos = eventCurrent.mousePosition;
        //
        //         }
        //
        //
        //         EditorGUI.DrawRect( rTopLeft , rectColor );
        //         EditorGUI.DrawRect( rTopCenter , rectColor );
        //         EditorGUI.DrawRect( rTopRight , rectColor );
        //         EditorGUI.DrawRect( rRightCenter , rectColor );
        //         EditorGUI.DrawRect( rBottomRight , rectColor );
        //         EditorGUI.DrawRect( rBottomCenter , rectColor );
        //         EditorGUI.DrawRect( rBottomLeft , rectColor );
        //         EditorGUI.DrawRect( rLeftCenter , rectColor );
        //
        //     }
        //
        //     if(eventCurrent.type == EventType.MouseUp){
        //         layer.onHandleTopLeft = false;
        //         layer.onHandleTopCenter = false;
        //         layer.onHandleTopRight = false;
        //         layer.onHandleRightCenter = false;
        //         layer.onHandleBottomRight = false;
        //         layer.onHandleBottomCenter = false;
        //         layer.onHandleBottomLeft = false;
        //         layer.onHandleLeftCenter = false;
        //         layer.onMiddle = false;
        //     }
        //
        //
        //     var color = layer.activeBox ? new Color(rectColor.r, rectColor.g, rectColor.b, 0.2f) : Color.clear;    
        //
        //     Handles.DrawSolidRectangleWithOutline( rect, color, rectColor );
        //
        //
        //
        // }
        //
        // public static void DrawBox(Layer layer, BoxData group, int index, int scale, Vector2 spriteSize){
        //
        //     var eventCurrent = Event.current;
        //     var frame = layer.frames[index];
        //     var rectColor = group.color;
        //
        //     var rect = frame.hitBoxRect;
        //     
        //     rect.position *=  scale;
        //     rect.size *= scale;
        //
        //
        //     var size = BoxSize;
        //     var rTopLeft = new Rect(rect.xMin - size/2, rect.yMin - size/2, size, size);
        //     var rTopCenter = new Rect((rect.xMin + rect.width/2) - size/2, rect.yMin - size/2, size, size);
        //     var rTopRight = new Rect(rect.xMax - size/2, rect.yMin - size/2, size, size);
        //     var rRightCenter = new Rect(rect.xMax - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
        //     var rBottomRight = new Rect(rect.xMax - size/2, rect.yMax - size/2, size, size);
        //     var rBottomCenter = new Rect((rect.xMin + rect.width/2) - size/2, rect.yMax - size/2, size, size);
        //     var rBottomLeft = new Rect(rect.xMin - size/2, rect.yMax - size/2, size, size);
        //     var rLeftCenter = new Rect(rect.xMin - size/2, rect.yMin + (rect.yMax - rect.yMin)/2 - size/2, size, size);
        //
        //     var rAdjustedMiddle = new Rect(rect.x + size/2, rect.y + size/2, rect.width - size, rect.height - size);
        //     if(layer.activeBox) {
        //         
        //         if(eventCurrent.button == 0 && eventCurrent.type == EventType.MouseDown){
        //             if(rTopLeft.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopLeft = true;
        //             else if(rTopCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopCenter = true;
        //             else if(rTopRight.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleTopRight = true;
        //             else if(rRightCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleRightCenter = true;
        //             else if(rBottomRight.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomRight = true;
        //             else if(rBottomCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomCenter = true;
        //             else if(rBottomLeft.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleBottomLeft = true;
        //             else if(rLeftCenter.Contains(eventCurrent.mousePosition))
        //                 layer.onHandleLeftCenter = true;
        //             else if(rAdjustedMiddle.Contains(eventCurrent.mousePosition)){
        //                 layer.onMiddle = true;
        //                 _clickedMousePos = eventCurrent.mousePosition;
        //             }
        //         }
        //
        //
        //
        //         if(layer.onHandleTopLeft){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
        //             frame.hitBoxRect.yMin = (int)eventCurrent.mousePosition.y/scale;
        //         }
        //         else if(layer.onHandleTopCenter){
        //             frame.hitBoxRect.yMin  = (int)eventCurrent.mousePosition.y/scale;
        //         }
        //         else if(layer.onHandleTopRight){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
        //             frame.hitBoxRect.yMin  = (int)eventCurrent.mousePosition.y/scale;
        //         }
        //         else if(layer.onHandleRightCenter){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
        //         
        //         }
        //         else if(layer.onHandleBottomRight){
        //             frame.hitBoxRect.xMax = (int)eventCurrent.mousePosition.x/scale;
        //             frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y/scale;
        //             
        //         
        //         }
        //         else if(layer.onHandleBottomCenter){
        //             frame.hitBoxRect.yMax  = (int)eventCurrent.mousePosition.y/scale;
        //         }
        //         else if(layer.onHandleBottomLeft){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
        //             frame.hitBoxRect.yMax = (int)eventCurrent.mousePosition.y/scale;
        //             
        //         }
        //         else if(layer.onHandleLeftCenter){
        //             frame.hitBoxRect.xMin = (int)eventCurrent.mousePosition.x/scale;
        //             
        //         }
        //         else if(layer.onMiddle){
        //         
        //             var deltaX = (_clickedMousePos.x - rect.xMin)/scale;
        //             var deltaY = (_clickedMousePos.y - rect.yMin)/scale;
        //             
        //
        //             frame.hitBoxRect.position = new Vector2((int)eventCurrent.mousePosition.x/scale - (int)deltaX, (int)eventCurrent.mousePosition.y/scale - (int)deltaY);    
        //             frame.hitBoxRect.size = new Vector2((int)rect.size.x/scale, (int)rect.size.y/scale);          
        //             
        //             _clickedMousePos = eventCurrent.mousePosition;
        //         
        //         }
        //
        //
        //         EditorGUI.DrawRect( rTopLeft , rectColor );
        //         EditorGUI.DrawRect( rTopCenter , rectColor );
        //         EditorGUI.DrawRect( rTopRight , rectColor );
        //         EditorGUI.DrawRect( rRightCenter , rectColor );
        //         EditorGUI.DrawRect( rBottomRight , rectColor );
        //         EditorGUI.DrawRect( rBottomCenter , rectColor );
        //         EditorGUI.DrawRect( rBottomLeft , rectColor );
        //         EditorGUI.DrawRect( rLeftCenter , rectColor );
        //
        //     }
        //
        //     if(eventCurrent.type == EventType.MouseUp){
        //         layer.onHandleTopLeft = false;
        //         layer.onHandleTopCenter = false;
        //         layer.onHandleTopRight = false;
        //         layer.onHandleRightCenter = false;
        //         layer.onHandleBottomRight = false;
        //         layer.onHandleBottomCenter = false;
        //         layer.onHandleBottomLeft = false;
        //         layer.onHandleLeftCenter = false;
        //         layer.onMiddle = false;
        //     }
        //
        //     EditorGUIUtility.AddCursorRect(rTopLeft, MouseCursor.ResizeUpLeft);
        //     EditorGUIUtility.AddCursorRect(rTopCenter, MouseCursor.ResizeVertical);
        //     EditorGUIUtility.AddCursorRect(rTopRight, MouseCursor.ResizeUpRight);
        //     EditorGUIUtility.AddCursorRect(rRightCenter, MouseCursor.ResizeHorizontal);
        //     EditorGUIUtility.AddCursorRect(rBottomRight, MouseCursor.ResizeUpLeft);
        //     EditorGUIUtility.AddCursorRect(rBottomCenter, MouseCursor.ResizeVertical);
        //     EditorGUIUtility.AddCursorRect(rBottomLeft, MouseCursor.ResizeUpRight);
        //     EditorGUIUtility.AddCursorRect(rLeftCenter, MouseCursor.ResizeHorizontal);
        //     EditorGUIUtility.AddCursorRect(rAdjustedMiddle, MouseCursor.MoveArrow);
        //
        //     rect.width = Mathf.Clamp(rect.width, 0, int.MaxValue);
        //     rect.height = Mathf.Clamp(rect.height, 0, int.MaxValue);
        //
        //     frame.hitBoxRect.x = Mathf.Clamp(frame.hitBoxRect.x, 0, spriteSize.x);
        //     frame.hitBoxRect.y = Mathf.Clamp(frame.hitBoxRect.y, 0, spriteSize.y);
        //     frame.hitBoxRect.width = Mathf.Clamp(frame.hitBoxRect.width, 0, spriteSize.x - frame.hitBoxRect.x);
        //     frame.hitBoxRect.height = Mathf.Clamp(frame.hitBoxRect.height, 0, spriteSize.y - frame.hitBoxRect.y);
        //
        //     var color = layer.activeBox ? new Color(rectColor.r, rectColor.g, rectColor.b, 0.2f) : Color.clear;
        //     Handles.DrawSolidRectangleWithOutline( rect, color, rectColor );
        //     
        // }
        //
        public static bool Button(Texture2D image, Rect rect){
            var e = Event.current;
            GUI.DrawTexture(rect, image);
            if (!rect.Contains(e.mousePosition)) return false;
            EditorGUI.DrawRect(rect, new Color(255, 255, 255, 0.2f)); 
            return e.button == 0 && e.type == EventType.MouseDown;
        }

        public static bool Button(Rect rect, Color color){
            var e = Event.current;
            EditorGUI.DrawRect(rect, color);
            if(rect.Contains(e.mousePosition)){
                EditorGUI.DrawRect(rect, new Color(255, 255, 255, 0.2f)); 
                if(e.button == 0 && e.type == EventType.MouseDown){
                    return true;
                }
            }
            return false;
        }

        public static bool Button(Texture2D defaultImg, Texture2D onMouse, Rect rect){
            var e = Event.current;
            if(rect.Contains(e.mousePosition)){
                GUI.DrawTexture(rect, onMouse);
                EditorGUI.DrawRect(rect, new Color(255, 255, 255, 0.2f)); 
                if(e.button == 0 && e.type == EventType.MouseDown){
                    return true;
                }
            }else{
                GUI.DrawTexture(rect, defaultImg);

            }
            return false;
        }


        public static void CreateTooltip(Rect rect, string tooltip, Vector2 containsPosition){
            if (rect.Contains(containsPosition)) {
                EditorGUI.LabelField(rect,
                    new GUIContent("", tooltip));
            }
        }
        

        public static void SystemObjectPreviewField(object obj, params GUILayoutOption[] guiLayoutOptions){
            using (new EditorGUI.DisabledGroupScope(true)) {
                const float width = 120;
                switch (obj) {
                    case int data:
                        EditorGUILayout.IntField(data, guiLayoutOptions);
                        break;
                    case string data:
                        EditorGUILayout.TextField(data, guiLayoutOptions);
                        break;
                    case bool data:
                        EditorGUILayout.Toggle(data, guiLayoutOptions);
                        break;
                    case float data:
                        EditorGUILayout.FloatField(data, guiLayoutOptions);
                        break;
                    case long data:
                        EditorGUILayout.LongField(data, guiLayoutOptions);
                        break;
                    case double data:
                        EditorGUILayout.DoubleField(data, guiLayoutOptions);
                        break;
                    case Rect data:
                        EditorGUILayout.RectField(data, guiLayoutOptions);
                        break;
                    case RectInt data:
                        EditorGUILayout.RectIntField(data, guiLayoutOptions);
                        break;
                    case Color data:
                        EditorGUILayout.ColorField(data, guiLayoutOptions);
                        break;
                    case AnimationCurve data:
                        EditorGUILayout.CurveField(data, guiLayoutOptions);
                        break;
                    case Bounds data:
                        EditorGUILayout.BoundsField(data, guiLayoutOptions);
                        break;
                    case BoundsInt data:
                        EditorGUILayout.BoundsIntField(data, guiLayoutOptions);
                        break;
                    case Vector2 data:
                        EditorGUILayout.Vector2Field(GUIContent.none, data, guiLayoutOptions);
                        break;
                    case Vector3 data:
                        EditorGUILayout.Vector3Field(GUIContent.none, data, guiLayoutOptions);
                        break;
                    case Vector4 data:
                        EditorGUILayout.Vector4Field(GUIContent.none, data, guiLayoutOptions);
                        break;
                    case Vector2Int data:
                        EditorGUILayout.Vector2IntField(GUIContent.none, data, guiLayoutOptions);
                        break;
                    case Vector3Int data:
                        EditorGUILayout.Vector3IntField(GUIContent.none, data, guiLayoutOptions);
                        break;
                    case UnityEngine.Object data:
                        EditorGUILayout.ObjectField(data, obj.GetType(), false, guiLayoutOptions);
                        break;
                    case Gradient data:
                        EditorGUILayout.GradientField(data, guiLayoutOptions);
                        break;
                    default:
                        Debug.LogWarning("The entered object cannot be converted to any class. " + $"{((obj))}");
                        break;
                }
                
            }
        }
        
        

#endif
        private static object CreateObject(Type type){
            object obj;
            if (type.IsValueType) {
                obj = Activator.CreateInstance(type);
                return obj;
            }
            obj = FormatterServices.GetUninitializedObject(type);
            obj = Convert.ChangeType(obj, type);
            return obj;

        }

        public static DataType ToDataType(Type type){
            var obj = CreateObject(type);
            return obj switch{
                int => DataType.IntData,
                string => DataType.StringData,
                bool => DataType.BoolData,
                float => DataType.FloatData,
                double => DataType.DoubleData,
                long => DataType.LongData,
                Rect => DataType.RectData,
                RectInt => DataType.RectIntData,
                Color => DataType.ColorData,
                AnimationCurve => DataType.AnimationCurveData,
                Bounds => DataType.BoundsData,
                BoundsInt => DataType.BoundsIntData,
                Vector2 => DataType.Vector2Data,
                Vector3 => DataType.Vector3Data,
                Vector4 => DataType.Vector4Data,
                Vector2Int => DataType.Vector2INTData,
                Vector3Int => DataType.Vector3INTData,
                UnityEngine.Object => DataType.UnityObjectData,
                Gradient => DataType.GradientData,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static object DataTypeToSystemObject(DataType type){
            return type switch{
                DataType.IntData => default(int),
                DataType.StringData => "",
                DataType.BoolData => default(bool),
                DataType.FloatData => default(float),
                DataType.DoubleData => default(double),
                DataType.LongData => default(long),
                DataType.RectData => default(Rect),
                DataType.RectIntData => default(RectInt),
                DataType.ColorData => default(Color),
                DataType.AnimationCurveData => new AnimationCurve(),
                DataType.BoundsData => default(Bounds),
                DataType.BoundsIntData => default(BoundsInt),
                DataType.Vector2Data => default(Vector2),
                DataType.Vector3Data => default(Vector3),
                DataType.Vector4Data => default(Vector4),
                DataType.Vector2INTData => default(Vector2Int),
                DataType.Vector3INTData => default(Vector3Int),
                DataType.UnityObjectData => new UnityEngine.Object(),
                DataType.GradientData => new Gradient(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        
        public static BaseData CreateBlankBaseData(DataType type){
            return type switch{
                DataType.IntData => new IntData(),
                DataType.StringData => new StringData(),
                DataType.BoolData => new BoolData(),
                DataType.FloatData => new FloatData(),
                DataType.DoubleData => new DoubleData(),
                DataType.LongData => new LongData(),
                DataType.RectData => new RectData(),
                DataType.RectIntData => new RectIntData(),
                DataType.ColorData => new ColorData(),
                DataType.AnimationCurveData => new AnimationCurveData(),
                DataType.BoundsData => new BoundsData(),
                DataType.BoundsIntData => new BoundsIntData(),
                DataType.Vector2Data => new Vector2Data(),
                DataType.Vector3Data => new Vector3Data(),
                DataType.Vector4Data => new Vector4Data(),
                DataType.Vector2INTData => new Vector2IntData(),
                DataType.Vector3INTData => new Vector3Data(),
                DataType.UnityObjectData => new UnityObjectData(),
                DataType.GradientData => new GradientData(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }


        public static object GetData(this BaseData baseData){
            return baseData switch{
                AnimationCurveData animationCurveData => animationCurveData.Data,
                BoolData boolData => boolData.Data,
                BoundsData boundsData => boundsData.Data,
                BoundsIntData boundsIntData => boundsIntData.Data,
                ColorData colorData => colorData.Data,
                DoubleData doubleData => doubleData.Data,
                FloatData floatData => floatData.Data,
                GradientData gradientData => gradientData.Data,
                IntData intData => intData.Data,
                LongData longData => longData.Data,
                RectData rectData => rectData.Data,
                RectIntData rectIntData => rectIntData.Data,
                StringData stringData => stringData.Data,
                UnityObjectData unityObjectData => unityObjectData.Data,
                Vector2Data vector2Data => vector2Data.Data,
                Vector2IntData vector2IntData => vector2IntData.Data,
                Vector3Data vector3Data => vector3Data.Data,
                Vector3IntData vector3IntData => vector3IntData.Data,
                Vector4Data vector4Data => vector4Data.Data,
                _ => throw new ArgumentOutOfRangeException(nameof(baseData))
                
            };
        }
        
        


    }

 
}


