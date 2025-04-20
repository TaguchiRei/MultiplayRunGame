using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayClientSystem : MonoBehaviour
    {
        /// <summary>
        /// ロビーリストの取得に使う。
        /// </summary>
        /// <returns></returns>
        public async UniTask<(bool,List<Lobby>)> GetAllLobbyList()
        {
            try
            {
                var lobbyList = await LobbyService.Instance.QueryLobbiesAsync();
                return (true, lobbyList.Results);
            }
            catch (Exception e)
            {
                Debug.LogError($"Get Lobby List Error : {e}");
                return (false,null);
            }
        }
        
        /// <summary>
        /// ロビーがまだ存在するかを調べる
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        protected async UniTask<bool> LobbyCheck(string lobbyId)
        {
            try
            { 
                await LobbyService.Instance.GetLobbyAsync(lobbyId);
                Debug.Log("Lobby Found");
                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"Lobby Not Found : {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// ロビーコードを利用したロビー参加に使う
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public async UniTask<bool> JoinLobbyFromLobbyId(string lobbyId)
        {
            try
            {
                var check = await LobbyCheck(lobbyId);
                if (!check) return false;
                
                await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("Join Success");
                return true;

            }
            catch(Exception e)
            {
                Debug.LogError($"Join Lobby Error : {e}");
                return false;
            }
        }
        

        public async UniTask<bool> JoinRelay(string joinCode)
        {
            try
            {
                var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                unityTransport.SetRelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData);
                
                    NetworkManager.Singleton.StartClient();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Join Relay Error{e.Message}");
                return false;
            }
        }
    }
}
