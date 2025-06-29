using System;
using Unity.Netcode;
using UnityEngine;

public class Beam : NetworkBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private MeshRenderer _meshRenderer;

    public Vector3 AimPosition;

    private void Update()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        transform.position = _player.transform.position;
        Vector3 direction = (AimPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
        transform.rotation = rotation;
    }

    [ClientRpc]
    public void BeamShowHideClientRPC(bool show)
    {
        _meshRenderer.enabled = show;
    }
}