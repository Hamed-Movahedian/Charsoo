using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

public class CheckMarket : MonoBehaviour
{
    public bool BazzarMarket;
    
    [FollowMachine("Market", "Bazaar,ZarrinPal")]
    public void SelectMarket()
    {
        
        if (BazzarMarket)
        {
            FollowMachine.SetOutput("Bazaar");
            return;
        }
        FollowMachine.SetOutput("ZarrinPal");
    }
}
