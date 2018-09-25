using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using UnityEngine;

namespace Assets.Scripts.Singletons
{
    public class PurchaseController : MgsSingleton<PurchaseController>
    {
        public void RestorePurchase(List<Purchases> purchaseses)
        {
            // Clear table
            LocalDBController.DataService.Connection.DeleteAll<Purchases>();

            // Add new records
            LocalDBController
                .DataService.Connection
                .InsertAll(purchaseses, typeof(Purchases));
        }
    }
}
