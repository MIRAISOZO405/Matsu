using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RentalIncome : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;    // �ƒ������p
    [SerializeField] private MoneyManager moneyManager;    // �ƒ������p
    [SerializeField] private LevelManager levelManager;

    private int rent;   // �ƒ�

    private void Start()
    {
        if (!scoreManager) Debug.LogError("�A�^�b�`����Ă��܂���");      
        if (!moneyManager)Debug.LogError("�A�^�b�`����Ă��܂���");       
        if (!levelManager)Debug.LogError("�A�^�b�`����Ă��܂���");
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
