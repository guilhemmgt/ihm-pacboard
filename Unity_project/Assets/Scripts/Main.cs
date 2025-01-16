//
//  Main.cs
//  Game
//  Created by Ingenuity i/o on 2025/01/08
//
// no description
//  Copyright © 2023 Ingenuity i/o. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Ingescape;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    public string _device = null;
    //default agent parameters to be overriden by command line parameters
    public uint _port = 0;
    public bool _verbose = true;


    void BarInputCallback (IopType iopType,
                            string name,
                            IopValueType valueType,
                            object value,
                            object myData) {
        UnityMainThreadDispatcher.Instance ().Enqueue (() => {
            Game agent = myData as Game;
            if (agent != null)
                agent.SetBarI ();
        });

    }


    void AgentEventCallback (AgentEvent agentEvent,
								   string uuid,
								   string name,
								   object eventData,
								   object myData) {
		UnityMainThreadDispatcher.Instance ().Enqueue (() => {
            Debug.Log (agentEvent + " from " + name + "(uuid: " + uuid + ")");
            Debug.Log ("event data: " + eventData);
		});
	}



    void ExternalStopCallback (object data) {
        Debug.Log ("ExternalForcedStop callback ...");
        UnityMainThreadDispatcher.Instance ().Enqueue (Close ());
    }

    void Start () {
        Application.logMessageReceived += LogMessageReceived;

        Igs.AgentSetName ("Game");
        Igs.DefinitionSetClass ("Game");
        Igs.LogSetConsoleLevel (LogLevel.LogDebug);
        Igs.LogSetConsole (_verbose);
        Igs.LogSetFile (_verbose);
        Igs.LogSetFileLevel (LogLevel.LogDebug);
        Igs.LogSetStream (_verbose);

        string[] netDevicesList = Igs.NetDevicesList ();
        string[] netAddressesList = Igs.NetAddressesList ();
        int deviceCount = netDevicesList.Count ();
        int addresseCount = netAddressesList.Count ();
		
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		if (deviceCount == addresseCount && string.IsNullOrEmpty (_device)) {
            if (deviceCount == 1) {
                _device = netDevicesList[0];
                Debug.Log (string.Format ("using {0} as default network device (this is the only one available)", _device));
            } else if (deviceCount == 2 && (netAddressesList[0] == "127.0.0.1" || netAddressesList[1] == "127.0.0.1")) {
                if (netAddressesList[0] == "127.0.0.1")
                    _device = netDevicesList[1];
                else
                    _device = netDevicesList[0];
                Debug.Log (string.Format ("using {0} as default network device (this is the only one available that is not the loopback)", _device));
            } else {
                if (deviceCount == 0) {
                    Debug.LogError ("No network device found");
                    Application.Quit ();
                } else {
					//Debug.LogError ("More than 2 net devices. Please use one of these network devices :");
					//foreach (string netDevice in netDevicesList)
					//    Debug.LogError (string.Format ("- {0}", netDevice));
				}

            }
        }
#else
        if (string.IsNullOrEmpty(_device))
        {
            Debug.LogError("Please use one of these network devices :");
            foreach (string netDevice in netDevicesList)
                Debug.LogError(string.Format("- {0}", netDevice));

            Application.Quit();
        }
#endif

		Debug.Log ("_device: " + _device);

		Igs.InputCreate ("bar", IopValueType.Impulsion);


        Igs.OutputCreate ("foo", IopValueType.Impulsion);

        Game agent = new Game ();

        Igs.ObserveInput ("bar", BarInputCallback, agent);

        Igs.ObserveAgentEvents (AgentEventCallback, agent);



        Igs.ObserveForcedStop (ExternalStopCallback, null);

        if (Igs.StartWithDevice (_device, _port) == Result.Success)
            Debug.Log (string.Format ("Starting with device {0} on port {1}", _device, _port));
        else
            Debug.Log (string.Format ("Failed with device {0} on port {1}", _device, _port));
    }

    private void LogMessageReceived (string condition, string stackTrace, LogType type) {
        string[] stackTraceLines = stackTrace.Split ('\n');
        string caller = (stackTraceLines.Length >= 2) ? stackTraceLines[1] : "";
        switch (type) {
            case (LogType.Assert):
            case (LogType.Error):
            case (LogType.Exception):
                Igs.Error (condition, caller);
                break;
            case (LogType.Warning):
                Igs.Warn (condition, caller);
                break;
            default:
                Igs.Info (condition, caller);
                break;
        }
    }

    private IEnumerator Close () {
        Debug.Log ("Application quit");
        Application.Quit ();
        yield return null;
    }

    void OnDestroy () {
        Application.Quit ();
    }

    private void OnApplicationQuit () {
        Application.logMessageReceived -= LogMessageReceived;
#if UNITY_EDITOR
        Igs.ClearContext ();
#endif
        Igs.Stop ();
        Debug.Log ("Ingescape stoped");
    }
}
