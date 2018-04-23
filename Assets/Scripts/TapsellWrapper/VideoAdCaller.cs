using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VideoAdCaller : BaseObject
{
    public UnityEvent OnVideoReady;
    public UnityEvent OnVideoCompeleted;

    public void CallForGetVideo()
    {
        AdManager.GetAdVideo(OnVideoReady);
    }

    public void ShowVideo()
    {
        AdManager.PlayAdVideo(OnVideoCompeleted);
    }

}
