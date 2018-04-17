using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour {

    [SerializeField]
    GameObject killfeedItemPrefab;

    void Start () {
        GameManager.instance.onPlayerKilledCallBack += OnKill;
	}
	
    public void OnKill(string player, string source)
    {
        GameObject go = (GameObject)Instantiate(killfeedItemPrefab, this.transform);
        go.GetComponent<KillFeedItem>().Setup(player, source);

        Destroy(go, 4f);
    }

}
