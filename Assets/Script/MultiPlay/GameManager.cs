using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState _inGameState;
    [SerializeField] MultiPlayRadioTower _radioTower;
    [SerializeField] InputManager _inputManager;
    [SerializeField] GroundManager _groundManager;
     
    [SerializeField, Grouping] private NetworkObject _hostPlayerNetworkObject;
    [SerializeField, Grouping] private NetworkObject _clientPlayerNetworkObject;
    
    [SerializeField] TextMeshProUGUI _countdownText;
    
    [SerializeField] private Animator _leftEffect;
    [SerializeField] private Animator _rightEffect;

    [SerializeField] private GameObject[] _titleObjects;

    
    private bool _started = false;

    private int _score;
    private int _hitPoints;
    
    
    /// <summary>
    /// 直近のジャンプのタイミング。
    /// </summary>
    [ReadOnlyInInspector]public NetworkVariable<float> _latestJumpTime;
    
    public void HostConnection()
    {
        Debug.Log("HostConnection ManagerInitialize");
        _radioTower.OnMultiPlayDataReceived += MethodInvoker;
        _inGameState = InGameState.Waiting;
    }

    public void Start()
    {
        _score = 0;
        _hitPoints = 5;
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
        _radioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        await UniTask.WaitUntil(() => _radioTower.IsSpawned);
        _radioTower.OnMultiPlayDataReceived += MethodInvoker;
        _inGameState = InGameState.Client;
        _radioTower.Send(0);
    }
    
    

    private void Update()
    {
        if(!_started)return;
    }

    private void GetScore()
    {
        Debug.Log("Add Score");
        _score++;
    }

    private void Damage()
    {
        Debug.Log("Damage");
        _hitPoints--;
    }

    public void Dead()
    {
        Debug.Log("Dead");
    }

    public void MethodInvoker(MultiPlayData data ,int num)
    {
        Debug.Log("Invoker");
        switch (num)
        {
            case 0://ホスト側
                _inGameState = InGameState.Connecting;
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
                _radioTower.Send(1);
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
                    _radioTower.Send(2);
                break;
            case 3://両者
                GetScore();
                break;
            case 4:
                Damage();
                break;
            default:
                break;
        }
    }

    private async UniTask WaitChangeOwner()
    {
        await UniTask.WaitUntil(() => _clientPlayerNetworkObject.IsOwner);
        Debug.Log($"Owner Changed {_clientPlayerNetworkObject.IsOwner}");
        _radioTower.Send(2);
    }

    private async UniTask StartCountDown()
    {
        _countdownText.gameObject.SetActive(true);
        for (int i = 5; i >= 0; i--)
        {
            await UniTask.WaitForSeconds(1);
            _countdownText.text = i.ToString();
        }
        _countdownText.gameObject.SetActive(false);
        
        if(NetworkManager.Singleton.IsHost)
            _hostPlayerNetworkObject.gameObject.GetComponent<MultiPlayInput>().GameStart();
        else
            _clientPlayerNetworkObject.gameObject.GetComponent<MultiPlayInput>().GameStart();
        _groundManager.GameStart();
        _inputManager.GameStart();
        foreach (var obj in _titleObjects)
        {
            obj.SetActive(false);
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
