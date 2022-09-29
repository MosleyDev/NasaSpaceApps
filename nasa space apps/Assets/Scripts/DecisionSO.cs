using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "Decision")]
public class DecisionSO : ScriptableObject
{
    public string partName;
    public Part[] partOptions = new Part[3];


    [System.Serializable]
    public struct Part
    {
        public string partName;
        [Range(0, 10)] public int partValue;
    }
}
