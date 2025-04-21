using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyRetention : MonoBehaviour
{
    public Lobby _joinedLobby { get; private set; }
    
    public static LobbyRetention Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LobbyJoin(Lobby lobby)
    {
        _joinedLobby = lobby;
    }
}
