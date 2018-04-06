using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNamePlates : MonoBehaviour {

    [SerializeField]
    Text usernameText;

    [SerializeField]
    private RectTransform healthBarFill;

    [SerializeField]
    private Player player;

    void Update()
    {
        usernameText.text = player.username;
        healthBarFill.localScale = new Vector3(player.getHealthPerc(), 1f, 1f);
    }

}
