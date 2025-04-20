using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class StartMultiPlayManager : MultiPlayManagerBase
{
    private static readonly int ConnectionSelect = Animator.StringToHash("ConnectionSelect");
    [SerializeField] private GameObject _loadingObject;

    [SerializeField] private GameObject _errorMessageField;
    private TextMeshProUGUI _errorMessage;
    
    [SerializeField] private Animator _StartCanvasAnimator;
    
    private UniTask<bool> _initialize;
    private UniTask<bool> _connectionHost; 
    private int _connectionPhase;
    
    public void Awake()
    {
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
                        _connectionPhase = 0;
                        _loadingObject.SetActive(false);
                        _StartCanvasAnimator.SetTrigger(ConnectionSelect);
                    }
                    else
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "オンラインサービスの初期化に失敗しました　\n ネット環境を確認してから再起動してください";
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
                }
                break;
            default:
                break;
        }
        if (_connectionPhase == 1)
        {

        }
    }

    public void ConnectionHost()
    {
        
         _connectionHost = HostConnect();
         _connectionPhase = 2;
    }
}
