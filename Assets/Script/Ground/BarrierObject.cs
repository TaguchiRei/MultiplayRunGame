using System;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class BarrierObject : MonoBehaviour
{
    [SerializeField] private bool _isHostBarrier;
    private MultiPlayRadioTower _multiPlayRadioTower;
    private GroundManager _groundManager;

    private void OnEnable()
    {
        _multiPlayRadioTower = FindAnyObjectByType<MultiPlayRadioTower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"IsHost : {NetworkManager.Singleton.IsHost}");
        Debug.Log($"IsClient : {NetworkManager.Singleton.IsClient}");
        if(!NetworkManager.Singleton.IsHost) return;//バリアへの衝突判定はホスト側のみで行う
        _multiPlayRadioTower.SendBoth(other.gameObject.CompareTag("HostBarrier") == _isHostBarrier ? 3 : 4);
    }
}
