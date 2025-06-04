using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _groundCount;
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _groundObject;
    [SerializeField] private int _obstacleSpawnTiming;
    [SerializeField] private GameObject[] _obstacleObjects;
    [SerializeField] private Vector3 _obstaclePool;

    private List<Transform> _groundObjects;
    private Queue<GameObject>[] _obstacleInstances;
    private int _obstacleSpawnCounter;
    private bool _isStarted = false;
    private const float GroundSize = 60; //グラウンドの大きさは60

    public void GameStart()
    {
        _isStarted = false;
        if (NetworkManager.Singleton.IsHost)
        {
            _ = Initialize();
        }
    }

    private async UniTask Initialize()
    {
        var groundResult = await InstantiateAsync(_groundObject, _groundCount, Vector3.zero, Quaternion.identity);
        _groundObjects = groundResult.Select(o => o.transform).ToList();
        _isStarted = true;
        for (int i = 0; i < _groundObjects.Count; i++)
        {
            _groundObjects[i].position = Vector3.forward * (i * GroundSize);
            _groundObjects[i].GetComponent<NetworkObject>().Spawn();
        }

        for (int i = 0; i < _obstacleObjects.Length; i++)
        {
            var obstacleResult = await InstantiateAsync(_obstacleObjects[i], 2, _obstaclePool, Quaternion.identity);

            foreach (var obstacle in obstacleResult)
            {
                _obstacleInstances[i].Enqueue(obstacle);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_isStarted) return;
        _groundObjects[0].position += Vector3.back * (_speed * Time.fixedDeltaTime);

        for (int i = 1; i < _groundObjects.Count; i++)
        {
            _groundObjects[i].position = _groundObjects[0].position + Vector3.forward * (i * GroundSize);
        }

        if (_groundObjects[0].position.z < GroundSize * -1)
        {
            _groundObjects.Add(_groundObjects[0]);
            _groundObjects.RemoveAt(0);
            if (_obstacleSpawnCounter >= _obstacleObjects.Length)
            {
                _obstacleSpawnCounter = 0;
                
            }
            else
            {
                _obstacleSpawnCounter++;
            }
        }
    }
}