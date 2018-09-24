using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CommandController : BaseObject
{
    private const string LastCmdKey = "LastUpdateCommandTime";

    private Dictionary<string, Action<JToken>> _commandToActionDic =
        new Dictionary<string, Action<JToken>>();

    public void AddListenerForCommand(string cmdString, Action<JToken> action)
    {
        _commandToActionDic.Add(cmdString,action);
    }

    public IEnumerator GetRecentCommands()
    {
        yield return ServerController.Post<string>(
            $@"Commands/GetCommands?playerID={101}&clientLastCmdTime={LastCmdTime:s}",
            null,
            RunCommands);

    }

    public void RunCommands(string content)
    {
        JArray jobject = JArray.Parse(content);

        List<JObject> commands = jobject.Select(co => co.ToObject<JObject>()).ToList();

        foreach (JObject command in commands)
        {
            string commandString = command.Value<string>("Command");

            if (_commandToActionDic.ContainsKey(commandString))
                _commandToActionDic[commandString](command["Data"]);
        }
    }

    public DateTime LastCmdTime
    {
        set
        {
            if (value > LastCmdTime)
                PlayerPrefs.SetString(LastCmdKey, value.ToString(CultureInfo.CurrentCulture));
        }
        get
        {
            return PlayerPrefs.HasKey(LastCmdKey) ?
                DateTime.Parse(PlayerPrefs.GetString(LastCmdKey)) :
                DateTime.MinValue;
        }
    }
 
 
}
