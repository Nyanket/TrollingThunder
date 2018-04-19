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

    [SyncVar]
    private int currIdx = 0;

    private WeaponGraphics currGraphics;

    [SerializeField]
    private List<GameObject> weaponObject;

    private GameObject currWeaponObj;

    private Player player;

    public bool isReloading = false;

    private void Start()
    {        
        player = GetComponent<Player>();
        for (int i = 0; i < weaponList.Length; i++)
        {
            CreateWeapon(weaponList[i]);
            weaponList[i].graphics.SetActive(false);
        }
        primaryWeapon = weaponList[0];
        currWeapon = primaryWeapon;
        CmdEquipWeapon(primaryWeapon, 0);
        foreach (PlayerWeapon weapon in weaponList)
        {
            weapon.currBullets = weapon.maxBullets;
        }        
        
    }

    void Update()
    {
        if(currWeaponObj)
            currWeaponObj.SetActive(true);

        if (isLocalPlayer)
        {
            if (currIdx != weaponSwitching.selectedWeapon)
            {
                int index = weaponSwitching.selectedWeapon;
                CmdEquipWeapon(weaponList[index], index);
                currIdx = index;
            }
            if (player.isDead)
            {
                foreach (PlayerWeapon weapon in weaponList)
                {
                    weapon.currBullets = weapon.maxBullets;
                }
            }
        }
    }

    public void SelectWeapon(int index)
    {
        int i = 0;
        foreach (Transform weapon in weaponHolder)
        {
            if (i == index)
            {
                weapon.gameObject.SetActive(true);
                currWeaponObj = weapon.gameObject;
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }

    [ClientRpc]
    void RpcEquipWeapon(PlayerWeapon _curr, int index)
    {
        currWeapon = _curr;
        currGraphics = weaponObject[index].GetComponent<WeaponGraphics>();
        SelectWeapon(index);
    }

    [Command]
    void CmdEquipWeapon(PlayerWeapon _curr, int index)
    {
        RpcEquipWeapon(_curr, index);
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
