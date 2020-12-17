using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationProvider
{
    public class LocationWraper
    {
        public float Longitude;
        public float Latitude;
    }
/*

    public static IEnumerator GetLocation(LocationWraper locationWraper)
    {
        locationWraper.Longitude = Input.location.lastData.longitude;
        locationWraper.Latitude = Input.location.lastData.latitude;
        // First, check if user has locationWraper service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying locationWraper
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
            yield break;

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
            yield break;
        else
        {
            locationWraper.Longitude = Input.location.lastData.longitude;
            locationWraper.Latitude = Input.location.lastData.latitude;

        }

        // Stop service if there is no need to query locationWraper updates continuously
        Input.location.Stop();
    }
    */

}