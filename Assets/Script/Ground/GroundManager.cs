using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int _groundCount;
    [SerializeField] private float _speed;
    [SerializeField, Range(-10, -1)] private int _groundReturnPoint;
    [SerializeField] private GameObject _groundObject;
    [SerializeField] private int _obstacleSpawnTiming;
    [SerializeField] private GameObject[] _obstacleObjects;
    [SerializeField] private Vector3 _obstaclePool;

    private List<Ground> _groundObjects;
    private List<Transform> _obstacleTransforms;

    private int _obstacleSpawnCounter;
    private bool _isStarted;
    private const float GroundSize = 29; //グラウンドの大きさは60

    public void GameStart()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _isStarted = false;
            _ = Initialize();
        }
    }

    private async UniTask Initialize(bool isMulti = true)
    {
        _obstacleTransforms = new ();
        var groundResult = await InstantiateAsync(_groundObject, _groundCount, Vector3.zero, Quaternion.identity);
        _groundObjects = groundResult.Select(o => o.GetComponent<Ground>()).ToList();
        _isStarted = true;
        for (int i = 0; i < _groundObjects.Count; i++)
        {
            _groundObjects[i].gameObject.transform.position = Vector3.forward * (i * GroundSize);
            if(isMulti) _groundObjects[i].GetComponent<NetworkObject>().Spawn();
        }

        //障害物のオブジェクトを生成
        foreach (var obstacle in _obstacleObjects)
        {
            for (int i = 0; i < 3; i++)
            {
                var obstacleObj = Instantiate(obstacle, _obstaclePool, Quaternion.identity); 
                if(isMulti) obstacleObj.GetComponent<NetworkObject>().Spawn();
                _obstacleTransforms.Add(obstacleObj.transform);
            }
        }
        //障害物をFisher-Yatesを利用してシャッフル
        for (int i = _obstacleTransforms.Count-1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (_obstacleTransforms[i], _obstacleTransforms[randomIndex]) = 
                (_obstacleTransforms[randomIndex], _obstacleTransforms[i]);
        }
    }

    private void FixedUpdate()
    {
        if (!_isStarted) return;
        //一番手前の地面の位置を決定
        _groundObjects[0].gameObject.transform.position += Vector3.back * (_speed * Time.fixedDeltaTime);

        //一番手前の地面の位置を基準に地面の大きさづつずらした位置に他の地面を再配置
        for (int i = 1; i < _groundObjects.Count; i++)
        {
            _groundObjects[i].gameObject.transform.position = 
                _groundObjects[0].gameObject.transform.position + Vector3.forward * (i * GroundSize);
            //地面が障害物を保持していた場合それも移動させる
            if (_groundObjects[i].Obstacle != null)
            {
                _groundObjects[i].Obstacle.position = _groundObjects[i].gameObject.transform.position;
            }
        }

        //地面がカメラより後ろにあった場合一番奥に移動させる
        if (_groundObjects[0].gameObject.transform.position.z < GroundSize * _groundReturnPoint)
        {
            //地面が障害物を保持していた場合、それを解除する
            _groundObjects[0].Obstacle = null;
            
            //障害物を出現させるタイミングだった場合は障害物を地面に登録する
            if (_obstacleSpawnCounter >= _obstacleSpawnTiming)
            {
                _obstacleSpawnCounter = 0;
                _groundObjects[0].Obstacle = _obstacleTransforms[0];
                var temp = _obstacleTransforms[0];
                _obstacleTransforms.RemoveAt(0);
                _obstacleTransforms.Add(temp);
            }
            else
            {
                _obstacleSpawnCounter++;
            }
            _groundObjects.Add(_groundObjects[0]);
            _groundObjects.RemoveAt(0);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_obstaclePool, 2);
    }

    private struct WallObject
    {
        public Transform TargetTransform;
        public GameObject WallGameObject;
    }
}