using GamesKeystoneFramework.MultiPlaySystem;
using UnityEngine;

public class HostPlayManager : MultiPlayManagerBase
{
    [SerializeField] private GameObject _hostPlayerPrefab;
    
    private GameObject _hostPlayerInstance;
    void Start()
    {
        _hostPlayerInstance = Instantiate(_hostPlayerPrefab, Vector3.zero, Quaternion.identity);
    }
    
    
}
