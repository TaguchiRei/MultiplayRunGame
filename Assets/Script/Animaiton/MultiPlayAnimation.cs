using Unity.Netcode;
using UnityEngine;

public class MultiPlayAnimation : NetworkBehaviour
{
    [SerializeField] Animator animator;


    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateBoolServerRpc(int hash, bool value)
    {
        AnimationUpdateBoolClientRpc(hash, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateFloatServerRpc(int hash, float value)
    {
        AnimationUpdateFloatClientRpc(hash, value);
    }
    
    [ClientRpc(RequireOwnership = false)]
    private void AnimationUpdateBoolClientRpc(int hash, bool value)
    {
        animator.SetBool(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    private void AnimationUpdateFloatClientRpc(int hash, float value)
    {
        animator.SetFloat(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateTriggerClientRpc(int hash)
    {
        animator.SetTrigger(hash);
    }
}
