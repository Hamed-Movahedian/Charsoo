using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

namespace Assets.Scripts.Singletons
{
    public class PurchaseController : MonoBehaviour
    {
        [FollowMachine("Restore Purchases", "Success,Fail,Not Register")]
        public IEnumerator RestorePurchase(int? playerID)
        {
            if (playerID == null)
            {
                FollowMachine.SetOutput("Not Register");
                yield break;
            }
            
            // Get records from server
            List<Purchases> purchaseses = null;
            yield return ServerController.Post<List<Purchases>>(
                $@"Purchases/GetPurchase?playerID={playerID}&clientLastCmdTime={DateTime.MinValue:s}",
                null,
                pList => { purchaseses = pList; });

            // Add records to local table
            if(purchaseses==null)
                FollowMachine.SetOutput("Fail");
            else
            {
                // Clear table
                LocalDBController.DataService.Connection.DeleteAll<Purchases>();

                // Add new records
                LocalDBController
                    .DataService.Connection
                    .InsertAll(purchaseses, typeof(Purchases));

                FollowMachine.SetOutput("Success");
            }
        }
    }
}
