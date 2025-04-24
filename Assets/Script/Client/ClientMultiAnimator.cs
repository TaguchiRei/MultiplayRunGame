using Unity.Netcode;
using UnityEngine;

public class ClientMultiAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateBool(int hash, bool value)
    {
        animator.SetBool(hash, true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AnimationUpdateFloat(int hash, int value)
    {
        animator.SetFloat(hash, value);
    }
}
