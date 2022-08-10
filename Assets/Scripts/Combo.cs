using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{   
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Image sliderFill;

    private float maxCombo = 7.0f;

    private float currentCombo;

    private float comboDurationCountdown;

    public void Start()
    {
        currentCombo = 0.0f;
        comboDurationCountdown = maxCombo;
        slider.value = 0.0f;
    }

    public void Update()
    {   
        if (comboDurationCountdown > 0)
        {
            comboDurationCountdown = comboDurationCountdown - Time.deltaTime > 0 ? comboDurationCountdown - Time.deltaTime : 0.0f;
        }

        if (comboDurationCountdown == 0.0f)
        {
            currentCombo = 0;
            slider.value = 0.0f;
        }
        else
        {
            float duration = maxCombo - currentCombo > 3.0f ? maxCombo - currentCombo : 3.0f;
            slider.value = (comboDurationCountdown / duration) * slider.maxValue;
        }

        FillColor();
    }

    private void FillColor()
    {
        switch(currentCombo) 
        {
            case 0.0f:
                sliderFill.color = Color.clear;
                break;
            case 1.0f:
                sliderFill.color = new Color(148f/255f, 0f, 211f/255f);
                break;
            case 2.0f:
                sliderFill.color = new Color(75f/255f, 0f, 130f/255f);
                break;
            case 3.0f:
                sliderFill.color = new Color(0f, 0f, 1f);
                break;
            case 4.0f:
                sliderFill.color = new Color(0f, 1f, 0f);
                break;
            case 5.0f:
                sliderFill.color = new Color(1f, 1f, 0f);
                break;
            case 6.0f:
                sliderFill.color = new Color(1f, 127/255f, 0f);
                break;
            default:
                sliderFill.color = new Color(1f, 0f, 0f);
                break;
        }
    }

    public void KeepCombo()
    {
        currentCombo = currentCombo + 1 > maxCombo ? maxCombo : currentCombo + 1;
        comboDurationCountdown = maxCombo - currentCombo > 3.0f ? maxCombo - currentCombo : 3.0f;
        slider.value = slider.maxValue;
    }

    public int GetIntCombo()
    {
        return (int) currentCombo;
    }
}