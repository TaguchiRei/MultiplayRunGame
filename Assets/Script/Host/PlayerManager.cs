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

    private Vector3 _moveDirection = Vector3.zero;
    private float _moveLR;
    private bool _jump;
    private bool _onGround;
    private bool _isHost;
    private bool _gameStarted;
    private bool _jumping;

    [SerializeField] private Vector3 _defaultGravity;
    [SerializeField] private Vector3 _fallingGravity;

    private void Start()
    {
        _gameStarted = false;
        _onGround = true;
    }

    public void GameStart()
    {
        Debug.Log($"Game Started{gameObject.name}");
        Debug.Log($"networkObject : {networkObject.IsOwner}");
        _animationManager.AnimationStart();
        _cameraScript.SetCamera(gameObject);
        _gameStarted = true;

        _moveDirection = new Vector3(0, 0, 10);
        _moveLR = 0;

        _inputManager.OnMove += Move;
        _inputManager.OnMoveEnd += MoveEnd;
        _inputManager.OnJump += Jump;
        _isHost = NetworkManager.Singleton.IsHost;
        _inputManager.GameStart();
    }

    private void FixedUpdate()
    {
        if (networkObject.IsOwner && _gameStarted)
        {
            _rigidbody.linearVelocity =
                new Vector3(_moveLR * _playerData.sideMoveSpeed, _rigidbody.linearVelocity.y, _moveDirection.z);
        }

        _rigidbody.AddForce(_rigidbody.linearVelocity.y >= 0 ? _defaultGravity : _fallingGravity);
    }

    private void Update()
    {
        if (_jumping)
        {
            _rigidbody.AddForce(Vector3.up * _playerData.jumpForce, ForceMode.Impulse);
            _jumping = false;
            if (_rigidbody.linearVelocity.y < 0)
            {
            }
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

    [Serializable]
    private struct PlayerData
    {
        public float moveSpeed;
        public float sideMoveSpeed;
        public float jumpForce;
    }
}