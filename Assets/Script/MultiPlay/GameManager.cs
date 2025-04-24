using System;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using GamesKeystoneFramework.MethodSupport;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState inGameState;
    [SerializeField] MultiPlayRadioTower radioTower;
    [SerializeField] InputManager inputManager;
    
    [SerializeField, Grouping] private NetworkObject _hostPlayerNetworkObject;
    [SerializeField, Grouping] private NetworkObject _clientPlayerNetworkObject;
    
    [SerializeField] TextMeshProUGUI _countdownText;

    
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
                Debug.Log("WaitChangeOwner");
                _ = WaitChangeOwner();
                break;
            case 2:
                Debug.Log("StartCountDown");
                _ = StartCountDown();
                if(NetworkManager.Singleton.IsHost)
                    radioTower.Send(2);
                break;
            default:
                break;
        }
    }

    private async UniTask WaitChangeOwner()
    {
        await UniTask.WaitUntil(() => _clientPlayerNetworkObject.IsOwner);
        Debug.Log($"Owner Changed {_clientPlayerNetworkObject.IsOwner}");
        radioTower.Send(2);
    }

    private async UniTask StartCountDown()
    {
        _countdownText.gameObject.SetActive(true);
        for (int i = 5; i >= 0; i--)
        {
            await UniTask.WaitForSeconds(1);
            _countdownText.text = i.ToString();
        }
        if(NetworkManager.Singleton.IsHost)
            _hostPlayerNetworkObject.gameObject.GetComponent<PlayerManager>().GameStart();
        else
            _clientPlayerNetworkObject.gameObject.GetComponent<PlayerManager>().GameStart();
    }

    public enum InGameState
    {
        Waiting,
        Connecting,
        Playing,
        Client,
    }
}
