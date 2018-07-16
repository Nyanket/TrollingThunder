using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Weapon", menuName = "Weapon")]
public class PlayerWeapon : ScriptableObject
{
    

    public string weaponName = "Glock";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f;

    public int maxBullets = 40;
    [HideInInspector]
    public int currBullets;

    public float reloadTime = 1.3f;

    public GameObject graphics;

    //public AudioClip shot;

    //public bool isProjectile = false;

    //public float projectileSpeed = 0f;

    //public GameObject projectile;

    public PlayerWeapon()
    {
        currBullets = maxBullets;
    }

}
