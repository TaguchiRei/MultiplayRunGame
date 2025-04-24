using System;
using GamesKeystoneFramework.Attributes;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");

    [SerializeField] GameObject _player;

    private Animator _animator;
    
    [ReadOnlyInInspector] public bool SlowMotion;

    private void Start()
    {
        _animator = _player.GetComponent<Animator>();
    }

    private void AnimationStart()
    {
        _animator.SetBool(Move, true);
    }
    

    public void RunStart()
    {
        _animator.SetBool(Run, true);
    }
    

    public void StartJump()
    {
        _animator.SetBool(Jump, true);
    }

    public void EndJump()
    {
        _animator.SetBool(Jump, false);
    }
}