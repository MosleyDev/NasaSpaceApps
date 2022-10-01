using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoverMissionManager : MonoBehaviour
{
    [SerializeField] private float requiredDistance;
    [SerializeField] private LayerMask targetPointsLayer;
    private void Update()
    {
        MissionSystem.Instance.NearestTargetPoint = GetClosestTargetPoint();
    }
    private TargetPoint GetClosestTargetPoint()
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(transform.position, requiredDistance, targetPointsLayer);
        foreach (var targetCol in targetCols)
        {
            var targetPoint = targetCol.GetComponent<TargetPoint>();
            return targetPoint;
        }
        return null;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, requiredDistance);
    }
}
