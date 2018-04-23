using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ServerEditor
{
    #region ProcessRequest

    private static bool ProcessRequest(UnityWebRequest request, string title, string operation)
    {
        request.Send();

        while (!request.isDone)
        {
            if (EditorUtility.DisplayCancelableProgressBar(operation, title, request.downloadProgress))
            {
                EditorUtility.ClearProgressBar();
                return false;
            }
        }
        EditorUtility.ClearProgressBar();

        if (request.isNetworkError)
        {
            EditorUtility.DisplayDialog("Network Error", request.error, "OK");
            return false;
        }


        if (request.isHttpError)
        {
            EditorUtility.DisplayDialog("Http Error", request.error, "OK");
            return false;
        }
        return true;
    }


    #endregion

  
    public static TReturnType Get<TReturnType>(string url, string title, string operation)
    {
        UnityWebRequest request = Server.GetRequest(url);

        if (!ProcessRequest(request, title, operation)) return default(TReturnType);

        return (JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
    }

    public static bool Post(string url, object bodyData, string title, string operation)
    {
        var request = Server.PostRequest(url, bodyData);

        return ProcessRequest(request, title, operation);
    }

 
    public static TReturnType Post<TReturnType>(string url, object bodyData, string title, string operation) 
    {
        var request = Server.PostRequest(url, bodyData);

        if (!ProcessRequest(request, title, operation))
            return default(TReturnType);

        return JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text);
    }
 
}
