using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MgsCommonLib;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class ServerController : MgsSingleton<ServerController>
{
    public UserPuzzlesServer UserPuzzles;

    #region URL

    public static string URL
    {
        get
        {
#if UNITY_EDITOR
            //return "http://charsoogame.ir";
            return "http://localhost:52391";
#else
            return "http://charsoogame.ir";
#endif

        }
    }

    #endregion

    #region PostRequst

    public static UnityWebRequest PostRequest(string url, object bodyData)
    {
        UnityWebRequest request = new UnityWebRequest(
            URL+@"/api/" + url,
            "POST",
            new DownloadHandlerBuffer(),
            (bodyData==null) ? null : new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyData))));

        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    #endregion

    #region GetRequest

    public static UnityWebRequest GetRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(URL + @"/api/"+ url);

        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }
    #endregion

    #region Post

 
    public static IEnumerator Post<TReturnType>(string url, object bodyData, 
        Action<TReturnType> onSuccess, 
        Action<UnityWebRequest> onError=null )
    {
        UnityWebRequest request = PostRequest(url, bodyData);

        request.Send();

        while (!request.isDone)
            yield return null;

        if (!request.isHttpError && !request.isNetworkError)
        {
            if (onSuccess != null)
                onSuccess(JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
        }
        else
        {
            if (onError != null)
                onError(request);
        }
    }

    #endregion

    #region Get

    public static IEnumerator Get<TReturnType>(string url, Action<object> onSuccess)
    {
        UnityWebRequest request = GetRequest(url);

        request.Send();

        while (!request.isDone)
            yield return null;

        if (!request.isHttpError && !request.isNetworkError)
            if(onSuccess!=null)
                onSuccess(JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
    }
    

    #endregion
}
