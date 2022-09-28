using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DecisionManager : MonoBehaviour
{
    public Toggle[] options => GetComponentsInChildren<Toggle>();
    public void OnToggle_Option(bool isToggle)
    {
        foreach (var option in options)
        {
            option.GetComponent<Outline>().enabled = option.isOn;
        }
    }
}
