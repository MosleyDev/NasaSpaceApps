using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationManager : MonoBehaviour
{
    [SerializeField] private float menuShowSpeeds;
    [Header("DayCountMenu")]
    [SerializeField] private GameObject dayCountMenu;
    [SerializeField] private float dayCountCooldown;
    [Header("BatteryDecreaseMenu")]
    [SerializeField] private GameObject batteryDecreaseMenu;
    [SerializeField] private float batteryAnimationSpeed;
    [SerializeField] private float batteryAnimationCooldown;
    [SerializeField] private Image batteryCountImage;
    [SerializeField] private TMP_Text batteryCountText;
    [SerializeField] private Gradient batteryGradient;
    [Header("ChangeRoverMenu")]
    [SerializeField] private GameObject changeRoverMenu;
    private string pressedButton = string.Empty;


    private void Start() => StartCoroutine(InformationMenuCoroutine());
    IEnumerator InformationMenuCoroutine()
    {
        #region DayCountMenu
        CanvasGroup dayCanvasGroup = dayCountMenu.GetComponent<CanvasGroup>();
        dayCanvasGroup.alpha = 0;
        while (dayCanvasGroup.alpha != 1)
        {
            dayCanvasGroup.alpha = Mathf.MoveTowards(dayCanvasGroup.alpha, 1, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }
        yield return new WaitForSeconds(dayCountCooldown);
        while (dayCanvasGroup.alpha != 0)
        {
            dayCanvasGroup.alpha = Mathf.MoveTowards(dayCanvasGroup.alpha, 0, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }
        #endregion

        #region BatteryDecreaseMenu
        CanvasGroup batteryCanvasGroup = batteryDecreaseMenu.GetComponent<CanvasGroup>();
        batteryCanvasGroup.alpha = 0;

        float currentBatteryAmount = PlayerPrefs.GetFloat("Battery", 100f);
        float currentBatteryPercent = currentBatteryAmount / 100;
        batteryCountImage.color = batteryGradient.Evaluate(currentBatteryPercent);
        batteryCountImage.fillAmount = currentBatteryPercent;
        batteryCountText.text = $"%{(int)currentBatteryAmount}";

        while (batteryCanvasGroup.alpha != 1)
        {
            batteryCanvasGroup.alpha = Mathf.MoveTowards(batteryCanvasGroup.alpha, 1, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }

        float newBatteryAmount = currentBatteryAmount - 10; //pil verimliliğine göre azalacak olan kısım
        while (currentBatteryAmount != newBatteryAmount)
        {
            currentBatteryAmount = Mathf.MoveTowards(currentBatteryAmount, newBatteryAmount, Time.deltaTime * batteryAnimationSpeed);
            currentBatteryPercent = currentBatteryAmount / 100;
            batteryCountImage.color = batteryGradient.Evaluate(currentBatteryPercent);
            batteryCountImage.fillAmount = currentBatteryPercent;
            batteryCountText.text = $"%{(int)currentBatteryAmount}";
            yield return null;
        }
        yield return new WaitForSeconds(batteryAnimationCooldown);
        while (batteryCanvasGroup.alpha != 0)
        {
            batteryCanvasGroup.alpha = Mathf.MoveTowards(batteryCanvasGroup.alpha, 0, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }
        #endregion

        #region ChangeRoverMenu
        CanvasGroup changeRoverCanvasGroup = changeRoverMenu.GetComponent<CanvasGroup>();
        changeRoverCanvasGroup.alpha = 0;
        while (changeRoverCanvasGroup.alpha != 1)
        {
            changeRoverCanvasGroup.alpha = Mathf.MoveTowards(changeRoverCanvasGroup.alpha, 1, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }
        yield return new WaitUntil(() => pressedButton != string.Empty);
        while (changeRoverCanvasGroup.alpha != 0)
        {
            changeRoverCanvasGroup.alpha = Mathf.MoveTowards(changeRoverCanvasGroup.alpha, 0, Time.deltaTime * menuShowSpeeds);
            yield return null;
        }
        if (pressedButton == "Skip")
        {
            Debug.Log("oyun başlatıldı");
        }
        else if (pressedButton == "Change")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DecisionScene");
        }
        #endregion
    }
    public void OnClick_StringButton(string buttonName)
    {
        pressedButton = buttonName;
    }
}
