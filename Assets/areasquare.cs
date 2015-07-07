using UnityEngine;
using System.Collections;

public class areasquare : MonoBehaviour {
    public int x;
    public int y;

    public SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        renderer.color = GameController.instance.props[x, y].fogcolour;

        float areasize = GameController.areasize;
        int nx = GameController.xsize;
        int ny = GameController.ysize;

        float worldx = areasize * (float)(x - nx / 2) ;
        float worldy = areasize * (float)(y - ny / 2) ;
        transform.position = new Vector3(worldx, worldy, 0);

	}
}
