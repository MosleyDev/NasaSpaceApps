using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DecisionManager : MonoBehaviour
{
    [SerializeField] private TMP_Text partNameText;
    [SerializeField] private TMP_Text decisionNumberText;
    [SerializeField] private Button nextButton, backButton;
    [SerializeField] private DecisionSO[] decisions;
    public Toggle[] Options => GetComponentsInChildren<Toggle>();
    private int currentDecisionIndex;
    private Toggle selectedOption;
    private void Start()
    {
        LoadDecision(decisions[0]);
    }
    public void LoadDecision(DecisionSO decision)
    {
        currentDecisionIndex = decisions.ToList().FindIndex(x => x == decision);

        partNameText.text = decision.partName;
        decisionNumberText.text = $"({currentDecisionIndex + 1}/{decisions.Length})";
        backButton.gameObject.SetActive(currentDecisionIndex != 0);
        TMP_Text nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
        nextButtonText.text = currentDecisionIndex < decisions.Length - 1 ? "Next" : "Finish";

        string selectedOption = PlayerPrefs.GetString(
            decision.partName,
            decision.partOptions[0].partName
        );
        for (int i = 0; i < Options.Length; i++)
        {
            Options[i].GetComponentInChildren<TMP_Text>().text = decision.partOptions[i].partName;
            if (decision.partOptions[i].partName == selectedOption)
            {
                Options[i].isOn = true;
            }
        }
    }
    public void OnToggle_Option(bool isToggle)
    {
        foreach (var option in Options)
        {
            option.GetComponent<Outline>().enabled = option.isOn;
            if (option.isOn == true)
            {
                selectedOption = option;
            }
        }
    }
    public void OnClick_Next()
    {
        PlayerPrefs.SetString(
            decisions[currentDecisionIndex].partName,
            selectedOption.GetComponentInChildren<TMP_Text>().text
        );

        if (currentDecisionIndex != decisions.Length - 1)
        {
            LoadDecision(decisions[currentDecisionIndex + 1]);
        }
        else
        {
            //testi bitir
        }
    }
    public void OnClick_Back()
    {
        PlayerPrefs.SetString(
            decisions[currentDecisionIndex].partName,
            selectedOption.GetComponentInChildren<TMP_Text>().text
        );

        LoadDecision(decisions[currentDecisionIndex - 1]);
    }
}
