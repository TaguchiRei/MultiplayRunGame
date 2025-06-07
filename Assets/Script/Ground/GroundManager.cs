using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
    
    private List<WallObject> _wallObjects;
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

        for (int i = 0; i < _obstacleObjects.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
               _obstacleInstances[i].Enqueue(Instantiate(_obstacleObjects[i], _obstaclePool, Quaternion.identity));
            }
        }

        foreach (var obstacle in _obstacleObjects)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(obstacle, _obstaclePool, Quaternion.identity);
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
            foreach (var wallObj in _wallObjects)
            {
                if (_groundObjects[i] == wallObj.TargetTransform)
                {
                    wallObj.WallGameObject.transform.position = _groundObjects[i].position;
                }
            }
        }

        if (_groundObjects[0].position.z < GroundSize * -1)
        {
            if (_wallObjects[0].TargetTransform == _groundObjects[0])
            {
                _wallObjects[0].TargetTransform.position = _obstaclePool;
                _wallObjects.RemoveAt(0);
            }
            _groundObjects.Add(_groundObjects[0]);
            _groundObjects.RemoveAt(0);
            if (_obstacleSpawnCounter >= _obstacleSpawnTiming)
            {
                _obstacleSpawnCounter = 0;
                bool isWall = Random.value < 0.5f;
                if (isWall)
                {
                    GameObject obj = _obstacleInstances[0].Dequeue();
                   _wallObjects.Add(new WallObject()
                   {
                       TargetTransform = _groundObjects[0].transform,
                       WallGameObject = obj
                   }); 
                   _obstacleInstances[0].Enqueue(obj);
                }
            }
            else
            {
                _obstacleSpawnCounter++;
            }
        }
    }

    private struct WallObject
    {
        public Transform TargetTransform;
        public GameObject WallGameObject;
    }
}