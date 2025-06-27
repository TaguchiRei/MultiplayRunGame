using Unity.Netcode;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    [SerializeField] private GameObject _obstacle;

    [ClientRpc]
    public void ObstacleHideClientRpc()
    {
        ObstacleHide();
    }

    public void ObstacleHide()
    {
        _obstacle.SetActive(false);
        Invoke(nameof(ObstacleShow), 7f);
    }

    private void ObstacleShow()
    {
        _obstacle.SetActive(true);
    }
}
