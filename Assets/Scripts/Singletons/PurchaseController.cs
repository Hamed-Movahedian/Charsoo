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
            purchaseses.ForEach(p=>p.PurchaseID=p.PurchaseID.Trim());
            // Add new records
            LocalDBController
                .DataService.Connection
                .InsertAll(purchaseses, typeof(Purchases));
        }

        public IEnumerator SyncPurchases()
        {
            var purchases = LocalDBController
            .Table<Purchases>()
            .SqlWhere(p => p.Dirty)
            .ToList();

            if (purchases.Count == 0)
                yield break;
            string resualt = "";
            yield return ServerController.Post<string>(
                $@"Purchases/AddPurchases",
                purchases,
                respond =>
                {
                    resualt = respond;
                    //FollowMachine.SetOutput(respond);
                },
                request =>
                {
                    if (request.isNetworkError)
                    {
                        //FollowMachine.SetOutput("No Network");
                        resualt = "No Network";
                    }
                    else if (request.isHttpError)
                    {
                        //FollowMachine.SetOutput("Fail");
                        resualt = "Fail";
                    }
                });

            if (resualt == "Success")
            {
                purchases.ForEach(pp =>
                {
                    pp.Dirty = false;
                    LocalDBController.InsertOrReplace(pp);
                });
            }
        }
    }
}
