using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

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
            .AnimationUpdateBoolClientRpc(Run, true);
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatClientRpc(FB, 1);

        _inputManager.OnMove += MoveDirectionUpdate;
    }

    private void MoveDirectionUpdate(Vector2 inputVector)
    {
        _multiPlayNeedComponents.MultiPlayAnimation
            .AnimationUpdateFloatClientRpc(LR, inputVector.x);
    }
    

    [Serializable]
    private struct MultiPlayNeedComponents
    {
        public NetworkObject NetworkObject;
        public MultiPlayAnimation MultiPlayAnimation;
        public MultiPlayRadioTower MultiPlayRadioTower;
    }
}