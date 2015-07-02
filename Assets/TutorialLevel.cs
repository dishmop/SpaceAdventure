using UnityEngine;
using System.Collections;
using UnityEngine.UI;   

//different tutorial states
enum gamestate
{
    Zoom1, Zoom2, 
    Shoot1, Shoot2, Shoot3, Shoot3a, Shoot4, 
    Momentum1, Momentum2, Momentum3, Momentum4, 
    Navigate1, Navigate2, Navigate3,
    Done
};

public class TutorialLevel : MonoBehaviour {
    public Text messagetext;
    public GameObject messagepanel;
    public GameObject okbutton;

    public GameObject junk;

    public GameObject masshighlight;
    public GameObject zoomhighlight;

    GameObject largerock;

    gamestate currentState;

	// Use this for initialization
	void Start () {
        currentState = gamestate.Zoom1;
	}
	
	// Update is called once per frame
	void Update () {
	    switch(currentState)
        {
            case gamestate.Zoom1:
                if (ShowText("Click the scrollwheel and move the mouse to zoom in/out.", -1)) currentState++;
                break;
            case gamestate.Zoom2:
                zoomhighlight.SetActive(true);
                ShowText("Try zooming far out.", 3);

                if(CameraFollow.instance.zoom>100)
                    currentState++;
                break;
            case gamestate.Shoot1:
                zoomhighlight.SetActive(false);
                if (ShowText("You can carry a certain amount of rock", -1)) currentState++;
                break;
            case gamestate.Shoot2:
                if (ShowText("Here is some now...", -1)) currentState++;
                break;
            case gamestate.Shoot3:
                masshighlight.SetActive(true);
                if (ShowText("Notice the bar in the bottom left.", 1)) currentState++;
                break;
            case gamestate.Shoot3a:
                if (SaucerPlayer.instance.sc.carriedmass < 50) SaucerPlayer.instance.sc.carriedmass += 0.5f;
                if (ShowText("Notice the bar in the bottom left.", 5, 0.4f)) currentState++;
                break;
            case gamestate.Shoot4:
                masshighlight.SetActive(false);
                ShowText("Click to shoot.", 5);

                if (SaucerPlayer.instance.sc.carriedmass < 50) currentState++;
                break;
            case gamestate.Momentum1:
                if (ShowText("You are giving a mass of rock a certain velocity, that is, you are giving it momentum.", -1)) currentState++;
                break;
            case gamestate.Momentum2:
                if(ShowText("Remember that Momentum = Mass × Velocity.", -1)) currentState++;
                break;
            case gamestate.Momentum3:
                if(ShowText("But momentum is conserved, so you are also giving the ship momentum in the opposite direction!",-1)) currentState++;
                break;
            case gamestate.Momentum4:
                if (ShowText("You can use this to move the ship.", -1)) currentState++;
                break;
            case gamestate.Navigate1:
                SaucerPlayer.instance.sc.carriedmass = 0;
                if(!done)
                {
                    done = true;
                    largerock = (GameObject)Instantiate(junk, SaucerPlayer.instance.transform.position + new Vector3(500, 500, 0), new Quaternion());
                    largerock.GetComponent<Rigidbody2D>().mass = 10000f;
                    largerock.GetComponent<Rigidbody2D>().velocity = SaucerPlayer.instance.GetComponent<Rigidbody2D>().velocity;
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
                    SaucerPlayer.instance.sc.carriedmass = 100;
                }
                ShowText("Try to navigate to the large red rock on your minimap.",1);
                if ((largerock.transform.position - SaucerPlayer.instance.transform.position).magnitude < 80)
                    currentState++;
                break;
            default:
                HideText();
                break;
        }
	}

    bool done = false;

    float texttime = 0;
    string oldtext;
    public bool finished
    {
        get;
        set;
    }

    //returns whether the text has been there long enough
    bool ShowText(string text, float displaytime = 0.0f, float startalpha = 1.0f)
    {
        Color col;

        if (text != oldtext)
        {
            texttime = 0;
            oldtext = text;
            messagetext.text = text;
            messagepanel.SetActive(true);
            col = Color.white;
            messagepanel.GetComponent<Image>().color = col;

            finished = false;
        }
        else
        {

            texttime += Time.deltaTime;

            col = messagepanel.GetComponent<Image>().color;
            col.a = Mathf.Exp(-texttime) * 0.6f + 0.4f;
            messagepanel.GetComponent<Image>().color = col;
        }

        if (displaytime > 0)
        {
            okbutton.SetActive(false);
            if (texttime > displaytime)
            {
                return true;
            }
        }else
        {
            okbutton.SetActive(true);
            if (finished) return true;
        }

        return false;

    }

    void HideText()
    {
        messagepanel.SetActive(false);
    }
}
