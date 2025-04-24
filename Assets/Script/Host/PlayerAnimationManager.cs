using System;
using GamesKeystoneFramework.Attributes;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private static readonly int FB = Animator.StringToHash("FB");
    private static readonly int LR = Animator.StringToHash("LR");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [SerializeField] GameObject _player;
    
    [SerializeField] InputManager _inputManager;
    [SerializeField] private Animator _animator;
    
    [SerializeField] private ClientMultiAnimator _clientMultiAnimator;
    
    [ReadOnlyInInspector] public bool SlowMotion;

    private NetworkAnimator _networkAnimator;
    

    public void AnimationStart()
    {
        _animator = _player.GetComponent<Animator>();
        _animator.SetBool(Run, true);
        _animator.SetFloat(FB, 1);

        if (!NetworkManager.Singleton.IsServer)
        {
            _clientMultiAnimator.AnimationUpdateBoolServerRPC(Run, true);
            _clientMultiAnimator.AnimationUpdateFloatServerRPC(FB, 1);
        }

        _inputManager.OnMove += LRFBUpdate;
        _inputManager.OnMoveEnd += OnMoveEnd;
    }

    /// <summary>
    /// 走るアニメーションの動きを制御
    /// </summary>
    /// <param name="moveVector"></param>
    private void LRFBUpdate(Vector2 moveVector)
    {
        _animator.SetFloat(FB, 1);
        _animator.SetFloat(LR, moveVector.x);

        if (!NetworkManager.Singleton.IsServer)
        {
            _clientMultiAnimator.AnimationUpdateFloatServerRPC(FB, 1);
            _clientMultiAnimator.AnimationUpdateFloatServerRPC(LR, moveVector.x);
            Debug.Log("Movement Client");
        }
        
        Debug.Log("Move");
    }

    private void OnMoveEnd()
    {
        _animator.SetFloat(LR, 0);
        if(!NetworkManager.Singleton.IsServer)
            _clientMultiAnimator.AnimationUpdateFloatServerRPC(LR, 0);
    }

    public void StartJump()
    {
        _animator.SetBool(Jump,true);
        if (!NetworkManager.Singleton.IsServer)
            _clientMultiAnimator.AnimationUpdateBoolServerRPC(Jump, true);
    }

    public void EndJump()
    {
        _animator.SetBool(Jump,false);
        if(!NetworkManager.Singleton.IsServer)
            _clientMultiAnimator.AnimationUpdateBoolServerRPC(Jump, false);
    }
}