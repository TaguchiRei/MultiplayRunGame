using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GamesKeystoneFramework.Save;

public class SaveDataTest : MonoBehaviour
{
    UniTask _saveDataTask;

    private void Start()
    {
        
    }
}

[Serializable]
public class PlayerSaveData : SaveDataBase<PlayerSaveData>
{
    public int PlayerLevel = 1;
    public string PlayerName = "Player";

    protected override PlayerSaveData Initialize()
    {
        return new PlayerSaveData
        {
            PlayerLevel = 1,
            PlayerName = "New Player"
        };
    }
}