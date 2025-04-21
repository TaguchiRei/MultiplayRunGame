using Unity.Netcode;
using UnityEngine;

public class PlaySceneMulti : MonoBehaviour
{
    [SerializeField] private GameObject _hostSceneManager;
    [SerializeField] private GameObject _clientSceneManager;
    private void Awake()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Instantiate(_hostSceneManager);
        }
        else
        {
            Instantiate(_clientSceneManager);
        }
    }
}
