using GamesKeystoneFramework.MultiPlaySystem;
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
    }
    
}
