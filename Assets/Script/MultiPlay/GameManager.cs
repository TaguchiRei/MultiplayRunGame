using Cysharp.Threading.Tasks;
using DG.Tweening;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MultiPlayManagerBase
{
    [SerializeField, ReadOnlyInInspector] InGameState _inGameState;
    [SerializeField] MultiPlayRadioTower _radioTower;
    [SerializeField] InputManager _inputManager;
    [SerializeField] GroundManager _groundManager;
     
    [SerializeField, Grouping] private NetworkObject _hostPlayerNetworkObject;
    [SerializeField, Grouping] private NetworkObject _clientPlayerNetworkObject;
    
    [SerializeField] TextMeshProUGUI _countdownText;
    
    [SerializeField] private Image _dmgEffect;
    [SerializeField] private Image _pointEffect;

    [SerializeField] private GameObject[] _titleObjects;

    [SerializeField] private GameObject _hitPointGage;
    [SerializeField] private Image _hitPointGageImage;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private int _maxHitPoint = 5;

    
    private bool _started = false;

    private int _score;
    private int _hitPoint;
    
    private bool _isSuvive = false;
    
    
    /// <summary>
    /// 直近のジャンプのタイミング。
    /// </summary>
    [ReadOnlyInInspector]public NetworkVariable<float> _latestJumpTime;
    
    public void HostConnection()
    {
        Debug.Log("HostConnection ManagerInitialize");
        _radioTower.OnMultiPlayDataReceived += MethodInvoker;
        _inGameState = InGameState.Waiting;
    }

    public void Start()
    {
        _score = 0;
        _isSuvive = true;
        _hitPoint = _maxHitPoint;
    }

    /// <summary>
    /// クライアント側で呼ばれる
    /// </summary>
    public void ClientConnection()
    {
        _ = WaitSpawn();
    }

    /// <summary>
    /// クライアント側で呼ばれる
    /// </summary>
    private async UniTask WaitSpawn()
    {
        _radioTower = FindAnyObjectByType<MultiPlayRadioTower>();
        await UniTask.WaitUntil(() => _radioTower.IsSpawned);
        _radioTower.OnMultiPlayDataReceived += MethodInvoker;
        _inGameState = InGameState.Client;
        _radioTower.Send(0);
    }
    
    

    private void Update()
    {
        if(!_started)return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Down");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) &&
                hit.collider.gameObject.CompareTag("WallObstacle") &&
                hit.collider.transform.root.TryGetComponent<Obstacle>(out var obstacle))
            {
                Debug.Log("Hit");
                obstacle.ObstacleHideClientRpc();
            }
        }
    }

    private void GetScore()
    {
        Debug.Log("Add Score");
        _score += 1200;
    }

    private void Damage()
    {
        if(!_isSuvive) return;
        Debug.Log("Damage");
        _hitPoint--;
        _hitPointGageImage.DOFillAmount((float)_hitPoint / _maxHitPoint,0.5f);
        if (_hitPoint <= 0)
        {
            _radioTower.SendBoth(5);
        }
    }

    public void Dead()
    {
        _isSuvive = false;
        Debug.Log("Dead");
        _scoreText.text = $"最終スコア : {_score}!";
        _titleObjects[0].SetActive(true);
        _scoreText.enabled = true;
    }

    public void MethodInvoker(MultiPlayData data ,int num)
    {
        Debug.Log("Invoker");
        switch (num)
        {
            case 0://ホスト側
                _inGameState = InGameState.Connecting;
                ulong ID = 0;
                foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    if (client.ClientId != NetworkManager.Singleton.LocalClientId)
                    {
                        ID = client.ClientId;
                        break;
                    }
                }
                if(ID == 0)return;
                _clientPlayerNetworkObject.ChangeOwnership(ID);
                _radioTower.Send(1);
                Debug.Log("Connecting");
                break;
            case 1://クライアント側
                Debug.Log("WaitChangeOwner");
                _ = WaitChangeOwner();
                break;
            case 2:
                Debug.Log("StartCountDown");
                _ = StartCountDown();
                if(NetworkManager.Singleton.IsHost)
                    _radioTower.Send(2);
                break;
            case 3://両者
                GetScore();
                ShowEffect(_pointEffect);
                break;
            case 4:
                Damage();
                ShowEffect(_dmgEffect);
                break;
            case 5:
                Dead();
                break;
            default:
                break;
        }
    }
    
    private async UniTask WaitChangeOwner()
    {
        await UniTask.WaitUntil(() => _clientPlayerNetworkObject.IsOwner);
        Debug.Log($"Owner Changed {_clientPlayerNetworkObject.IsOwner}");
        _radioTower.Send(2);
    }

    private async UniTask StartCountDown()
    {
        _groundManager.GameStart();
        _countdownText.gameObject.SetActive(true);
        for (int i = 5; i >= 0; i--)
        {
            await UniTask.WaitForSeconds(1);
            _countdownText.text = i.ToString();
        }
        _countdownText.gameObject.SetActive(false);
        _hitPointGage.SetActive(true);
        
        if(NetworkManager.Singleton.IsHost)
            _hostPlayerNetworkObject.gameObject.GetComponent<MultiPlayInput>().GameStart();
        else
            _clientPlayerNetworkObject.gameObject.GetComponent<MultiPlayInput>().GameStart();
        _inputManager.GameStart();
        foreach (var obj in _titleObjects)
        {
            obj.SetActive(false);
        }
        _started = true;
    }

    public void GameEnd()
    {
        Application.Quit();
    }
    
    
    private void ShowEffect(Image effect)
    {
        effect.color = new Color(effect.color.r, effect.color.g, effect.color.b, 1f);
        effect.DOFade(0f, 1f)
            .SetEase(Ease.OutQuad);
    }

    public enum InGameState
    {
        Waiting,
        Connecting,
        Playing,
        Client,
    }
}
