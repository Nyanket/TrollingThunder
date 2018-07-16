using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour {
   
    public int selectedWeapon = 0;
    
	void Update () {

        if (PauseMenu.isOn)
        {            
            return;
        }

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            selectedWeapon = 1;        

    }

}
