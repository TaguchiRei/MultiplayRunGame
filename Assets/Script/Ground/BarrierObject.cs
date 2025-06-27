using System;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class BarrierObject : MonoBehaviour
{
    [SerializeField] private bool _isHostBarrier;
    private MultiPlayRadioTower _multiPlayRadioTower;
    private SoloGameManager _soloGameManager;
    private GroundManager _groundManager;

    private bool _obstacleSoloMode;

    private void OnEnable()
    {
        _multiPlayRadioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        if (_multiPlayRadioTower == null)
        {
            _obstacleSoloMode = true;
            _soloGameManager = FindAnyObjectByType<SoloGameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_obstacleSoloMode)
        {
            if (!NetworkManager.Singleton.IsHost) return; //バリアへの衝突判定はホスト側のみで行う
            _multiPlayRadioTower.SendBoth(other.gameObject.CompareTag("HostPlayer") == _isHostBarrier ? 3 : 4);
        }
        else
        {
            if (other.gameObject.CompareTag("HostPlayer")) _soloGameManager.GetScore();
            else _soloGameManager.Damage();
        }
    }
}