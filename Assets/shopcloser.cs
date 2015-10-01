using UnityEngine;
using System.Collections;

public class shopcloser : MonoBehaviour {

    public void Close()
    {
        if (SaucerPlayer.instance.tractor != upgradelevel.none && SaucerPlayer.instance.cargo != upgradelevel.none)
        {
            gameObject.SetActive(false);
        }
    }
}
