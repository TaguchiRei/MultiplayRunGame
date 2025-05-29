using Unity.Netcode;
using UnityEngine;

public class MultiPlayAnimation : NetworkBehaviour
{
    [SerializeField] Animator animator;


    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateBoolServerRpc(int hash, bool value)
    {
        Debug.Log("AnimationUpdateBoolServerRpc1111111111111111111111111111");
        AnimationUpdateBoolClientRpc(hash, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateFloatServerRpc(int hash, float value)
    {
        Debug.Log("AnimationUpdateFloatServerRpc22222222222222222222222222222");
        AnimationUpdateFloatClientRpc(hash, value);
    }
    
    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateBoolClientRpc(int hash, bool value)
    {
        Debug.Log("AnimationChange");
        animator.SetBool(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateFloatClientRpc(int hash, float value)
    {
        Debug.Log("AnimationChange");
        animator.SetFloat(hash, value);
    }

    [ClientRpc(RequireOwnership = false)]
    public void AnimationUpdateTriggerClientRpc(int hash)
    {
        Debug.Log("AnimationChange");
        animator.SetTrigger(hash);
    }
}
