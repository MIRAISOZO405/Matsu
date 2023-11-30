using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RentalIncome : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;    // 家賃収入用
    [SerializeField] private MoneyManager moneyManager;    // 家賃収入用
    [SerializeField] private LevelManager levelManager;

    private int rent;   // 家賃

    private void Start()
    {
        if (!scoreManager) Debug.LogError("アタッチされていません");      
        if (!moneyManager)Debug.LogError("アタッチされていません");       
        if (!levelManager)Debug.LogError("アタッチされていません");
    }
   
    public void RentPayment()
    {
        int score = scoreManager.GetScore();
        int payment = score * rent;
        moneyManager.AddMoney(payment);
    }

    public void SetRent(int newRent)
    {
        rent = newRent;
    }
}
