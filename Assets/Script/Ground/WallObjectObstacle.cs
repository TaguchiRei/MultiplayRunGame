using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class WallObjectObstacle : MonoBehaviour
{
    private MultiPlayRadioTower _multiPlayRadioTower;
    private GroundManager _groundManager;
    

    private void OnEnable()
    {
        _multiPlayRadioTower = FindAnyObjectByType<MultiPlayRadioTower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!NetworkManager.Singleton.IsHost) return;//壁への衝突判定はホスト側のみで行う
        _multiPlayRadioTower.SendBoth(4);
    }
}
