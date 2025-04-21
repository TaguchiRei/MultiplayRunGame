using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class HostPlayManager : MultiPlayManagerBase
{
    [SerializeField] private GameObject _hostPlayer;
    
    [SerializeField] private TextMeshProUGUI _startText;
    
    private GameObject _hostPlayerInstance;
    
    private Lobby _joinedLobby;
    private void Start()
    {
        _startText.enabled = false;
        _joinedLobby = LobbyRetention.Instance.JoinedLobby;
        Debug.Log($"LobbyName : {_joinedLobby.Name} LobbyID : {_joinedLobby.Id}");
        NetworkManager.Singleton.OnClientConnectedCallback += _ => Lock();
    }
    
    private void Lock()
    {
        Debug.Log("Lock");
        _startText.enabled = true;
        _startText.text = "サポーターが接続しました";
        _ = LobbyLock();
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
    }

    private async UniTask GameStart()
    {
        await UniTask.WaitForSeconds(1);
        
    }
}
