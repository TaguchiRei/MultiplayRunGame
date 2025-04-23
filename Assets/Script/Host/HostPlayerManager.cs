using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HostPlayerManager : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private HostPlayerAnimationManager _animationManager;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private MultiPlayRadioTower _multiPlayRadioTower;
    
    [SerializeField, Grouping] private PlayerData _playerData;
    
    private Vector3 _moveDirection = Vector3.zero;

    private bool _jump;
    private bool _onGround;
    
    /// <summary>
    /// 直近のジャンプのタイミング。
    /// </summary>
    [ReadOnlyInInspector]public NetworkVariable<float> _latestJumpTime;
    
    [HideInInspector] public HostPlayerManager _hostPlayer;

    /// <summary>
    /// テストプレイ用、
    /// </summary>
    private void Start()
    {
        GameStart();
    }

    void GameStart()
    {
        _moveDirection = new Vector3(0, 0, 10);
        _inputManager.OnMove += Move;
        _inputManager.OnJump += Jump;
        _inputManager.OnJumpEnd += JumpEnd;
    }
    
    private void FixedUpdate()
    {
        if (NetworkManager.Singleton.IsHost)//ホストの場合自分で動かす
        {
            _rigidbody.linearVelocity = new Vector3(_moveDirection.x, _rigidbody.linearVelocity.y, _moveDirection.z);
        }
    }

    private void Move(Vector2 inputDirection)
    {
        _moveDirection.x = inputDirection.x * _playerData.sideMoveSpeed;
    }

    private void Jump()
    {
        if (!_onGround) return;
        _onGround = false;
        _rigidbody.AddForce(0,_playerData.jumpForce,0, ForceMode.Impulse);
        _latestJumpTime.Value = (float)NetworkManager.Singleton.ServerTime.Time;
    }

    private void JumpEnd()
    {
        if (_rigidbody.linearVelocity.y > 0)
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
