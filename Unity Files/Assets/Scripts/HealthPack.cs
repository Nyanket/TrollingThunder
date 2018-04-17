using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class HealthPack : NetworkBehaviour {

    [SerializeField]
    private GameObject healthPack;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private Image cdImage;
    [SerializeField]
    private float packCD = 10f;
    [SerializeField]
    private int addedHealth = 100;

    [SyncVar]
    private float currCD;
    [SyncVar]
    private float nextReadyTime;

    void Start () {
        currCD = packCD;
        canvas.SetActive(false);
        cdImage.fillAmount = 0f;
	}
	
	
	void Update () {
        float rotationSpeed = 20;
        healthPack.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        bool packReady = (Time.time > nextReadyTime);
        if (packReady)
        {
            CmdSpawnHealthPack();
        }
        else
        {
            CmdCooldown();
        }


    }

    [Client]
    void OnTriggerEnter(Collider col)
    {
        Player _player = GameManager.GetPlayer(col.transform.name);
        if(_player.GetCurrHealth() == _player.GetMaxHealth())
        {
            Debug.Log(col.transform.name + " health full");
            return;
        }
        TakeHealthPack();
        CmdTakeHealthPack();
        CmdAddHealth(col.transform.name);       
    }

    [ClientRpc]
    private void RpcDoSpawnHealthPack()
    {
        SpawnHealthPack();
    }

    [Command]
    private void CmdSpawnHealthPack()
    {
        RpcDoSpawnHealthPack();
    }

    private void SpawnHealthPack()
    {
        canvas.SetActive(false);
        healthPack.SetActive(true);
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
    }

    private void Cooldown()
    {
        currCD -= Time.deltaTime;
        cdImage.fillAmount = 1f - (currCD / packCD);
    }

    private void TakeHealthPack()
    {        
        healthPack.SetActive(false);
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;
        nextReadyTime = packCD + Time.time;
        currCD = packCD;
        canvas.SetActive(true);
    }

    [Command]
    void CmdAddHealth(string _playerID)
    {
        Debug.Log(_playerID + " take health pack");
        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeHealthPack(addedHealth);        
    }

    [ClientRpc]
    private void RpcDoCooldown()
    {
        Cooldown();
    }

    [Command]
    private void CmdCooldown()
    {
        RpcDoCooldown();
    }

    [ClientRpc]
    private void RpcDoTakeHealthPack()
    {
        TakeHealthPack();        
    }

    [Command]
    private void CmdTakeHealthPack()
    {
        RpcDoTakeHealthPack();        
    }

    

}
