using System;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMultiPlayManager : MultiPlayManagerBase
{
    private static readonly int ConnectionSelect = Animator.StringToHash("ConnectionSelect");
    [SerializeField] private GameObject _loadingObject;

    [SerializeField] private GameObject _errorMessageField;
    private TextMeshProUGUI _errorMessage;
    
    [SerializeField] private Animator _StartCanvasAnimator;
    
    [SerializeField, Grouping] Buttons _buttons;
    
    private UniTask<bool> _initialize;
    private UniTask<bool> _connectionHost; 
    private int _connectionPhase;
    
    public void Awake()
    {
        _buttons.DisableButtons();
        _errorMessage = _errorMessageField.GetComponentInChildren<TextMeshProUGUI>();
        _loadingObject.SetActive(true);
        _connectionPhase = 0;
        _initialize = ServicesInitialize();
        Debug.Log("MultiPlayManagerBase.Awake");
    }

    void Update()
    {
        switch (_connectionPhase)
        {
            case 1:
                var initializeAwaiter = _initialize.GetAwaiter();
                if (initializeAwaiter.IsCompleted)
                {
                    var result = initializeAwaiter.GetResult();
                    if (result)
                    {
                        _buttons.EnableButtons();
                        _connectionPhase = 0;
                        _loadingObject.SetActive(false);
                        _StartCanvasAnimator.SetTrigger(ConnectionSelect);
                    }
                    else
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "オンラインサービスの初期化に失敗しました　\n ネット環境を確認してください";
                    }
                }
                break;
            case 2:
                var connectionHostAwaiter = _connectionHost.GetAwaiter();
                if (connectionHostAwaiter.IsCompleted)
                {
                    var result = connectionHostAwaiter.GetResult();
                    if (result)
                    {
                        SceneManager.LoadScene("HostScene");
                    }
                    else
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "ルーム作成に失敗しました。　\n ネット環境を確認してください";
                    }
                }
                break;
            case 3:
                break;
            default:
                break;
        }
    }
    
    public void ConnectionHost()
    {
        _buttons.DisableButtons();
        _connectionHost = HostConnect();
        _connectionPhase = 2;
    }
    

    [Serializable]
    struct Buttons
    {
        public Button RunnnerStartButton;
        public Button SupporterStartButton;

        public Button RandomConnectionButton;
        public Button SearchConnectionButton;

        public void EnableButtons()
        {
            RunnnerStartButton.enabled = true;
            SupporterStartButton.enabled = true;
            RandomConnectionButton.enabled = true;
            SearchConnectionButton.enabled = true;
        }

        public void DisableButtons()
        {
            RunnnerStartButton.enabled = false;
            SupporterStartButton.enabled = false;
            RandomConnectionButton.enabled = false;
            SearchConnectionButton.enabled = false;
        }
    }
}
