using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currWeapon;

    private WeaponGraphics currGraphics;

    public bool isReloading = false;

    private void Start()
    {
        equipWeapon(primaryWeapon);
    }

    public PlayerWeapon GetCurrWeapon()
    {
        return currWeapon;
    }

    public WeaponGraphics GetCurrGraphics()
    {
        return currGraphics;
    }

    void equipWeapon(PlayerWeapon _weapon)
    {
        currWeapon = _weapon;
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics,weaponHolder.position,Quaternion.Euler(0,0,0));
        _weaponIns.transform.SetParent(weaponHolder);
        _weaponIns.transform.localPosition = Vector3.zero;
        _weaponIns.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currGraphics = _weaponIns.GetComponent<WeaponGraphics>();



        if (currGraphics == null)
            Debug.LogError("No WeaponGraphics on the object: " + _weaponIns.name);

        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }

    public void Reload()
    {
        if (isReloading)
            return;
        StartCoroutine(Reload_Coroutine());
        
    }

    private IEnumerator Reload_Coroutine()
    {
        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currWeapon.reloadTime);

        currWeapon.currBullets = currWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currGraphics.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Reload");
        }
    }

}
