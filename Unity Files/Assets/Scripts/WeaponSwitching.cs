using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour {
   
    public int selectedWeapon = 0;
    
	/*void Start () {
        //SelectWeapon();
	}*/
	
	
	void Update () {

        int previousSelectedWeapon = selectedWeapon;

		if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
            {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
            {
                selectedWeapon--;
            }
        }

        /*if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }*/

    }

}
