using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI resultText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private Image sliderFill;

    private float currentCombo;
    private float maxCombo = 7.0f;
    private float comboDurationCountdown;

    private bool isGameOver;

    private int score;

    private bool isSelected;
    private GameObject firstItem;
    private GameObject secondItem;
    private int totalItems;
    private int clearItems;

    public void Start()
    {
        score = 0;
        isGameOver = false;

        currentCombo = 0.0f;
        slider.value = 0.0f;
        comboDurationCountdown = maxCombo;

        clearItems = 0;
        totalItems = GetComponent<Board>().GetTotalItems();
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;
        ComboCooldown();
        FillComboColor();
        CheckWin();
    }

    private void ComboCooldown()
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
    }

    private void FillComboColor()
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

    private void KeepCombo()
    {
        currentCombo = currentCombo + 1 > maxCombo ? maxCombo : currentCombo + 1;
        comboDurationCountdown = maxCombo - currentCombo > 3.0f ? maxCombo - currentCombo : 3.0f;
        slider.value = slider.maxValue;
    }

    private void ClearCombo()
    {
        currentCombo = 0;
        slider.value = 0.0f;
    }

    public void SetGameOver()
    {
        isGameOver = true;
        ResetItems();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SelectFirstItem(GameObject item)
    {
        if (!isGameOver)
        {
            firstItem = item;
            isSelected = true;
        }
    }

    public void SelectSecondItem(GameObject item)
    {
        secondItem = item;
        CompareTwoItems();
        ResetItems();
    }

    private void CompareTwoItems()
    {
        if (firstItem != null && secondItem != null)
        {
            Item firstItemComponent = firstItem.GetComponent<Item>();
            Item secondItemComponent = secondItem.GetComponent<Item>();

            if (firstItemComponent.value == secondItemComponent.value)
            {
                Destroy(firstItem);
                Destroy(secondItem);
                AddScore();
                KeepCombo();
                clearItems += 2;

                if (clearItems >= totalItems)
                {
                    isGameOver = true;
                }
            }
            else
            {
                firstItem.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.clear;
                ClearCombo();
            }
        }
    }

    private void ResetItems()
    {
        firstItem = null;
        secondItem = null;
        isSelected = false;
    }

    private void AddScore()
    {
        int point = 100;
        score = score + point + (int) currentCombo * 10;
    }

    private void CheckWin()
    {
        if (clearItems >= totalItems && isGameOver)
        {
            ResetItems();
            resultText.text = "Pro !!!";
        }
        else if (isGameOver)
        {
            resultText.text = "Press Restart to Play Again";
        }
    }
}