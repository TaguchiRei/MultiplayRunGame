using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MultiPlayAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    
    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateBoolClientRpc(int hash, bool value)
    {
        animator.SetBool(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateFloatClientRpc(int hash, float value)
    {
        animator.SetFloat(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateTriggerClientRpc(int hash)
    {
        animator.SetTrigger(hash);
    }
}
