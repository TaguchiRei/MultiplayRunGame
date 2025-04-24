using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject _targetObj;

    private Vector3 _relativePosition;
    
    private bool _gameStarted = false;
    
    private void Start() => _gameStarted = false;
    
    public void SetCamera(GameObject target)
    {
        _targetObj = target;
        _relativePosition = target.transform.position - transform.position;
        _gameStarted = true;
    }

    private void FixedUpdate()
    {
        if(!_gameStarted) return;
        var targetPos = _targetObj.transform.position - _relativePosition;
        transform.position = new Vector3(0, transform.position.y, targetPos.z);
    }
}
