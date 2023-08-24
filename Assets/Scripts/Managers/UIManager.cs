using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject winPanel;
    public TextMeshProUGUI levelText, moneyText, earnedMoneyText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoneyText(CurrencyManager.Instance.GetCurrentMoney());
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateMoneyText(int amount)
    {
        moneyText.text = amount.ToString();
    }

    public void UpdateEarnedText()
    {
        int amount = Random.Range(20, 70);
        earnedMoneyText.text = "EARNED " + amount.ToString();
        CurrencyManager.Instance.AddMoney(amount);
        UpdateMoneyText(CurrencyManager.Instance.GetCurrentMoney());
    }

    public void OpenWinPanel()
    {
        winPanel.SetActive(true);
        UpdateEarnedText();
    }
}
