using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloGameManager : MonoBehaviour
{
    private int _score;
    private int _hitPoint;
    private bool _isSurvive;
    [SerializeField] private int _maxHitPoint = 5;
    [SerializeField] private Image _hitPointGageImage;
    [SerializeField] private GameObject _hitPointGage;

    [SerializeField] private GameObject[] _titleObjects;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private GroundManager _groundManager;
    
    [SerializeField] private SoloPlayerManager _soloPlayerManager;
    [SerializeField] private InputManager _inputManager;
    
    private void Start()
    {
        _groundManager.GameStart(false);
        _inputManager.GameStart();
        _ = StartCountDown();
    }
    
    public void GetScore()
    {
        Debug.Log("Add Score");
        _score += 856;//スコアを乱雑な数値にしてそれらしく
    }

    public void Damage()
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
        for (int i = 5; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            await UniTask.WaitForSeconds(1f);
        }
        _countdownText.enabled = false;
        _hitPointGage.SetActive(true);
        foreach (var obj in _titleObjects)
        {
            obj.SetActive(false);
        }
        _soloPlayerManager.GameStart();
    }
}
