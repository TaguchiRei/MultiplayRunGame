using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class MultiPlayInput : MonoBehaviour
{
    private static readonly int FB = Animator.StringToHash("FB");
    private static readonly int LR = Animator.StringToHash("LR");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    
    [SerializeField, Grouping] private MultiPlayNeedComponents _multiPlayNeedComponents;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private InputManager _inputManager;
    
    [SerializeField] private Vector3 _defaultGravity;
    [SerializeField] private Vector3 _fallingGravity;
    
    private GameManager _gameManager;

    private bool _gameStarted;
    private bool _onGround;

    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _gameStarted = false;
        _onGround = true;
    }

    public void GameStart()
    {
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateBoolServerRpc(Run, true);
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatServerRpc(FB, 1f);

        _inputManager.OnMove += MoveDirectionUpdate;
        _inputManager.OnMoveEnd += StopMovement;
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
        _rigidbody.linearVelocity = new Vector3(inputVector.x, 0, 0);
    }

    /// <summary>
    /// 移動を止めるために使用する
    /// </summary>
    private void StopMovement()
    {
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatServerRpc(LR, 0);
        _rigidbody.linearVelocity = Vector3.zero;
    }
    

    [Serializable]
    private struct MultiPlayNeedComponents
    {
        public NetworkObject NetworkObject;
        public MultiPlayAnimation MultiPlayAnimation;
        public MultiPlayRadioTower MultiPlayRadioTower;
    }
}