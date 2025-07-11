using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class MultiPlayInput : MonoBehaviour
{
    private static readonly int FB = Animator.StringToHash("FB");
    private static readonly int LR = Animator.StringToHash("LR");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [SerializeField, Grouping] private MultiPlayNeedComponents _multiPlayNeedComponents;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private InputManager _inputManager;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpForce;

    [SerializeField] private Vector3 _defaultGravity;
    [SerializeField] private Vector3 _fallingGravity;

    private GameManager _gameManager;

    private bool _gameStarted;
    private bool _onGround;
    private Vector3 _moveDirection;

    public bool OnGround
    {
        get => _onGround;
    }

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameStarted = false;
        _onGround = true;
    }

    private void FixedUpdate()
    {
        if(!_gameStarted || !_multiPlayNeedComponents.NetworkObject.IsOwner) return;
        if (_rigidbody.linearVelocity.y > 2)
        {
            _rigidbody.AddForce(_defaultGravity, ForceMode.Acceleration);
        }
        else
        {
            _rigidbody.AddForce(_fallingGravity, ForceMode.Acceleration);
        }

        _rigidbody.linearVelocity = new Vector3(_moveDirection.x, _rigidbody.linearVelocity.y, 0);
    }

    public void GameStart()
    {
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatServerRpc(FB, 1f);

        _inputManager.OnMove += MoveDirectionUpdate;
        _inputManager.OnMoveEnd += StopMovement;
        _inputManager.OnJump += JumpInput;
        
        _gameStarted = true;
    }

    /// <summary>
    /// 移動に使用する
    /// </summary>
    /// <param name="inputVector"></param>
    private void MoveDirectionUpdate(Vector2 inputVector)
    {
        Debug.Log("Move");
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatServerRpc(LR, inputVector.x);
        _moveDirection = new Vector3(inputVector.x, 0, 0) * _moveSpeed;
    }

    /// <summary>
    /// 移動を止めるために使用する
    /// </summary>
    private void StopMovement()
    {
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatServerRpc(LR, 0);
        _moveDirection = Vector3.zero;
    }

    private void JumpInput()
    {
        if (_onGround)
        {
            _multiPlayNeedComponents.MultiPlayAnimation
                .AnimationUpdateBoolServerRpc(Jump, true);
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _onGround = false;
        }
    }

    [Serializable]
    private struct MultiPlayNeedComponents
    {
        public NetworkObject NetworkObject;
        public MultiPlayAnimation MultiPlayAnimation;
        public MultiPlayRadioTower MultiPlayRadioTower;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _onGround = true;
            _multiPlayNeedComponents.MultiPlayAnimation
                .AnimationUpdateBoolServerRpc(Jump, false);
        }
    }
}