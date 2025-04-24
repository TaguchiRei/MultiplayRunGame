using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject targetObj;

    private Vector3 _relativePosition;
    
    private bool _gameStarted = false;
    
    private void Start() => _gameStarted = false;
    
    public void SetCamera(GameObject target)
    {
        _relativePosition = target.transform.position - transform.position;
    }

    private void FixedUpdate()
    {
        if(!_gameStarted) return;
        transform.position = targetObj.transform.position + _relativePosition;
    }
}
