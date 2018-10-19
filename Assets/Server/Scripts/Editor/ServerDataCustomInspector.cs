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
            var controllers= ServerEditor.Get(@"Interface/GetControllers", "Download interface", "Download");

            var controllersJArray = JArray.Parse(controllers);

            _serverData.Controllers.Clear();

            foreach (var controllerJObject in controllersJArray)
            {
                var controller = new ServerData.Controller();

                controller.Name = (string) controllerJObject["Name"];

                controller.Methods=new List<string>();

                foreach (var methodJobject in controllerJObject["Methods"])
                {
                    controller.Methods.Add((string) methodJobject["Name"]);
                }
                _serverData.Controllers.Add(controller);
            }
        }

        DrawDefaultInspector();
    }
}
