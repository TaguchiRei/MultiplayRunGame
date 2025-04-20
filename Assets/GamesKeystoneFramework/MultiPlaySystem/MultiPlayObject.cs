using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public class MultiPlayObject : NetworkBehaviour
    {
        [SerializeField] protected Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody.isKinematic = false;
        }

        private void OnDrawGizmos()
        {
            Vector3 pos = transform.position;
            Gizmos.DrawIcon(pos, "CommunicationMark.png", true);
        }
    }
}
