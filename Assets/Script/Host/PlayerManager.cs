using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private NetworkObject networkObject;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private HostPlayerAnimationManager _animationManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private MultiPlayRadioTower _multiPlayRadioTower;
    
    [SerializeField, Grouping] private PlayerData _playerData;
    
    private Vector3 _moveDirection = Vector3.zero;

    private Vector3 _animationDirection;

    private bool _jump;
    private bool _onGround;

    private bool _isHost;
    private GameObject _hostPlayerObject;


    private bool _gameStarted;

    private void Start()
    {
        _gameStarted = false;
    }

    public void GameStart()
    {
        _gameStarted = true;
        _moveDirection = new Vector3(0, 0, 10);
        _inputManager.OnMove += Move;
        _inputManager.OnJump += Jump;
        _inputManager.OnJumpEnd += JumpEnd;
        if (NetworkManager.Singleton.IsHost)
        {
            _isHost = true;
        }
        else
        {
            _isHost = false;
            _hostPlayerObject = GameObject.FindGameObjectWithTag("HostPlayer");
        }
    }
    
    private void FixedUpdate()
    {
        if (networkObject.IsOwner && _gameStarted)
        {
            _rigidbody.linearVelocity = new Vector3(_moveDirection.x, _rigidbody.linearVelocity.y, _moveDirection.z);
        }
    }

    private void Move(Vector2 inputDirection)
    {
        _moveDirection.x = inputDirection.x * _playerData.sideMoveSpeed;
        _animationDirection = new Vector2();
    }

    private void Jump()
    {
        if (!_onGround && !networkObject.IsOwner) return;
        _onGround = false;
        _rigidbody.AddForce(0,_playerData.jumpForce,0, ForceMode.Impulse);
    }

    private void JumpEnd()
    {
        if (_rigidbody.linearVelocity.y > 0 && !networkObject.IsOwner)
        {
            _rigidbody.linearVelocity = new Vector3(
                _rigidbody.linearVelocity.x,
                _rigidbody.linearVelocity.y / 2,
                _rigidbody.linearVelocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !_onGround)
        {
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
