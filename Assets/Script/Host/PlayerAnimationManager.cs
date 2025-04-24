using System;
using GamesKeystoneFramework.Attributes;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int FB = Animator.StringToHash("FB");
    private static readonly int LR = Animator.StringToHash("LR");

    [SerializeField] GameObject _player;
    
    [SerializeField] InputManager _inputManager;
    private Animator _animator;
    
    [ReadOnlyInInspector] public bool SlowMotion;

    private void Start()
    {
        _animator = _player.GetComponent<Animator>();
    }

    public void LRFBUpdate(Vector2 moveVector)
    {
        _animator.SetFloat(FB, 1);
        _animator.SetFloat(LR, moveVector.x);
    }

    public void AnimationStart()
    {
        _animator.SetBool(Move, true);
        _animator.SetBool(Run, true);
        _inputManager.OnJump += StartJump;
        _inputManager.OnMove += LRFBUpdate;
    }


    private void StartJump()
    {
        _animator.SetBool(Jump, true);
    }

    public void EndJump()
    {
        _animator.SetBool(Jump, false);
    }
}