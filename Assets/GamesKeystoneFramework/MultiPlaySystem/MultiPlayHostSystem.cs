using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayHostSystem : MonoBehaviour
    {
        /// <summary>
        /// ロビーを作成する際に使用するメソッド。
        /// </summary>
        public async UniTask<(bool,Lobby)> CreateLobby(LobbyData lobbyData)
        {
                //Relayの割り当て
                var allocation = await RelayService.Instance.CreateAllocationAsync(lobbyData.MaxPlayers);
                lobbyData.Data ??= new();

                //Lobbyの作成
                var createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = lobbyData.IsPrivate,
                    IsLocked = lobbyData.IsLocked,
                    Data = lobbyData.Data,
                };

                //RelayJoinCode取得と追加
                var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                if (!lobbyData.IsPrivate)
                {
                    Debug.Log($"Add JoinCode : {relayJoinCode}");
                    createLobbyOptions.Data.Add("RelayJoinCode",
                        new DataObject(lobbyData.VisibilityOptions, relayJoinCode));
                }

                //ロビー作成
                var joinLobby = await LobbyService.Instance.CreateLobbyAsync
                    (lobbyData.LobbyName, lobbyData.MaxPlayers, createLobbyOptions);
                
// Relayの接続方式をプラットフォームによって変更する
#if UNITY_WEBGL && !UNITY_EDITOR
                var relayServerData = allocation.ToRelayServerData("wss");
#else
                var relayServerData = allocation.ToRelayServerData("dtls");
#endif

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                return (true, joinLobby);
        }

        public bool ConnectionHost()
        {
            //ホストとして接続
            try
            {
                NetworkManager.Singleton.StartHost();
                Debug.Log($"IsServer{NetworkManager.Singleton.IsServer}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Connection Host Error :{e.Message}");
                return false;
            }
        }
    }
}