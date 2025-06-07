using System;
using GamesKeystoneFramework.Attributes;
using UnityEngine;

public class BarrierObject : MonoBehaviour
{
    [SerializeField,Grouping] private bool _isHostBarrier;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("HostBarrier") == _isHostBarrier)
        {
            
        }
    }
}
