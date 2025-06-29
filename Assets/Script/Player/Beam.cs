using System;
using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private MeshRenderer  _meshRenderer;

    public Vector3 AimPosition;
    private void Update()
    {
        var dir = AimPosition - _player.transform.position;
        transform.RotateAround(_player.transform.position, dir.normalized, Time.deltaTime);
    }

    private void BeamShowHide(bool show)
    {
        _meshRenderer.enabled = show;
    }
}
