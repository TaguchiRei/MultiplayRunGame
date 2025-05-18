using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _groundObject;
    
    private List<Transform> _groundObjects;
    private bool _isStarted = false;

    private const float GroundSize = 60;//グラウンドの大きさは60
    
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    { 
        _isStarted = false;
        _= Initialize();
    }

    private async UniTask Initialize()
    {
        var result = await InstantiateAsync(_groundObject, 10, Vector3.zero, Quaternion.identity);
        _groundObjects = result.Select(o => o.transform).ToList();
        _isStarted = true;
    }

    private void FixedUpdate()
    {
        if (!_isStarted || !NetworkManager.Singleton.IsHost)return;
        
        _groundObjects[0].position += Vector3.back * (_speed * Time.fixedDeltaTime);

        for (int i = 1; i < _groundObjects.Count; i++)
        {
            _groundObjects[i].position = _groundObjects[0].position + Vector3.forward * (i * GroundSize);
        }
        
        if (_groundObjects[0].position.z < GroundSize * -1)
        {
            _groundObjects.Add(_groundObjects[0]);
            _groundObjects.RemoveAt(0);
        }
    }
}
