using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    int currentMoney;
    const string CURRENT_MONEY = "CurrentMoney";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentMoney = GetCurrentMoney();
    }

    public int GetCurrentMoney()
    {
        return PlayerPrefs.GetInt(CURRENT_MONEY, 0);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        PlayerPrefs.SetInt(CURRENT_MONEY, currentMoney);
    }

    public void RemoveMoney(int amount)
    {
        currentMoney -= amount;
        PlayerPrefs.SetInt (CURRENT_MONEY, currentMoney);
    }
}
