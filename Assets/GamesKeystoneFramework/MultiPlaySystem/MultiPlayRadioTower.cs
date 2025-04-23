using System;
using Unity.Netcode;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayRadioTower : NetworkBehaviour
    {
        public Action<MultiPlayData,int> OnMultiPlayDataReceived;
        [SerializeField] NetworkObject networkObject;
        
        private void OnEnable()
        {
            
        }

        public void Send(int methodNum, MultiPlayData data = default)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("MultiPlayRadioTower: Sending MultiPlayData");
                data.Value = "Send To Client";
                SendDataToClientRPC(data, methodNum);
            }
            else
            {
                Debug.Log("MultiPlayRadioTower: Not Host");
                data.Value = "Send To Server";
                SendDataToServerRPC(data, methodNum);
            }
        }
        /// <summary>
        /// クライアントにデータを送信
        /// サーバー側で呼び出すとクライアント側で実行される
        /// サーバー　→　クライアント
        /// </summary>
        [ClientRpc(RequireOwnership = false)]
        public void SendDataToClientRPC(MultiPlayData multiPlayData,int methodNum)
        {
            if(NetworkManager.Singleton.IsHost)return;//Clientはホストも含まれるため
            Debug.Log(multiPlayData.Value);
            OnMultiPlayDataReceived?.Invoke(multiPlayData,methodNum);
        }

        /// <summary>
        /// サーバーにデータを送信
        /// クライアント側で呼び出すとサーバー側で実行される
        /// クライアント　→　サーバー
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void SendDataToServerRPC(MultiPlayData multiPlayData,int methodNum)
        {
            Debug.Log(multiPlayData.Value);
            OnMultiPlayDataReceived?.Invoke(multiPlayData,methodNum);
        }
    }
}
