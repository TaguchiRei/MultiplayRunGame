using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using UnityEngine;

public class StartMultiPlayManager : MultiPlayManagerBase
{
    private static readonly int ConnectionSelect = Animator.StringToHash("ConnectionSelect");
    [SerializeField] private GameObject _loadingObject;
    private UniTask<bool> _initialize;
    private int _connectionPhase;
    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _errorMessageField;
    private TextMeshProUGUI _errorMessage;
    
    public void Awake()
    {
        _errorMessage = _errorMessageField.GetComponentInChildren<TextMeshProUGUI>();
        _loadingObject.SetActive(true);
        _connectionPhase = 0;
        _initialize = ServicesInitialize();
    }

    void Update()
    {
        switch (_connectionPhase)
        {
            case 0:
                var awaiter = _initialize.GetAwaiter();
                if (awaiter.IsCompleted)
                {
                    var result = awaiter.GetResult();
                    if (result)
                    {
                        _connectionPhase = 1;
                        _loadingObject.SetActive(false);
                        _animator.SetTrigger(ConnectionSelect);
                    }
                    else
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "オンラインサービスの初期化に失敗しました　\n ネット環境を確認してから再起動してください";
                    }
                }

                break;
            case 1:
                break;
        }
    }
}
