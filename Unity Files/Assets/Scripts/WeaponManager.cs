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
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics,weaponHolder.position,weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currGraphics = _weaponIns.GetComponent<WeaponGraphics>();



        if (currGraphics == null)
            Debug.LogError("No WeaponGraphics on the object: " + _weaponIns.name);

        if (isLocalPlayer)
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
    }


}
