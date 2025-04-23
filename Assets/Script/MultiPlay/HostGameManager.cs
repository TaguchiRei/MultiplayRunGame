using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class HostGameManager : MultiPlayManagerBase
{
    [SerializeField] private GameObject _hostPlayer;
    
    [SerializeField] private TextMeshProUGUI _startText;
    
    [SerializeField] private MultiPlayRadioTower _multiPlayRadioTower;
    
    private GameObject _hostPlayerInstance;
    
    private Lobby _joinedLobby;
    
    
    private HostPlayerManager _hostPlayerManager;
    
    private void Start()
    {
        _startText.enabled = false;
        _joinedLobby = LobbyRetention.Instance.JoinedLobby;
        Debug.Log($"LobbyName : {_joinedLobby.Name} LobbyID : {_joinedLobby.Id}");
        
        _hostPlayerInstance = Instantiate(_hostPlayer);
        _hostPlayerManager = _hostPlayerInstance.GetComponent<HostPlayerManager>();
        _hostPlayerManager._hostGameManager = this;
        
        _multiPlayRadioTower.OnMultiPlayDataReceived = MethodInvoker;
    }
    
    private void Lock()
    {
        Debug.Log("Lock");
        _startText.enabled = true;
        _startText.text = "サポーターが接続しました";
        _ = LobbyLock();
    }

    private void MethodInvoker(MultiPlayData multiPlayData,int methodNum)
    {
        switch (methodNum)
        {
            case 0 :
                Debug.Log("Client Connection");
                Lock();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ロビーをロックする
    /// </summary>
    private async UniTask LobbyLock()
    {
        var updateOptions = new UpdateLobbyOptions
        {
            IsLocked = true
        };
        
        await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, updateOptions);
        await GameStart();
    }

    private async UniTask GameStart()
    {
        await UniTask.WaitForSeconds(2);
        for (int i = 5; i >= 0; i--)
        {
            _startText.text = i.ToString();
            await UniTask.WaitForSeconds(1);
        }
        _hostPlayerManager.GameStart();
    }
}
