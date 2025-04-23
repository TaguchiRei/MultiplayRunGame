using System;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using GamesKeystoneFramework.MethodSupport;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState inGameState;
    [SerializeField] MultiPlayRadioTower radioTower;
    [SerializeField] InputManager inputManager;

    [SerializeField, Grouping] private NetworkObject _clientPlayerNetworkObject;
    
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
    
    /// <summary>
    /// クライアント側で呼ばれる
    /// </summary>
    public void ClientConnection()
    {
        _ = WaitSpawn();
    }

    /// <summary>
    /// クライアント側で呼ばれる
    /// </summary>
    private async UniTask WaitSpawn()
    {
        radioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        await UniTask.WaitUntil(() => radioTower.IsSpawned);
        radioTower.OnMultiPlayDataReceived += MethodInvoker;
        inGameState = InGameState.Client;
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
                ulong ID = 0;
                foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    if (client.ClientId != NetworkManager.Singleton.LocalClientId)
                    {
                        ID = client.ClientId;
                        break;
                    }
                }
                if(ID == 0)return;
                _clientPlayerNetworkObject.ChangeOwnership(ID);
                radioTower.Send(1);
                Debug.Log("Connecting");
                break;
            case 1://クライアント側
                _ = WaitChangeOwner();
                break;
            default:
                break;
        }
    }

    private async UniTask WaitChangeOwner()
    {
        await UniTask.WaitUntil(() => _clientPlayerNetworkObject.IsOwner);
        Debug.Log("Owner Changed");
    }

    public enum InGameState
    {
        Waiting,
        Connecting,
        Playing,
        Client,
    }
}
