using System;
using GamesKeystoneFramework.MultiPlaySystem;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientGameManager : MultiPlayManagerBase
{
    MultiPlayRadioTower _radioTower;
    
    private void Start()
    {
        _radioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        _radioTower.Send(0);
    }
}
