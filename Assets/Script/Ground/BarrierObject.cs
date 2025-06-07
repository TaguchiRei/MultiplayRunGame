using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class BarrierObject : MonoBehaviour
{
    [SerializeField,Grouping] private bool _isHostBarrier;
    
    MultiPlayRadioTower _multiPlayRadioTower;

    private void OnEnable()
    {
        _multiPlayRadioTower = FindAnyObjectByType<MultiPlayRadioTower>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!NetworkManager.Singleton.IsHost) return;//バリアへの衝突判定はホスト側のみで行う

        _multiPlayRadioTower.Send(other.gameObject.CompareTag("HostBarrier") == _isHostBarrier ? 3 : 4);
    }
}
