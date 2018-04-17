using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour{

    [SyncVar]
    private bool _isDead;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField] private int maxHealth = 100;

    [SyncVar]
    private int currHealth;

    [SyncVar]
    public string username = "Loading...";

    public int kills;
    public int deaths;

    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    [SerializeField] private GameObject[] disableGameObjectsOnDeath;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;

    private bool firstSetup = true;

    private WeaponManager weaponManager;
    private PlayerWeapon currWeapon;

    private void Update()
     {
         if (!isLocalPlayer)
             return;
         if (Input.GetKeyDown(KeyCode.K))
         {
             RpcTakeDamage(50,null);
         }
     }

    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    public float getHealthPerc()
    {
        return (float)currHealth / maxHealth;
    }

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }       
        
        CmdBroadCastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClient();
    }
    [ClientRpc]
    private void RpcSetupPlayerOnAllClient()
    {
        if (firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }

        SetDefaults();
    }


    [ClientRpc]
    public void RpcTakeDamage (int _amount, string _sourceID)
    {
        if (isDead)
            return;
        currHealth -= _amount;
        Debug.Log(transform.name + " now has " + currHealth + " health");
        
        if(currHealth <= 0)
        {
            Die(_sourceID);
        }
    }

    private void Die(string _sourceID)
    {
        isDead = true;

        Player sourcePlayer = GameManager.GetPlayer(_sourceID);
        if(sourcePlayer != null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallBack.Invoke(username, sourcePlayer.username);
        }       

        deaths++;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        Collider _col = GetComponent<Collider>();

        if (_col != null)
            _col.enabled = false;

        GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);
        Debug.Log(transform.name + " is Dead");

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        StartCoroutine(Respawn());
        currWeapon = weaponManager.GetCurrWeapon();
        currWeapon.currBullets = currWeapon.maxBullets;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
    }

    public void SetDefaults()
    {
        isDead = false;

        currHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>();

        if (_col != null)
            _col.enabled = true;

        

        GameObject _gfxIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxIns, 3f);

    }

    [ClientRpc]
    public void RpcTakeHealthPack(int _amount)
    {
        if(currHealth + _amount > maxHealth)
        {
            currHealth = maxHealth;
        }else
        {
            currHealth += _amount;
        }

    }

    public int GetCurrHealth()
    {
        return currHealth;
    }
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
