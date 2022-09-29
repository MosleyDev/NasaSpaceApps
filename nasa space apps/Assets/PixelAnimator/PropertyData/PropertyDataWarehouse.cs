using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;



namespace binc.PixelAnimator.PropertyDataProvider{

    
    public enum DataType{
        IntData,
        StringData,
        BoolData,
        FloatData,
        DoubleData,
        LongData,
        RectData,
        RectIntData,
        ColorData,
        AnimationCurveData,
        BoundsData,
        BoundsIntData,
        Vector2Data,
        Vector3Data,
        Vector4Data,
        Vector2INTData,
        Vector3INTData,
        UnityObjectData,
        GradientData,

    }

    
    [Serializable]
    public class BaseData{

        [ReadOnly, SerializeField]
        private string guid;
        
        public string Guid => guid;
        public BaseData(string guid){
            this.guid = guid;
        }
        public BaseData(){
        }

        public void SetGuid(string guid){
            this.guid = guid;
        }
        

    }
    
    [Serializable]
    public struct PropertyValue{
        [SerializeReference] public BaseData baseData;
        public DataType dataType;

        
    }
    
    #region DataClasses 
    
    [Serializable]
    public class IntData : BaseData{
        [SerializeField] private int data;
        public int Data => data;
        public IntData(string guid) : base(guid){
        }

        public IntData(){
            
        }
        
    }
    [Serializable]
    public class StringData : BaseData{
        [SerializeField] private string data;
        public string Data => data;
        public StringData(string guid) : base(guid){
        }

        public StringData(){
            
        }
        
    }

    [Serializable]
    public class BoolData : BaseData{
        [SerializeField] private bool data;
        public bool Data => data;
        public BoolData(string guid, string name) : base(guid){
        }

        public BoolData(){
            
        }
    }
    
    [Serializable]
    public class FloatData : BaseData{
        [SerializeField] private float data;
        public float Data => data;
        public FloatData(string guid, string name) : base(guid){
        }

        public FloatData(){
            
        }
    }
    
    
    [Serializable]
    public class DoubleData : BaseData{
        [SerializeField] private double data;
        public double Data => data;
        public DoubleData(string guid) : base(guid){
        }

        public DoubleData(){
            
        }
    }

    
    [Serializable]
    public class LongData : BaseData{
        [SerializeField] private long data;
        public long Data => data;
        public LongData(string guid) : base(guid){
        }

        public LongData(){
            
        }
    }
    
    public class RectData : BaseData{
        [SerializeField] private Rect data;
        public Rect Data => data;
        public RectData(string guid) : base(guid){
        }

        public RectData(){
            
        }
    }
    
    public class RectIntData : BaseData{
        [SerializeField] private RectInt data;
        public RectInt Data => data;
        public RectIntData(string guid) : base(guid){
        }

        public RectIntData(){
            
        }
    }
    
    [Serializable]
    public class ColorData : BaseData{
        [SerializeField] private Color data;
        public Color Data => data;
        public ColorData(string guid) : base(guid){
        }

        public ColorData(){
            
        }
    }
    
    [Serializable]
    public class AnimationCurveData : BaseData{
        [SerializeField] private AnimationCurve data;
        public AnimationCurve Data => data;
        public AnimationCurveData(string guid) : base(guid){
        }

        public AnimationCurveData(){
            
        }
    }
    
    [Serializable]
    public class BoundsData : BaseData{
        [SerializeField] private Bounds data;
        public Bounds Data => data;
        public BoundsData(string guid) : base(guid){
        }

        public BoundsData(){
            
        }
    }
    [Serializable]
    public class BoundsIntData : BaseData{
        [SerializeField] private BoundsInt data;
        public BoundsInt Data => data;
        public BoundsIntData(string guid) : base(guid){
        }

        public BoundsIntData(){
            
        }
    }
    
    
    [Serializable]
    public class Vector2Data : BaseData{
        [SerializeField] private Vector2 data;
        public Vector2 Data => data;
        public Vector2Data(string guid) : base(guid){
        }

        public Vector2Data(){
            
        }
    }
    
    [Serializable]
    public class Vector3Data : BaseData{
        [SerializeField] private Vector3 data;
        public Vector3 Data => data;
        public Vector3Data(string guid) : base(guid){
        }

        public Vector3Data(){
            
        }
    }
    
    [Serializable]
    public class Vector4Data : BaseData{
        [SerializeField] private Vector4 data;
        public Vector4 Data => data;
        public Vector4Data(string guid) : base(guid){
        }

        public Vector4Data(){
            
        }
    }
    
    
    [Serializable]
    public class Vector2IntData : BaseData{
        [SerializeField] private Vector2Int data;
        public Vector2Int Data => data;
        public Vector2IntData(string guid) : base(guid){
        }

        public Vector2IntData(){
            
        }
    }
    
    [Serializable]
    public class Vector3IntData : BaseData{
        [SerializeField] private Vector3Int data;
        public Vector3Int Data => data;
        public Vector3IntData(string guid) : base(guid){
        }

        public Vector3IntData(){
            
        }
    }
    
    [Serializable]
    public class UnityObjectData : BaseData{
        [SerializeField] private Object data;
        public Object Data => data;
        public UnityObjectData(string guid) : base(guid){
        }

        public UnityObjectData(){
            
        }
    }
    
    [Serializable]
    public class GradientData : BaseData{
        [SerializeField] private Gradient data;
        public Gradient Data => data;
        public GradientData(string guid) : base(guid){
        }

        public GradientData(){
            
        }
    }
    #endregion
    
    [Serializable]
    public class PropertyDataWarehouse{
        [SerializeField] private string name;
        public string Name => name;
        public DataType dataType;
        [ReadOnly, SerializeField] private string guid;

        public string Guid => guid;

        public PropertyDataWarehouse(string name, string guid){
            this.name = name;
            this.guid = guid;
        }
        
        public PropertyDataWarehouse(string guid){
            this.guid = guid;
        }

        public void SetName(string name){
            this.name = name;
        }
        
        
    }
    
    [Serializable]
    public class PropertyData{
        public List<PropertyValue> propertyValues;
        public List<string> eventNames;

        public PropertyData(){
            propertyValues = new List<PropertyValue>();
            eventNames = new List<string>();
        }
    }
    
    

}