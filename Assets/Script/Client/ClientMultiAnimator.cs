using Unity.Netcode;
using UnityEngine;

public class ClientMultiAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateBoolServerRPC(int hash, bool value)
    {
        Debug.Log("adf");
        animator.SetBool(hash, true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateFloatServerRPC(int hash, float value)
    {
        animator.SetFloat(hash, value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateTriggerServerRPC(int hash)
    {
        animator.SetTrigger(hash);
    }
}
