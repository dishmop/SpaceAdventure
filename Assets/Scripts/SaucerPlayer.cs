using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum upgradelevel
{
    none, basic, medium, high
};


[RequireComponent(typeof(SaucerController))]
public class SaucerPlayer : MonoBehaviour {
    public static SaucerPlayer instance { get; private set; }

    public SaucerController sc;

    Rigidbody2D rb;

    public Slider massslider;
    public Slider minmassslider;
    public Slider healthslider;
    public Slider shotsize;

    public Text tractorbutton;
    public Text shieldbutton;

    public float maxshotmass = 10f;

    public upgradelevel shield = upgradelevel.none;
    public upgradelevel tractor = upgradelevel.none;

	// Use this for initialization
	void Start () {
        instance = this;
        sc = GetComponent<SaucerController>();
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        //rotate arm towards mouse
        Vector3 cursor = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        cursor.z = 0;
        //Vector3 mouseoffset = cursor - arm.transform.position;

        switch(tractor)
        {
            case upgradelevel.none:
                sc.tractorrange = 0;
                break;
            case upgradelevel.basic:
                sc.tractorrange = 100f;
                break;
            case upgradelevel.medium:
                sc.tractorrange = 300f;
                break;
            case upgradelevel.high:
                sc.tractorrange = 800f;
                break;
        }

        switch (shield)
        {
            case upgradelevel.none:
                sc.shield = 0;
                break;
            case upgradelevel.basic:
                sc.maxshield = 100f;
                break;
            case upgradelevel.medium:
                sc.maxshield = 300f;
                break;
            case upgradelevel.high:
                sc.maxshield = 800f;
                break;
        }


        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //shoot when mouse clicked
            if (Input.GetKey(KeyCode.Mouse0))
            {
                float shotmass = shotsize.value * (maxshotmass - GameController.instance.minAsteroidMass) + GameController.instance.minAsteroidMass;
                sc.Shoot(shotmass, cursor);
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            sc.Tractor(true, new Vector3(cursor.x, cursor.y));
        }
        else
        {
            sc.Tractor(false, new Vector3(cursor.x, cursor.y));
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.angularDrag = 5;
        }
        else
        {
            rb.angularDrag = 0;
        }

        massslider.value = sc.carriedmass / sc.maxcarriedmass;
        healthslider.value = sc.health;
        minmassslider.value = (sc.carriedmass - sc.rockmass) / sc.maxcarriedmass;
	}

    public void Respawn()
    {
        sc.health = 1;
        sc.dead = false;
        sc.shield = sc.maxshield;
        rb.velocity = new Vector2();
        CameraFollow.instance.linearvel = new Vector2(0, 0);
    }

    public void Upgradetractor()
    {
        switch(tractor)
        {
            case upgradelevel.none:
                if(GameController.instance.Cash >= 100)
                {
                    tractor++;
                    GameController.instance.Cash -= 100;
                    tractorbutton.text = "Upgrade - $250";
                }
                break;
        }
    }

    public void Upgradeshield()
    {

    }

    float Pdvalue = 10;
    float Irvalue = 20;
    float Wvalue = 30;

    public void SellMinerals()
    {
        GameController.instance.Cash += (int)(Pdvalue * sc.Pd + Irvalue * sc.Ir + Wvalue * sc.W);
        sc.Pd = sc.Ir = sc.W = 0;
    }
}
