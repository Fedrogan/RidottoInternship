using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceHolder : MonoBehaviour
{
    private float prevBalance;
    private float newBalance;
    [SerializeField] CounterAnimator balanceAnimator;

    private void Start()
    {
        prevBalance = 0;
        newBalance = 0;
    }

    public void AddPrize(float prize) 
    {
        newBalance += prize;
        CountBalance();
        prevBalance = newBalance;
    }

    private void CountBalance()
    {
        balanceAnimator.UpdateValue(prevBalance, newBalance);
    }
}
