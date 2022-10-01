using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MissionSystem : MonoBehaviour
{
    #region Instance
    public static MissionSystem Instance;
    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [Header("Spawn Target Points")]
    [SerializeField] private TargetPoint targetPointPrefab;
    [SerializeField] private int targetPointsCount;
    [SerializeField] private float spawnYPosition;
    [SerializeField] private Transform minSpawnPosition, maxSpawnPosition;
    [Header("Analyzing")]
    [SerializeField] private AnalyzePanelManager analyzePanelPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button startAnalyzingButton;
    [SerializeField] private TMPro.TMP_Text analyzedTargetsText;

    private TargetPoint nearestTargetPoint;
    public TargetPoint NearestTargetPoint
    {
        get => nearestTargetPoint;
        set
        {
            nearestTargetPoint = value;
        }
    }
    private TargetPoint[] allTargetPoints;
    private int completedTargetPointCount;
    public bool isAnalysing;
    private void Start()
    {
        GenerateTargetPoints();
        analyzedTargetsText.text = $"{completedTargetPointCount}/{targetPointsCount}";
    }
    private void Update()
    {
        startAnalyzingButton.gameObject.SetActive(nearestTargetPoint != null);
    }
    private void GenerateTargetPoints()
    {
        allTargetPoints = new TargetPoint[targetPointsCount];
        for (int i = 0; i < targetPointsCount; i++)
        {
            allTargetPoints[i] = Instantiate(
                targetPointPrefab,
                new Vector3(Random.Range(minSpawnPosition.position.x, maxSpawnPosition.position.x), spawnYPosition),
                Quaternion.identity
            );
        }
    }
    public void CompleteTarget(TargetPoint target)
    {
        target.isCompleted = true;
        completedTargetPointCount++;
        analyzedTargetsText.text = $"{completedTargetPointCount}/{targetPointsCount}";
        isAnalysing = false;

        if (completedTargetPointCount >= targetPointsCount)
        {
            //üsse geri dön
        }
    }
    public void OnClick_AnalyzingButton()
    {
        if (isAnalysing == true) return;

        isAnalysing = true;
        Instantiate(
            analyzePanelPrefab,
            Camera.main.WorldToScreenPoint(nearestTargetPoint.transform.position),
            Quaternion.identity,
            canvas.transform
        );
    }
}
