using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySceneMulti : MonoBehaviour
{
    [SerializeField] private GameObject _hostGameManager;
    [SerializeField] private GameObject _clientGameManager;
    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Instantiate(_hostGameManager);
        }
        else
        {
            Instantiate(_clientGameManager);
        }
    }
}
