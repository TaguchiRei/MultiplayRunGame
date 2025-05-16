using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private NetworkObject networkObject;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerAnimationManager _animationManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private MultiPlayRadioTower _multiPlayRadioTower;

    [SerializeField, Grouping] private PlayerData _playerData;

    [SerializeField] private CameraScript _cameraScript;
    
    private GameManager _gameManager;

    private Vector3 _moveDirection = Vector3.zero;
    private float _moveLR;
    private bool _jump;
    private bool _onGround;
    private bool _gameStarted;
    private bool _jumping;
    private bool _isHost;

    [SerializeField] private Vector3 _defaultGravity;
    [SerializeField] private Vector3 _fallingGravity;
    
    [SerializeField] Transform _hostTransform;
    [SerializeField] private GameObject _clientModel;

    private void Start()
    {
        _gameStarted = false;
        _onGround = true;
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    public void GameStart()
    {
        Debug.Log($"Game Started{gameObject.name}");
        Debug.Log($"networkObject : {networkObject.IsOwner}");
        _animationManager.AnimationStart();
        _cameraScript.SetCamera(gameObject);
        _gameStarted = true;
        _isHost = NetworkManager.Singleton.IsHost;

        _moveDirection = new Vector3(0, 0, _playerData.moveSpeed);
        _moveLR = 0;

        _inputManager.OnMove += Move;
        _inputManager.OnMoveEnd += MoveEnd;
        _inputManager.OnJump += Jump;
        _inputManager.GameStart();
    }

    private void Update()
    {
        if (_jumping)
        {
            _rigidbody.AddForce(Vector3.up * _playerData.jumpForce, ForceMode.Impulse);
            _jumping = false;
        }
    }

    private void Move(Vector2 inputDirection)
    {
        _moveLR = inputDirection.x;
    }

    private void MoveEnd()
    {
        _moveLR = 0;
    }

    private void Jump()
    {
        Debug.Log($"Jump : {_onGround}");
        if (!_onGround) return;
        _onGround = false;
        _jumping = true;
        _animationManager.StartJump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !_onGround)
        {
            Debug.Log("OnGround");
            _animationManager.EndJump();
            _onGround = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HostBarrier") && NetworkManager.Singleton.IsHost)
        {
            _multiPlayRadioTower.Send(_isHost ? 3 : 4);
            _gameManager.MethodInvoker(default,_isHost ? 3 : 4);
        }
        else if (other.gameObject.CompareTag("ClientBarrier") && NetworkManager.Singleton.IsClient)
        {
            _multiPlayRadioTower.Send(!_isHost ? 3 : 4);
            _gameManager.MethodInvoker(default,!_isHost ? 3 : 4);
        }
    }

    [Serializable]
    private struct PlayerData
    {
        public float moveSpeed;
        public float sideMoveSpeed;
        public float jumpForce;
    }
}