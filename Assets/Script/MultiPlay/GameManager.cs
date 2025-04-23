using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using UnityEngine;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState inGameState;
    [SerializeField] MultiPlayRadioTower radioTower;
    
    private bool _started = false;
    
    public void HostConnection()
    {
        radioTower.OnMultiPlayDataReceived += MethodInvoker;
        inGameState = InGameState.Waiting;
    }

    public void ClientConnection()
    {
        radioTower.OnMultiPlayDataReceived += MethodInvoker;
        inGameState = InGameState.Client;
        radioTower.Send(0);
    }

    private void Update()
    {
        if(!_started)return;
        
    }

    private void MethodInvoker(MultiPlayData data ,int num)
    {
        switch (num)
        {
            case 0:
                inGameState = InGameState.Connecting;
                Debug.Log("Connecting");
                break;
            default:
                break;
        }
    }

    public enum InGameState
    {
        Waiting,
        Connecting,
        Playing,
        Client,
    }
}
