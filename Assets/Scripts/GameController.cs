using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip startGameSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] public GameObject resultPanel;
    [SerializeField] public GameObject board;

    private float currentCombo;
    private float maxCombo = 7.0f;
    private float comboDurationCountdown;
    private float playerMaxCombo;

    private bool isGameOver;
    private bool isPause;

    private int score;
    private int level;

    private bool isSelected;
    private GameObject firstItem;
    private GameObject secondItem;
    private int totalItems;
    private int clearItems;

    public void Start()
    {
        level = PlayerPrefs.GetInt("level");
        score = 0;
        isGameOver = false;
        isPause = false;

        currentCombo = 0.0f;
        playerMaxCombo = 0.0f;
        slider.value = 0.0f;
        comboDurationCountdown = maxCombo;

        clearItems = 0;
        totalItems = GetComponent<Board>().GetTotalItems();

        PlayStartSound();
    }

    private void Update()
    {
        if (!isPause && !isGameOver)
        {
            levelText.text = "Level: " + level;
            scoreText.text = "Score\n" + score;
            ComboCooldown();
            ComboColor();
        }
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

    private void ComboColor()
    {
        var color = Color.clear;
        switch (currentCombo)
        {
            case 0.0f:
                color = Color.clear;
                break;
            case 1.0f:
                color = new Color(148f / 255f, 0f, 211f / 255f);
                break;
            case 2.0f:
                color = new Color(75f / 255f, 0f, 130f / 255f);
                break;
            case 3.0f:
                color = new Color(0f, 0f, 1f);
                break;
            case 4.0f:
                color = new Color(0f, 1f, 0f);
                break;
            case 5.0f:
                color = new Color(1f, 1f, 0f);
                break;
            case 6.0f:
                color = new Color(1f, 127 / 255f, 0f);
                break;
            default:
                color = new Color(1f, 0f, 0f);
                break;
        }

        comboText.text = currentCombo != 0 ? "x" + (int)currentCombo : "";
        sliderFill.color = color;
    }

    private void KeepCombo()
    {
        currentCombo = currentCombo + 1;
        playerMaxCombo = playerMaxCombo < currentCombo ? currentCombo : playerMaxCombo;
        comboDurationCountdown = maxCombo - currentCombo > 3.0f ? maxCombo - currentCombo : 3.0f;
        slider.value = slider.maxValue;
    }

    private void ClearCombo()
    {
        currentCombo = 0;
        slider.value = 0.0f;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public GameObject GetFirstItem()
    {
        return firstItem;
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

    public void PlayStartSound()
    {
        audioSource.PlayOneShot(startGameSound);
    }

    private void CompareTwoItems()
    {
        if (firstItem != null && secondItem != null)
        {
            Item firstItemComponent = firstItem.GetComponent<Item>();
            Item secondItemComponent = secondItem.GetComponent<Item>();
            bool isConnection = GetComponent<Board>().CheckConnection(firstItemComponent.row, firstItemComponent.column, secondItemComponent.row, secondItemComponent.column);

            if (firstItemComponent.value == secondItemComponent.value && isConnection)
            {
                GetComponent<Board>().Clear(firstItemComponent.row, firstItemComponent.column, secondItemComponent.row, secondItemComponent.column);
                Destroy(firstItem);
                Destroy(secondItem);
                AddScore();
                KeepCombo();
                clearItems += 2;
                audioSource.PlayOneShot(correctSound);

                if (clearItems >= totalItems)
                {
                    isGameOver = true;
                    audioSource.PlayOneShot(winSound);
                    ClearCombo();
                }
            }
            else
            {
                ClearCombo();
                firstItem.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                secondItem.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                score -= 20;
                audioSource.PlayOneShot(incorrectSound);
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
        score = score + point + (int)currentCombo * 10;
    }

    public void MinusChangeScore(int totalChanges)
    {
        score -= 20 * totalChanges;
    }

    public void SetGameOver()
    {
        isGameOver = true;
        audioSource.PlayOneShot(gameOverSound);
        ResetItems();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPause()
    {
        return isPause;
    }

    public void SetPause(bool pause)
    {
        isPause = pause;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    private void CheckWin()
    {
        if (isGameOver)
        {
            board.SetActive(false);
            float remainTime = GetComponent<RoundCountdown>().GetRemainTime();
            resultPanel.GetComponent<ResultPanel>().Show(clearItems >= totalItems, this.score, this.playerMaxCombo, remainTime);
        }
    }
}
