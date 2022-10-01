using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AnalyzePanelManager : MonoBehaviour
{
    [SerializeField] private float analyzeTime;
    [SerializeField] private float analyzeAnimationSpeed;
    [SerializeField] private Image bottomStick, upStick;
    [SerializeField] private TMPro.TMP_Text analyzePercentText;
    private TargetPoint analyzingTarget;
    private void Start()
    {
        analyzingTarget = MissionSystem.Instance.NearestTargetPoint;
        StartCoroutine(AnalyzingCoroutine());
    }
    IEnumerator AnalyzingCoroutine()
    {
        CanvasGroup textCanvasGroup = analyzePercentText.GetComponent<CanvasGroup>();
        textCanvasGroup.alpha = 0;
        bottomStick.fillAmount = 0;
        upStick.fillAmount = 0;
        float animationTime = 0;
        while (Mathf.Abs(animationTime - 2) > 0.01f)
        {
            animationTime = Mathf.Lerp(animationTime, 2, Time.deltaTime * analyzeAnimationSpeed);
            bottomStick.fillAmount = Mathf.Clamp(animationTime, 0, 1);
            upStick.fillAmount = Mathf.Clamp(animationTime - 1, 0, 1);
            yield return null;
        }
        bottomStick.fillAmount = 1;
        upStick.fillAmount = 1;
        while (Mathf.Abs(textCanvasGroup.alpha - 1) > 0.01f)
        {
            textCanvasGroup.alpha = Mathf.Lerp(textCanvasGroup.alpha, 1, Time.deltaTime * 5);
            yield return null;
        }
        textCanvasGroup.alpha = 1;

        for (int i = 100; i > 0; i--)
        {
            analyzePercentText.text = $"%{i}";
            yield return new WaitForSeconds(analyzeTime / 100);
        }
        while (Mathf.Abs(textCanvasGroup.alpha) > 0.01f)
        {
            textCanvasGroup.alpha = Mathf.Lerp(textCanvasGroup.alpha, 0, Time.deltaTime * 5);
            yield return null;
        }
        textCanvasGroup.alpha = 0;
        while (Mathf.Abs(animationTime) > 0.01f)
        {
            animationTime = Mathf.Lerp(animationTime, 0, Time.deltaTime * analyzeAnimationSpeed);
            bottomStick.fillAmount = Mathf.Clamp(animationTime, 0, 1);
            upStick.fillAmount = Mathf.Clamp(animationTime - 1, 0, 1);
            yield return null;
        }
        bottomStick.fillAmount = 0;
        upStick.fillAmount = 0;

        MissionSystem.Instance.CompleteTarget(analyzingTarget);

        Destroy(gameObject);
    }
}
