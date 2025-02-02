//
//  Main.cs
//  Game
//  Created by Ingenuity i/o on 2025/02/02
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

public class Main : MonoBehaviour
{
    public string _device = null;
    //default agent parameters to be overriden by command line parameters
    public uint _port = 5140;
    public bool _verbose = true;


    void DirectionInputCallback(IopType iopType,
                            string name,
                            IopValueType valueType,
                            object value,
                            object myData)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Game agent = myData as Game;
            if (agent != null)
                agent.DirectionI = (int)value;
        });

    }




    void ExternalStopCallback(object data)
    {
        Debug.Log("ExternalForcedStop callback ...");
        UnityMainThreadDispatcher.Instance().Enqueue(Close());
    }

    void Start()
    {
        Application.logMessageReceived += LogMessageReceived;

        Igs.AgentSetName("Game");
            Igs.DefinitionSetClass("Game");
        Igs.LogSetConsoleLevel(LogLevel.LogDebug);
        Igs.LogSetConsole(_verbose);
        Igs.LogSetFile(_verbose);
        Igs.LogSetFileLevel(LogLevel.LogDebug);
        Igs.LogSetStream(_verbose);

        string[] netDevicesList = Igs.NetDevicesList();
        string[] netAddressesList = Igs.NetAddressesList();
        int deviceCount = netDevicesList.Count();
        int addresseCount = netAddressesList.Count();

    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (deviceCount == addresseCount && string.IsNullOrEmpty(_device))
        {
            if (deviceCount == 1)
            {
                _device = netDevicesList[0];
                Debug.Log(string.Format("using {0} as default network device (this is the only one available)", _device));
            }
            else if (deviceCount == 2 && (netAddressesList[0] == "127.0.0.1" || netAddressesList[1] == "127.0.0.1"))
            {
                if (netAddressesList[0] == "127.0.0.1")
                    _device = netDevicesList[1];
                else
                    _device = netDevicesList[0];
                Debug.Log(string.Format("using {0} as default network device (this is the only one available that is not the loopback)", _device));
            }
            else
            {
                if (deviceCount == 0)
                    Debug.LogError("No network device found");
                else
                {
                    Debug.LogError("More than 2 net devices. Please use one of these network devices :");
                    foreach (string netDevice in netDevicesList)
                        Debug.LogError(string.Format("- {0}", netDevice));
                }
                Application.Quit();
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
        Igs.InputCreate("direction", IopValueType.Integer);
        Igs.InputSetDescription("direction", "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0//EN\" \"http://www.w3.org/TR/REC-html40/strict.dtd\">\n<html><head><meta name=\"qrichtext\" content=\"1\" /><meta charset=\"utf-8\" /><style type=\"text/css\">\np, li { white-space: pre-wrap; }\nhr { height: 1px; border-width: 0; }\nli.unchecked::marker { content: \"\\2610\"; }\nli.checked::marker { content: \"\\2612\"; }\n</style></head><body style=\" font-family:'Asap'; font-size:13px; font-weight:400; font-style:normal;\">\n<p style=\" margin-top:0px; margin-bottom:0px; margin-left:0px; margin-right:0px; -qt-block-indent:0; text-indent:0px;\">0=up, 1=left, 2=down, 3=right</p></body></html>");
        Igs.InputAddConstraint("direction", "range [0,3]");


        Igs.OutputCreate("coin_collected", IopValueType.Impulsion);
        Igs.OutputCreate("ghost_killed", IopValueType.Impulsion);
        Igs.OutputCreate("game_won", IopValueType.Impulsion);
        Igs.OutputCreate("game_lost", IopValueType.Impulsion);

        Game agent = Game.GetInstance ();

        Igs.ObserveInput("direction", DirectionInputCallback, agent);



        Igs.ObserveForcedStop(ExternalStopCallback, null);

        if (Igs.StartWithDevice(_device, _port) == Result.Success)
            Debug.Log(string.Format("Starting with device {0} on port {1}", _device, _port));
    }

    private void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        string[] stackTraceLines = stackTrace.Split('\n');
        string caller = (stackTraceLines.Length >= 2) ? stackTraceLines[1] : "";
        switch (type)
        {
            case (LogType.Assert):
            case (LogType.Error):
            case (LogType.Exception):
                Igs.Error(condition, caller);
                break;
            case (LogType.Warning):
                Igs.Warn(condition, caller);
                break;
            default:
                Igs.Info(condition, caller);
                break;
        }
    }

    private IEnumerator Close()
    {
        Debug.Log("Application quit");
        Application.Quit();
        yield return null;
    }

    void OnDestroy()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Application.logMessageReceived -= LogMessageReceived;
        #if UNITY_EDITOR
            Igs.ClearContext();
        #endif
        Igs.Stop();
        Debug.Log("Ingescape stoped");
    }
}
