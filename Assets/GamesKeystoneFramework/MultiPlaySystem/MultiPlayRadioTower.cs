using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayRadioTower : NetworkBehaviour
    {
        public void Send()
        {
            var data = new MultiPlayData();
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("MultiPlayRadioTower: Sending MultiPlayData");
                data.Value = "Send To Client";
                SendDataToClientRPC(data);
            }
            else
            {
                Debug.Log("MultiPlayRadioTower: Not Host");
                data.Value = "Send To Server";
                SendDataToServerRPC(data);
            }
        }
        /// <summary>
        /// クライアントにデータを送信
        /// サーバー側で呼び出すとクライアント側で実行される
        /// </summary>
        [ClientRpc(RequireOwnership = false)]
        public void SendDataToClientRPC(MultiPlayData multiPlayData)
        {
            if(NetworkManager.Singleton.IsHost)return;
            Debug.Log(multiPlayData.Value);
        }

        /// <summary>
        /// サーバーにデータを送信
        /// クライアント側で呼び出すとサーバー側で実行される
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void SendDataToServerRPC(MultiPlayData multiPlayData)
        {
            Debug.Log(multiPlayData.Value);
        }
    }
}
