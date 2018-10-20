using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FollowMachineDll.Assets;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ServerData))]
public class ServerDataCustomInspector : Editor
{
    private ServerData _serverData;

    public override void OnInspectorGUI()
    {
        _serverData = target as ServerData;

        if (GUILayout.Button("Update Interface"))
        {
            var controllers = ServerEditor.Get(@"Interface/GetControllers", "Download interface", "Download");

            var controllersJArray = JArray.Parse(controllers);

            _serverData.Controllers.Clear();

            foreach (JToken controllerJObject in controllersJArray)
            {
                var controller = new ServerData.Controller();

                controller.Name = (string)controllerJObject["Name"];



                controller.Methods = new List<ServerData.Controller.MethodData>();

                foreach (var methodJobject in controllerJObject["Methods"])
                {
                    var outputs = (string)methodJobject["Outputs"];

                    controller.Methods.Add(new ServerData.Controller.MethodData
                    {
                        Name = (string)methodJobject["Name"],
                        Info = (string)methodJobject["Info"],
                        Outputs = outputs == null
                            ? new List<string>()
                            : outputs
                                .Split(',')
                                .Select(s => s.Trim())
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList()
                    });
                }
                _serverData.Controllers.Add(controller);
            }
        }

        DrawDefaultInspector();
    }
}
