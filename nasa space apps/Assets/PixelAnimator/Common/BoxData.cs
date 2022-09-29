using UnityEngine;

namespace binc.PixelAnimator.Common{

    [System.Serializable]
    public struct BoxData{
        public Color color;
        
        public string boxType;
        [Tooltip("LayerMask Index")]
        public int activeLayer;
        public PhysicsMaterial2D physicMaterial;
        public bool rounded;
        public int collisionLayer;
        public enum ColliderDetection{Enter, Stay, Exit}
        public ColliderDetection colliderDetection;
        
        [ReadOnly, SerializeField]
        private string guid;

        public string Guid => guid;

    }



}