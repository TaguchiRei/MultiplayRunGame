using System;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using GamesKeystoneFramework.MethodSupport;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState inGameState;
    [SerializeField] MultiPlayRadioTower radioTower;
    [SerializeField] InputManager inputManager;
    
    private bool _started = false;
    
    /// <summary>
    /// 直近のジャンプのタイミング。
    /// </summary>
    [ReadOnlyInInspector]public NetworkVariable<float> _latestJumpTime;
    
    public void HostConnection()
    {
        Debug.Log("HostConnection ManagerInitialize");
        radioTower.OnMultiPlayDataReceived += MethodInvoker;
        inGameState = InGameState.Waiting;
    }
    
    public void ClientConnection()
    {
        _ = WaitSpawn();
    }

    private async UniTask WaitSpawn()
    {
        radioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        await UniTask.WaitUntil(() => radioTower.IsSpawned);
        radioTower.OnMultiPlayDataReceived += MethodInvoker;
        inGameState = InGameState.Client;
        radioTower.TestSend();
        radioTower.Send(0);
    }
    
    

    private void Update()
    {
        if(!_started)return;
    }

    private void HostJump()
    {
        //ジャンプした際にジャストかどうかの判定を行うプログラムを入れる
    }

    private void ClientJump()
    {
        
    }

    private void MethodInvoker(MultiPlayData data ,int num)
    {
        Debug.Log("Invoker");
        switch (num)
        {
            case 0://ホスト側
                inGameState = InGameState.Connecting;
                radioTower.Send(1);
                Debug.Log("Connecting");
                break;
            case 1://クライアント側
                
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
