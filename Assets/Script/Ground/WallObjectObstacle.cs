using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class WallObjectObstacle : MonoBehaviour
{
    private MultiPlayRadioTower _multiPlayRadioTower;
    private GroundManager _groundManager;
    private SoloGameManager _soloGameManager;
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
            if(!NetworkManager.Singleton.IsHost) return;//壁への衝突判定はホスト側のみで行う
            _multiPlayRadioTower.SendBoth(4);
        }
        else
        {
            if (other.gameObject.CompareTag("HostPlayer")) _soloGameManager.GetScore();
            else _soloGameManager.Damage();
        }
    }
}
