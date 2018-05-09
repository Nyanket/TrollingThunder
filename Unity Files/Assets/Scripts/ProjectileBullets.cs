using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBullets : MonoBehaviour {

    [SerializeField]
    private PlayerWeapon weapon;

    public string sourceID;

    public PlayerShoot shoot;
    [SerializeField]
    private LayerMask mask;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.layer != LayerMask.NameToLayer("Bullets") && col.collider.gameObject.layer != LayerMask.NameToLayer("LocalPlayer"))
        {
            if (col.collider.gameObject.layer == LayerMask.NameToLayer("RemotePlayer"))
            {
                Player _player = GameManager.GetPlayer(col.transform.name);
                _player.RpcTakeDamage(weapon.damage, sourceID);
            }

            foreach (ContactPoint contact in col.contacts)
                shoot.CmdOnHit(contact.point, contact.normal);
            Debug.Log(col.transform.name);
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        Destroy(gameObject,5f);
    }
}
