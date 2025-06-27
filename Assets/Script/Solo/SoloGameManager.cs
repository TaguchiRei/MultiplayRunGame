using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloGameManager : MonoBehaviour
{
    public static bool IsSoloMode = false;

    private int _score;
    private int _hitPoint;
    private bool _isSurvive;
    [SerializeField] private int _maxHitPoint = 5;
    [SerializeField] private Image _hitPointGageImage;

    [SerializeField] private GameObject[] _titleObjects;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private GroundManager _groundManager;
    

    private void Start()
    {
        IsSoloMode = true;
        _groundManager.GameStart();
    }

    private void GameStart()
    {
        
    }
    
    private void GetScore()
    {
        Debug.Log("Add Score");
        _score += 856;//スコアを乱雑な数値にしてそれらしく
    }

    private void Damage()
    {
        if(!_isSurvive) return;
        Debug.Log("Damage");
        _hitPoint--;
        _hitPointGageImage.DOFillAmount((float)_hitPoint / _maxHitPoint, 0.5f);
        if (_hitPoint <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        _isSurvive = false;
        Debug.Log("Dead");
        _scoreText.text = $"最終スコア : {_score}!";
        _titleObjects[0].SetActive(true);
        _scoreText.enabled = true;
    }
    private async UniTask StartCountDown()
    {
        _countdownText.enabled = true;
        for (int i = 3; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            await UniTask.Delay(1000);
        }
        _countdownText.text = "Start!";
        await UniTask.Delay(1000);
        _countdownText.enabled = false;
    }
}
