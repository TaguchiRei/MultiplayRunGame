using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class BarrierObject : MonoBehaviour
{
    [SerializeField] private bool _isHostBarrier;
    private MultiPlayRadioTower _multiPlayRadioTower;
    private GroundManager _groundManager;

    public Transform GroundTransform;

    private void OnEnable()
    {
        _multiPlayRadioTower = FindAnyObjectByType<MultiPlayRadioTower>();
    }

    private void FixedUpdate()
    {
        if(!NetworkManager.Singleton.IsHost) return;
        if (GroundTransform)
        {
            transform.position = GroundTransform.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!NetworkManager.Singleton.IsHost) return;//バリアへの衝突判定はホスト側のみで行う

        _multiPlayRadioTower.Send(other.gameObject.CompareTag("HostBarrier") == _isHostBarrier ? 3 : 4);
    }
}
