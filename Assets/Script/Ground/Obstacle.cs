using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : NetworkBehaviour
{
    [SerializeField] private GameObject _obstacle;
    [SerializeField] private GameObject _brokenObstacle;

    [ClientRpc]
    public void ObstacleHideClientRpc()
    {
        _obstacle.SetActive(false);
        BrokenObstacleShow();
    }

    public void Update()
    {
        if (transform.position.z <= -20)
        {
            _obstacle.SetActive(true);
        }
    }

    private void BrokenObstacleShow()
    {
        _brokenObstacle.SetActive(true);
        for (int i = 0; i < _brokenObstacle.transform.childCount; i++)
        {
            var obj = _brokenObstacle.transform.GetChild(i).gameObject;
            obj.transform.localPosition = new Vector3(0, 0, Random.Range(-0.05f, 0.05f));
            var objectRig = obj.GetComponent<Rigidbody>();
            objectRig.linearVelocity = Vector3.zero;
            objectRig.AddForce(
                new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0), ForceMode.Impulse);
        }

        Invoke(nameof(BrokenObstacleHide), 3);
    }

    private void BrokenObstacleHide()
    {
        _obstacle.SetActive(false);
    }
}