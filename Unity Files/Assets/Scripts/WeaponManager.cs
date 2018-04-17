using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private WeaponSwitching weaponSwitching;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon[] weaponList;

    private PlayerWeapon primaryWeapon;

    [SerializeField]
    private PlayerWeapon currWeapon;

    private int currIdx = 0;

    private WeaponGraphics currGraphics;

    [SerializeField]
    private List<GameObject> weaponObject;

    public bool isReloading = false;

    private void Start()
    {
        for (int i = 0; i < weaponList.Length; i++)
        {
            CreateWeapon(weaponList[i]);
            weaponList[i].graphics.SetActive(false);
        }
        primaryWeapon = weaponList[0];
        EquipWeapon(primaryWeapon, 0);
    }

    void Update()
    {
        if(currIdx != weaponSwitching.selectedWeapon)
        {
            int index = weaponSwitching.selectedWeapon;
            EquipWeapon(weaponList[index], index);
            currIdx = index;
        }
    }

    public void EquipWeapon(PlayerWeapon _curr, int index)
    {
        currWeapon = _curr;
        currGraphics = weaponObject[index].GetComponent<WeaponGraphics>();
    }

    public PlayerWeapon GetCurrWeapon()
    {
        return currWeapon;
    }

    public WeaponGraphics GetCurrGraphics()
    {
        return currGraphics;
    }

    void CreateWeapon(PlayerWeapon _weapon)
    {
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, Quaternion.Euler(0, 0, 0), weaponHolder);
        _weaponIns.transform.localPosition = Vector3.zero;
        _weaponIns.transform.localRotation = Quaternion.Euler(0, 0, 0);
        currGraphics = _weaponIns.GetComponent<WeaponGraphics>();



        if (currGraphics == null)
            Debug.LogError("No WeaponGraphics on the object: " + _weaponIns.name);

        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        weaponObject.Add(_weaponIns);
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
