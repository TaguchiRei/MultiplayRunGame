using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public abstract class MultiPlayManagerBase : MonoBehaviour
    {
        [SerializeField] private GameObject MultiPlayObjectGroup;

        [field: Header("初期化完了しているかどうか")]
        public bool CanMultiPlay { get; private set; }


        [SerializeField,Grouping,Header("必須")] protected SystemClass _systemClass;
        
        [SerializeField, Grouping,Header("ロビーを作成する際に使用するデータ")]
        protected LobbyData lobbyData;
        
        [SerializeField, ReadOnlyInInspector,Header("接続状況")]
        private ConnectionPhase connectionPhase;
        
        /// <summary>
        /// ロビーのリスト
        /// </summary>
        public static QueryLobbiesOptions QueryLobbiesOptions;

        protected Lobby _joinedLobby;
        
        /// <summary>
        /// ホストする際にサービスを初期化する。
        /// 一度だけ使用する
        /// 通信エラー発生時はfalseを返す
        /// </summary>
        protected async UniTask<bool> ServicesInitialize()
        {
            connectionPhase = ConnectionPhase.NotInitialized;
            //ゲーム側のサービス初期化
            try
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    //アカウントが無かった場合新たに作成
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"Services Initialize Error{e.Message}");
                CanMultiPlay = false;
                return false;
            }
        
            Debug.Log("Services initialized");
            CanMultiPlay = true;
            connectionPhase = ConnectionPhase.OffLine; 
            return true;
        }

        /// <summary>
        /// 通信を切断する際に使用する
        /// </summary>
        /// <returns></returns>
        protected async UniTask<bool> DisConnect()
        {
            if(!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)return false;
            try
            {
                NetworkManager.Singleton.Shutdown();
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                connectionPhase = ConnectionPhase.OffLine;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Disconnect Error : {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// ホスト接続する際に使用する
        /// </summary>
        /// <returns></returns>
        protected async UniTask<bool> HostConnect()
        {
            if(!CanMultiPlay) return false;

            var lobbySetting = await _systemClass.MultiPlayHostSystem.CreateLobby(lobbyData);
            if (!lobbySetting.Item1)
            {
                Debug.Log("HostConnect Failed");
                return false;
            }
            _joinedLobby = lobbySetting.Item2;

            if (_systemClass.MultiPlayHostSystem.ConnectionHost())
            {
                Debug.Log("HostConnect Success");
                return true;
            }

            Debug.Log("HostConnect Failed");
            return false;
        }

        protected async UniTask<bool> ClientConnect(Lobby lobby)
        {
            var joinLobby = await _systemClass.MultiPlayClient.JoinLobbyFromLobbyId(lobby.Id);
            if (!joinLobby)
            {
                Debug.Log("LobbyConnect Failed");
                return false;
            }
            connectionPhase = ConnectionPhase.JoinLobby;
            Debug.Log("LobbyConnect Success");
            var relayId = lobby.Data["RelayJoinCode"].Value;
            var joinRelay = await _systemClass.MultiPlayClient.JoinRelay(relayId);
            if (!joinRelay)
            {
                Debug.Log("LobbyConnect Failed");
                return false;
            }
            connectionPhase = ConnectionPhase.JoinRelay;
            Debug.Log("LobbyConnect Success");
            return true;
        }

        protected GameObject InstantiateMultiObject(GameObject obj,Vector3 position, Quaternion rotation = default)
        {
            if (obj == null)
            {
                Debug.Log("InstantiateMultiObject: obj is null");
                return null;
            }
            
            var spawnObj = Instantiate(obj,position,quaternion.identity);
            var objNetworkObj = spawnObj.GetComponent<NetworkObject>();
            if (objNetworkObj != null)
            {
                objNetworkObj.Spawn(true);
                spawnObj.transform.SetParent(MultiPlayObjectGroup.transform);
            }
            else
            {
                Debug.Log("NetworkObject Is Not found");
                Destroy(spawnObj);
                return null;
            }
            Debug.Log("ObjectSpawn Success");
            return spawnObj;
        }
        
        
    }

    [Serializable]
    public struct SystemClass
    {
        public MultiPlayHostSystem MultiPlayHostSystem;
        public MultiPlayClientSystem MultiPlayClient;
    }
    /// <summary>
    /// ロビー作成時に設定する項目をまとめた構造体
    /// </summary>
    [Serializable]
    public struct LobbyData
    {
        public string LobbyName;
        public int MaxPlayers;
        public bool IsPrivate;
        public bool IsLocked;
        public DataObject.VisibilityOptions VisibilityOptions;
        public Dictionary<string, DataObject> Data;
    }

    public enum ConnectionPhase
    {
        NotInitialized,
        OffLine,
        JoinLobby,
        JoinRelay
    }
}
