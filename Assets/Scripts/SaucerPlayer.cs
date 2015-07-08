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
    public Text cargobutton;

    public Text repairbutton;
    public Text sellbutton;

    public Text stabilizerbutton;

    public AudioSource beamsound;
    public AudioSource buttonsound;

    public float maxshotmass = 10f;

    public upgradelevel shield = upgradelevel.none;
    public upgradelevel tractor = upgradelevel.none;
    public upgradelevel cargo = upgradelevel.none;

    public int Cash;

    int mineralvalue
    {
        get
        {
            int minvalue = 0;

            for (int i = 0; i < GameController.numminerals; i++)
            {
                minvalue += (int)(sc.mineralmass[i] * GameController.instance.MineralValue[i]);
            }
            return minvalue;
        }
    }
    int repaircost;

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

        switch (cargo)
        {
            case upgradelevel.none:
                sc.maxcarriedmass = 0.1f;
                break;
            case upgradelevel.basic:
                sc.maxcarriedmass = 80f;
                break;
            case upgradelevel.medium:
                sc.maxcarriedmass = 200f;
                break;
            case upgradelevel.high:
                sc.maxcarriedmass = 500f;
                break;
        }

        switch (shield)
        {
            case upgradelevel.none:
                sc.shield = 0;
                break;
            case upgradelevel.basic:
                sc.maxshield = 200;
                break;
            case upgradelevel.medium:
                sc.maxshield = 800f;
                break;
            case upgradelevel.high:
                sc.maxshield = 2000f;
                break;
        }


        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //shoot when mouse clicked
            if (Input.GetKey(KeyCode.Mouse0))
            {
                float shotmass = GameController.instance.minAsteroidMass;// shotsize.value * (maxshotmass - GameController.instance.minAsteroidMass) + GameController.instance.minAsteroidMass;
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

        //if( Input.GetKeyDown(KeyCode.Mouse1))
        //{
        //    if(tractor != upgradelevel.none)
        //    {
        //        beamsound.Play();
        //    }
        //}

        //if(Input.GetKeyUp(KeyCode.Mouse1))
        //{
        //    beamsound.Stop();
        //}

        if(sc.beam.active)
        {
            if(!beamsound.isPlaying)
                beamsound.Play();

            if(sc.tractedobj!=null)
            {
                beamsound.pitch = 0.75f;
            } else
            {
                beamsound.pitch = 0.7f;
            }
        }
        else
        {
            beamsound.Stop();
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

        

        repaircost = (int)((1 - sc.health) * 500f);

        sellbutton.text = "Sell Minerals + $" + mineralvalue;
        repairbutton.text = "Repair Ship - $" + repaircost;
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
                if(Cash >= 100)
                {
                    buttonsound.Play();

                    tractor++;
                    Cash -= 100;
                    tractorbutton.text = "Upgrade range - $250";
                    stabilizerbutton.text = "Buy Stabilizer - $1000";
                }
                break;
            case upgradelevel.basic:
                if(Cash >= 250)
                {
                    buttonsound.Play();

                    tractor++;
                    Cash -= 250;
                    tractorbutton.text = "Upgrade range - $500";
                }
                break;
            case upgradelevel.medium:
                if (Cash >= 500)
                {
                    buttonsound.Play();

                    tractor++;
                    Cash -= 500;
                    tractorbutton.text = "Maxed";
                }
                break;
        }
    }

    public void Upgradecargo()
    {
        switch (cargo)
        {
            case upgradelevel.none:
                if (Cash >= 50)
                {
                    buttonsound.Play();

                    cargo++;
                    Cash -= 50;
                    cargobutton.text = "Upgrade capacity- $200";
                }
                break;
            case upgradelevel.basic:
                if (Cash >= 200)
                {
                    buttonsound.Play();

                    cargo++;
                    Cash -= 200;
                    cargobutton.text = "Upgrade capacity- $500";
                }
                break;
            case upgradelevel.medium:
                if (Cash >= 500)
                {
                    buttonsound.Play();

                    cargo++;
                    Cash -= 500;
                    cargobutton.text = "Maxed";
                }
                break;
        }
    }

    public void Upgradeshield()
    {
        switch (shield)
        {
            case upgradelevel.none:
                if (Cash >= 200)
                {
                    buttonsound.Play();

                    shield++;
                    Cash -= 200;
                    shieldbutton.text = "Upgrade strength - $500";
                }
                break;
            case upgradelevel.basic:
                if (Cash >= 500)
                {
                    buttonsound.Play();

                    shield++;
                    Cash -= 500;
                    shieldbutton.text = "Upgrade strength - $1500";
                }
                break;
            case upgradelevel.medium:
                if (Cash >= 1500)
                {
                    buttonsound.Play();

                    shield++;
                    Cash -= 1500;
                    shieldbutton.text = "Maxed";
                }
                break;
        }
    }

    public void SellMinerals()
    {
        buttonsound.Play();

        Cash += mineralvalue;
        for(int i=0; i<GameController.numminerals; i++)
        {
            sc.mineralmass[i] = 0;
        }
    }

    public void RepairShip()
    {
        if(Cash >= repaircost)
        {
            buttonsound.Play();

            sc.health = 1;
            Cash -= repaircost;
        }
    }

    public void BuyStabilizer()
    {
        if(Cash >= 1000)
        {
            buttonsound.Play();
            Cash -= 1000;
            sc.tractorstabilizer = true;
            stabilizerbutton.text = "Stabilizer bought";
        }
    }
}
