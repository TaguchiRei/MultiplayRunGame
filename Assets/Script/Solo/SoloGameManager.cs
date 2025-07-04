using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] private Image _scoreEffect;
    [SerializeField] private Image _dmgEffect;
    
    Tween _pointTween;
    Tween _dmgTween;

    private void Start()
    {
        _groundManager.GameStart(false);
        _inputManager.GameStart();
        _ = StartCountDown();
        _isSurvive = true;
        _hitPoint = _maxHitPoint;
    }

    public void GetScore()
    {
        if (!_isSurvive) return;
        Debug.Log("Add Score");
        _score += 356; //スコアを乱雑な数値にしてそれらしく
        ShowEffect(_scoreEffect);
    }

    public void Damage()
    {
        if (!_isSurvive) return;
        Debug.Log("Damage");
        _hitPoint--;
        _hitPointGageImage.DOFillAmount((float)_hitPoint / _maxHitPoint, 0.5f);
        ShowEffect(_dmgEffect);
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


    private void ShowEffect(Image effect)
    {
        if (effect == _dmgEffect)
        {
            _dmgTween?.Kill();
            _dmgEffect.color = new Color(1f, 0f, 0f, 1f); // 赤、完全表示
            _dmgTween = _dmgEffect.DOFade(0f, 1f).SetEase(Ease.OutQuad);
        }
        else if (effect == _scoreEffect)
        {
            _pointTween?.Kill();
            _scoreEffect.color = new Color(0f, 1f, 0f, 1f); // 緑、完全表示
            _pointTween = _scoreEffect.DOFade(0f, 1f).SetEase(Ease.OutQuad);
        }
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

    public void GameEnd()
    {
#if UNITY_WEBGL
        SceneManager.LoadScene("StartScene");
#else
        Application.Quit();
#endif
    }

    public void ReTry()
    {
        SceneManager.LoadScene("SoloScene");
    }
}