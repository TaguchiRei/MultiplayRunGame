using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamesKeystoneFramework.Attributes;
using GamesKeystoneFramework.MultiPlaySystem;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StartMultiPlayManager : MultiPlayManagerBase
{
    private static readonly int ConnectionSelect = Animator.StringToHash("ConnectionSelect");
    private static readonly int ClientConnection = Animator.StringToHash("ClientConnection");
    private static readonly int HostConnection = Animator.StringToHash("HostConnection");
    [SerializeField] private GameObject _loadingObject;

    [SerializeField] private GameObject _errorMessageField;
    private TextMeshProUGUI _errorMessage;

    [SerializeField] private Animator _StartCanvasAnimator;

    [SerializeField, Grouping] Buttons _buttons;

    private UniTask<bool> _initialize;
    private UniTask<bool> _connectionHost;
    private UniTask<bool> _connectionClient;
    private UniTask<(bool, Lobby)> _lobbyCheck;
    private UniTask<(bool, List<Lobby>)> _connectionClientRandom;

    private int _connectionPhase;
    
    [SerializeField] GameManager _gameManager;

    private float _checkLateTimer;

    public void Awake()
    {
        _checkLateTimer = 0;
        _buttons.DisableButtons();
        _errorMessage = _errorMessageField.GetComponentInChildren<TextMeshProUGUI>();
        _loadingObject.SetActive(true);
        _connectionPhase = 1;
        _initialize = ServicesInitialize();
        _buttons.SearchConnectionField.onEndEdit.AddListener(ConnectionClient);
        Debug.Log("MultiPlayManagerBase.Awake");
    }

    void Update()
    {
        _checkLateTimer += Time.deltaTime;
        if(_checkLateTimer > 1) return;
        _checkLateTimer = 0;
        
        switch (_connectionPhase)
        {
            case 1://初期化を行う。
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
                        _loadingObject.SetActive(false);
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "オンラインサービスの初期化に失敗しました　\n ネット環境を確認してください";
                    }
                }

                break;
            case 2://ルーム作成待機を行う
                var connectionHostAwaiter = _connectionHost.GetAwaiter();
                if (connectionHostAwaiter.IsCompleted)
                {
                    var result = connectionHostAwaiter.GetResult();
                    _loadingObject.SetActive(false);
                    if (result)
                    {
                        Debug.Log($"LobbyID : {_joinedLobby.Id}");
                        _connectionPhase = 0;
                        //-------------------------ゲーム開始処理をここに記述-------------------------------
                        _gameManager.enabled = true;
                        _gameManager.HostConnection();
                        gameObject.SetActive(false);
                        //------------------------------------------------------------------------------
                    }
                    else
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "ルーム作成に失敗しました。　\n ネット環境を確認してください";
                    }
                }

                break;
            case 3://ロビーリストの取得を待機している
                var connectionClientRandomAwaiter = _connectionClientRandom.GetAwaiter();
                if (connectionClientRandomAwaiter.IsCompleted)
                {
                    var result = connectionClientRandomAwaiter.GetResult();
                    if (result.Item1 && result.Item2.Count != 0)
                    {
                        _connectionPhase = 4;
                        //参加可能なロビーを探して可能なものの中からランダムで参加する。
                        var canJoinLobbies = result.Item2.Where((l) => !l.IsLocked).ToList();
                        var joinLobby = canJoinLobbies[Random.Range(0, canJoinLobbies.Count)];
                        Debug.Log($"joinLobbyID : {joinLobby.Id}");
                        _connectionClient = ClientConnect(joinLobby);
                    }
                    else
                    {
                        _loadingObject.SetActive(false);
                        _connectionPhase = 0;
                        _errorMessageField.SetActive(true);
                        if (result.Item2.Count != 0)
                            _errorMessage.text = "ルームの取得に失敗しました。　\n ネット環境を確認してください";
                        else
                            _errorMessage.text = "現在ルームは存在しません。　\n　作成するか、作成されるまでお待ちください";
                    }
                }

                break;
            case 4://3または５で取得したロビーへの参加を待機している
                var joinLobbyAwaiter = _connectionClient.GetAwaiter();
                if (joinLobbyAwaiter.IsCompleted)
                {
                    _connectionPhase = 0;
                    _loadingObject.SetActive(false);
                    if(!joinLobbyAwaiter.GetResult())
                    {
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "ルーム参加に失敗しました。　\n　ネット環境を確認してください";
                    }
                    else
                    {
                        _gameManager.enabled = true;
                        _gameManager.ClientConnection();
                        gameObject.SetActive(false);
                    }
                }

                break;
            case 5://ロビーコードからロビーの取得を待機している
                var lobbyCheckAwaiter = _lobbyCheck.GetAwaiter();
                if (lobbyCheckAwaiter.IsCompleted)
                {
                    var result = lobbyCheckAwaiter.GetResult();
                    if (result.Item1 && !result.Item2.IsLocked)
                    {
                        _connectionPhase = 4;//これ以降の処理は3～と同じなので流用
                        _connectionClient = ClientConnect(result.Item2);
                    }
                    else
                    {
                        _loadingObject.SetActive(false);
                        _errorMessageField.SetActive(true);
                        _errorMessage.text = "指定されたルームが見つかりませんでした。　\n 接続コードを見直してください";
                    }
                }
                break;
        }
    }

    public void ClipRoomID()
    {
        GUIUtility.systemCopyBuffer = _joinedLobby.Id;
    }


    public void SelectClientMode()
    {
        _StartCanvasAnimator.SetTrigger(ClientConnection);
    }

    public void SelectHostMode()
    {
        _StartCanvasAnimator.SetTrigger(HostConnection);
    }


    public void ConnectionHost(bool isPrivate)
    {
        _buttons.HideButtons();
        _loadingObject.SetActive(true);
        _buttons.DisableButtons();
        _connectionHost = HostConnect();
        _connectionPhase = 2;
    }

    public void ConnectionClientForRandom()
    {
        _buttons.HideButtons();
        _loadingObject.SetActive(true);
        _connectionClientRandom = _systemClass.MultiPlayClient.GetAllLobbyList();
        _buttons.DisableButtons();
        _connectionPhase = 3;
    }

    public void ConnectionClient(string lobbyID)
    {
        _buttons.HideButtons();
        _loadingObject.SetActive(true);
        _buttons.DisableButtons();
        _lobbyCheck = GetLobby(lobbyID);
        _connectionPhase = 5;
    }

    public void SoloModeStart()
    {
        SceneManager.LoadScene("SoloScene");
    }

    private async UniTask<(bool, Lobby)> GetLobby(string lobbyId)
    {
        try
        {
            Debug.Log(lobbyId + " lobbyID");
            var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
            return (true, lobby);
        }
        catch
        {
            return (false, null);
        }
    }


    [Serializable]
    struct Buttons
    {
        public Button RunnnerStartButton;
        public Button SupporterStartButton;

        public Button RandomConnectionButton;
        public TMP_InputField SearchConnectionField;

        public Button PrivateConnectionButton;
        public Button PublicConnectionButton;

        public void EnableButtons()
        {
            RunnnerStartButton.enabled = true;
            SupporterStartButton.enabled = true;
            RandomConnectionButton.enabled = true;
            SearchConnectionField.enabled = true;
            PrivateConnectionButton.enabled = true;
            PublicConnectionButton.enabled = true;
        }

        public void DisableButtons()
        {
            RunnnerStartButton.enabled = false;
            SupporterStartButton.enabled = false;
            RandomConnectionButton.enabled = false;
            SearchConnectionField.enabled = false;
            PrivateConnectionButton.enabled = false;
            PublicConnectionButton.enabled = false;
        }

        public void HideButtons()
        {
            RunnnerStartButton.gameObject.SetActive(false);
            SupporterStartButton.gameObject.SetActive(false);
            RandomConnectionButton.gameObject.SetActive(false);
            SearchConnectionField.gameObject.SetActive(false);
            PrivateConnectionButton.gameObject.SetActive(false);
            PublicConnectionButton.gameObject.SetActive(false);
        }
    }
}