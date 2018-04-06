using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

    [SerializeField]
    Text text;

    private void Start()
    {
        transform.SetAsFirstSibling();
        
    }

    public void Setup(string player, string source)
    {
        text.text = "<b>" + source + "</b>" + " killed " + "<i>" + player + "</i>";
    }
}
