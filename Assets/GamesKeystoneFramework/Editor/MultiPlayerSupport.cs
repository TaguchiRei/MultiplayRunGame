using System;
using System.Collections.Generic;
using GamesKeystoneFramework.MultiPlaySystem;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Transports.UTP;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamesKeystoneFramework.Editor
{
    public class MultiPlayerSupport : EditorWindow
    {
        private bool _setUp;

        private bool _useRigidBody;

        private GameObject _multiPlayerObjectGroup;
        
        private List<GameObject> _settingObject;
        private SerializedObject _settingSerializedObject;
        private SerializedProperty _settingProperty;
        
        Vector2 _scrollPosition;
        
        [MenuItem("Window/GamesKeystoneFramework/MultiplayerSupport")]
        public static void ShowWindow()
        {
            GetWindow<MultiPlayerSupport>("MultiPlayerSupport").Show();
        }

        private void OnEnable()
        {
            _multiPlayerObjectGroup = GameObject.Find("MultiPlayerObjectGroup");
            _setUp = _multiPlayerObjectGroup != null;
            _settingObject = new List<GameObject> { null };
        }

        private void OnGUI()
        {
            if (!_setUp)
            {
                if (GUILayout.Button("Setup Multiplayer"))
                {
                    //マルチプレイで共有するオブジェクトの親を作成
                    _setUp = true;
                    GameObject multiPlayerObjectGroup = new GameObject("MultiPlayerObjectGroup");
                    _multiPlayerObjectGroup = multiPlayerObjectGroup;
                    Undo.RegisterCreatedObjectUndo(multiPlayerObjectGroup, "MultiPlayerObjectGroup");
                    Selection.activeGameObject = multiPlayerObjectGroup;

                    //マルチプレイマネージャーのセットアップ
                    GameObject MultiPlayManager = new GameObject("MultiPlayManager");
                    
                    //var manager = MultiPlayManager.AddComponent<MultiPlayManager>();
                    MultiPlayManager.AddComponent<MultiPlayHostSystem>();
                    MultiPlayManager.AddComponent<MultiPlayHostSystem>();
                    MultiPlayManager.AddComponent<NetworkManager>();
                    MultiPlayManager.AddComponent<UnityTransport>();
                    /*
                    SerializedObject managerObj = new SerializedObject(manager);
                    SerializedProperty managerProp = managerObj.FindProperty("MultiPlayObjectGroup");
                    if (managerProp != null)
                    {
                        managerProp.objectReferenceValue = multiPlayerObjectGroup;
                        managerObj.ApplyModifiedProperties();
                        EditorUtility.SetDirty(manager);
                    }
                    else
                    {
                        Debug.Log("Property :MultiPlayerObjectGroup is not found");
                    }
                    */
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Setup Object", EditorStyles.boldLabel,GUILayout.Width(100));
                if (GUILayout.Button("Add", GUILayout.Width(60)))
                {
                    _settingObject.Add(null);
                }

                if (GUILayout.Button("Remove",GUILayout.Width(60)) && _settingObject.Count != 1)
                {
                    _settingObject.RemoveAt(_settingObject.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition,GUILayout.Height(Mathf.Min(position.height - 180,(_settingObject.Count + 1) * 20)));
                for (int i = 0; i < _settingObject.Count; i++)
                {
                    _settingObject[i] =
                        (GameObject)EditorGUILayout.ObjectField(_settingObject[i], typeof(GameObject), true);
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.ToggleLeft("Use RigidBody", _useRigidBody);
                if (GUILayout.Button("Setting"))
                {
                    foreach (var obj in _settingObject)
                    {
                        if(obj == null) continue;
                        obj.transform.SetParent(_multiPlayerObjectGroup.transform);
                        obj.AddComponent<NetworkObject>();
                        obj.AddComponent<NetworkTransform>();
                        obj.AddComponent<MultiPlayObject>();
                        if (_useRigidBody)
                        {
                            obj.AddComponent<Rigidbody>();
                            obj.AddComponent<NetworkRigidbody>();
                        }
                    }
                    
                    _settingObject.Clear();
                    _settingObject.Add(null);
                }
            }
        }
    }
}