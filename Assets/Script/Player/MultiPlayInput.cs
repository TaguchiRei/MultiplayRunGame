using System;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using UnityEngine;

public class MultiPlayInput : MonoBehaviour
{
    [SerializeField, Grouping] private MultiPlayNeedComponents multiPlayNeedComponents;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private InputManager _inputManager;
    

    [Serializable]
    private struct MultiPlayNeedComponents
    {
        public NetworkObject NetworkObject;
        public MultiPlayAnimation MultiPlayAnimation;
        public MultiPlayRadioTower MultiPlayRadioTower;
    }
}