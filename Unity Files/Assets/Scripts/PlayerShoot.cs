using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {


    private const string PLAYER_TAG = "Player";    

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currWeapon;

    private WeaponManager weaponManager;

    //public GameObject tes;


    void Start()
    {
        if(!cam)
        {
            Debug.LogError("PlayerShoot: No Camera References");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

    }

    void Update()    {
        
        currWeapon = weaponManager.GetCurrWeapon();
        //tes = weaponManager.GetCurrProjectile();

        if (PauseMenu.isOn)
            return;

        if(currWeapon.currBullets < currWeapon.maxBullets)
            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponManager.Reload();
                return;
            }

        if (currWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot",0f,1f/currWeapon.fireRate);
            }else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    [Command]
    public void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrGraphics().muzzleFlash.Play();
        weaponManager.GetCurrGraphics().GetComponent<AudioSource>().Play();
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if(currWeapon.currBullets<= 0)
        {
            weaponManager.Reload();
            return;
        }

        currWeapon.currBullets--;

        CmdOnShoot();
                    
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currWeapon.damage, transform.name);
            }
            CmdOnHit(_hit.point, _hit.normal);                 
        }
        

        if (currWeapon.currBullets <= 0)
            weaponManager.Reload();
    }

    [Command]
    void CmdPlayerShot (string _playerID, int _damage, string _sourceID)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage, _sourceID);
    }

    /* [ClientRpc]
     void RpcSpawnBullet(Vector3 hit)
     {
         GameObject _bullet = (GameObject)Instantiate(weaponManager.GetCurrProjectile(), weaponManager.GetCurrFirePoint().position, weaponManager.GetCurrFirePoint().rotation);
         _bullet.layer = LayerMask.NameToLayer("Bullets");
         _bullet.GetComponent<ProjectileBullets>().sourceID = transform.name;
         _bullet.GetComponent<ProjectileBullets>().shoot = GetComponent<PlayerShoot>();
         _bullet.GetComponent<Rigidbody>().MovePosition(hit);
         _bullet.GetComponent<Rigidbody>().AddForce(_bullet.transform.position * currWeapon.projectileSpeed);
         Vector3 direction = (hit - _bullet.transform.position).normalized;
         Vector3 move = direction * 1 * Time.deltaTime;
         _bullet.GetComponent<Rigidbody>().MovePosition(move);
         _bullet.transform.LookAt(hit);
         _bullet.GetComponent<Rigidbody>().AddRelativeForce(_bullet.transform.forward * 500);
         _bullet.transform.Translate((hit - _bullet.transform.position) * Time.deltaTime * 10f);
         _bullet.transform.position = Vector3.MoveTowards(_bullet.transform.position, hit, 10f * Time.deltaTime);
         _bullet.transform.position = Vector3.LerpUnclamped(_bullet.transform.position, hit, 10f * Time.deltaTime);

     }

     [Command]
     void CmdSpawnBullet(Vector3 hit)
     {
         RpcSpawnBullet(hit);
     }*/

}
