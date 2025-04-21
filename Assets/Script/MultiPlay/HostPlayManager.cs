using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class HostPlayManager : MultiPlayManagerBase
{
    [SerializeField] private GameObject _hostPlayerPrefab;
    
    private GameObject _hostPlayerInstance;
    
    private Lobby _joinedLobby;
    private void Start()
    {
        _hostPlayerInstance = Instantiate(_hostPlayerPrefab, Vector3.zero, Quaternion.identity);
        _joinedLobby = LobbyRetention.Instance.JoinedLobby;
        Debug.Log($"LobbyName : {_joinedLobby.Name} LobbyID : {_joinedLobby.Id}");
        NetworkManager.Singleton.OnClientConnectedCallback += _ => Lock();
    }

    void Update()
    {
        
    }

    private void Lock()
    {
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

    private enum LobbyState
    {
        WaitingForClient,
        Started,
    } 
}
