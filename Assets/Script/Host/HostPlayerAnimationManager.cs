using System;
using UnityEngine;

public class HostPlayerAnimationManager : MonoBehaviour
{
    private static readonly int StartRun = Animator.StringToHash("Start");
    private static readonly int SlidingStart = Animator.StringToHash("SlidingStart");
    private static readonly int SlidingEnd = Animator.StringToHash("SlidingEnd");
    private static readonly int JumpEnd = Animator.StringToHash("JumpEnd");
    private static readonly int JumpStart = Animator.StringToHash("JumpStart");
    [SerializeField] GameObject _player;

    private Animator _animator;

    private void Start()
    {
        _animator = _player.GetComponent<Animator>();
    }

    public void RunStart()
    {
        _animator.SetTrigger(StartRun);
        
    }

    public void StartSliding()
    {
        _animator.SetTrigger(SlidingStart);
    }

    public void EndSliding()
    {
        _animator.SetTrigger(SlidingEnd);
    }

    public void StartJump()
    {
        _animator.SetTrigger(JumpStart);
    }

    public void EndJump()
    {
        _animator.SetTrigger(JumpEnd);
    }
}