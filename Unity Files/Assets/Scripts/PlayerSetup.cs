﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	[SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string RemoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDraw = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponent();
            AssignRemoteLayer();
        }
        else
        {
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDraw));            
            playerUIInstance = (GameObject)Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.Log("No PlayerUI component on PlayerUI prefab.");
            ui.SetPlayer(GetComponent<Player>());
            GetComponent<Player>().SetupPlayer();

            string _username = "Loading..";
            if (UserAccountManager.isLoggedIn)
                _username = UserAccountManager.playerUsername;
            else
                _username = transform.name;

            CmdSetUsername(transform.name, _username);

        }

        //RegisterPlayer();
        
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if(player != null)
        {
            player.username = username;
        }
    }

    void SetLayerRecursively (GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }
    /*
    void RegisterPlayer()
    {
        string _ID = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
    }*/

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(RemoteLayerName);
    }

    void DisableComponent()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        if(isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

}
