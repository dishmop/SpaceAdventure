using UnityEngine;
using System.Collections;
using UnityEngine.UI;   

//different tutorial states
enum gamestate
{
    Welcome1, Welcome2,
    Shoot1, Shoot2, Shoot3, Shoot3a, Shoot4, 
    Momentum1, Momentum2, Momentum3, Momentum4,
    Zoom1, Zoom2, 
    Navigate1, Navigate2, Navigate3,
    Force1, Force2, Force3,
    Tractor1, Tractor2, Tractor3, Tractor4, Tractor5, Tractor6,
    Done
};

public class TutorialLevel : LevelAbstract {

    public GameObject junk;



    GameObject largerock;

    gamestate currentState;

	// Use this for initialization
	void Start () {
        currentState = gamestate.Welcome1;
	}
	
	// Update is called once per frame
	void Update () {
	    switch(currentState)
        {
            case gamestate.Welcome1:
                if (ShowText("Welcome.",2)) currentState++;
                Fade();
                break;
            case gamestate.Welcome2:
                if (ShowText("This is your ship.")) currentState++;
                break;
            case gamestate.Zoom1:
                if (ShowText("Click the scrollwheel and move the mouse to zoom in/out.")) currentState++;
                break;
            case gamestate.Zoom2:
                zoomhighlight.SetActive(true);
                ShowText("Try zooming far out.", 3);

                if(CameraFollow.instance.zoom>100)
                    currentState++;
                break;
            case gamestate.Shoot1:
                
                if (ShowText("You can carry a certain amount of rock")) currentState++;
                break;
            case gamestate.Shoot2:
                if (ShowText("Here is some now...")) currentState++;
                break;
            case gamestate.Shoot3:
                masshighlight.SetActive(true);
                if (ShowText("Notice the bar in the bottom left.", 1)) currentState++;
                break;
            case gamestate.Shoot3a:
                if (SaucerPlayer.instance.sc.carriedmass < 50) SaucerPlayer.instance.sc.rockmass += 0.5f;
                if (ShowText("Notice the bar in the bottom left.", 5, 0.4f)) currentState++;
                break;
            case gamestate.Shoot4:
                masshighlight.SetActive(false);
                ShowText("Click to shoot.", 5);

                if (SaucerPlayer.instance.sc.carriedmass < 50) currentState++;
                break;
            case gamestate.Momentum1:
                if (ShowText("You are giving a mass of rock a certain velocity, that is, you are giving it momentum.")) currentState++;
                break;
            case gamestate.Momentum2:
                if(ShowText("Remember that Momentum = Mass × Velocity.", -1)) currentState++;
                break;
            case gamestate.Momentum3:
                if(ShowText("But momentum is conserved, so you are also giving the ship momentum in the opposite direction!")) currentState++;
                break;
            case gamestate.Momentum4:
                if (ShowText("You can use this to move the ship.", -1)) currentState++;
                break;
            case gamestate.Navigate1:
                zoomhighlight.SetActive(false);
                SaucerPlayer.instance.sc.rockmass = 0;
                Fade();
                if(!done)
                {
                    ResetFade();
                    done = true;
                    largerock = (GameObject)Instantiate(junk, SaucerPlayer.instance.transform.position + new Vector3(500, 500, 0), new Quaternion());
                    largerock.GetComponent<junkscript>().rockmass = 10000f;
                    SaucerPlayer.instance.GetComponent<Rigidbody2D>().velocity = new Vector3();
                    CameraFollow.instance.linearvel = new Vector2(0, 0);
                }
                if (ShowText("Remember Newton's First Law: An object in motion continues at constant velocity unless acted on by forces.")) currentState++;
                break;
            case gamestate.Navigate2:
                done = false;
                if (ShowText("This means you'll keep going forever, since there is no air resistance in space!")) currentState++;
                break;
            case gamestate.Navigate3:
                if (!done)
                {
                    done = true;
                    SaucerPlayer.instance.sc.rockmass = 100;
                }
                ShowText("Try to navigate to the large red rock on your minimap.",1);
                ShowReset();
                Fade();
                if(reset)
                {
                    Destroy(largerock);
                    reset = false;
                    largerock = (GameObject)Instantiate(junk, SaucerPlayer.instance.transform.position + new Vector3(500, 500, 0), new Quaternion());
                    largerock.GetComponent<junkscript>().rockmass = 10000f;
                    SaucerPlayer.instance.Respawn();
                    CameraFollow.instance.linearvel = new Vector2(0, 0);
                    SaucerPlayer.instance.sc.rockmass = 100;
                    ResetFade();
                }
                if ((largerock.transform.position - SaucerPlayer.instance.transform.position).magnitude < 80)
                    currentState++;
                break;
            case gamestate.Force1:
                HideReset();
                if(ShowText("Recall that force is the rate of change of momentum (Newton's Second Law).")) currentState++;
                break;
            case gamestate.Force2:
                if(ShowText("So if you stop quickly, by crashing into something, you experience a lot of force.")) currentState++;
                done = false;
                break;
            case gamestate.Force3:
                if(ShowText("Your shields can absorb some of the force, but not lots at one time.")) currentState++;
                if(!done)
                {
                    done = true;
                    SaucerPlayer.instance.Respawn();
                    ResetFade();
                    Destroy(largerock);
                    GameController.instance.NumRandomAsteroids = 500;
                    GameController.instance.SpawnRocks();
                    GameController.instance.PeriodicBCs = true;
                }
                Fade();
                break;
            case gamestate.Tractor1:
                SaucerPlayer.instance.tractor = upgradelevel.medium;
                if (ShowText("You can use a tractor beam by right-clicking.")) currentState++;
                break;
            case gamestate.Tractor2:
                if (ShowText("You can pick up green rocks by pulling them into you with the tractor beam.")) currentState++;
                break;
            case gamestate.Tractor3:
                if (ShowText("Red rocks are too large to be picked up.")) currentState++;
                break;
            case gamestate.Tractor4:
                if (ShowText("The beam exerts an attractive force on the rock.")) currentState++;
                break;
            case gamestate.Tractor5:
                if (ShowText("Newton's Third Law states: Every force has an equal and opposite force.")) currentState++;
                break;
            case gamestate.Tractor6:
                if (ShowText("This means that you are also pulling the ship towards the rock! You can use this to get around.")) currentState++;
                break;
            case gamestate.Done:
                if (ShowText("Tutorial complete! Press ok to enter the game.")) Application.LoadLevel(1);
                break;
            default:
                HideText();
                break;
        }
	}

    bool done = false;
}
