using UnityEngine;
using System.Collections;
using UnityEngine.UI;   


public abstract class LevelAbstract : MonoBehaviour {
    public Text messagetext;
    public GameObject messagepanel;
    public GameObject okbutton;
    public GameObject resetbutton;

    public GameObject masshighlight;
    public GameObject zoomhighlight;
    public GameObject fader;
    float texttime = 0;
    string oldtext;
    public bool finished
    {
        get;
        set;
    }

    public bool reset
    {
        get;
        set;
    }

    protected void ShowReset()
    {
        resetbutton.SetActive(true);
    }

    protected void HideReset()
    {
        resetbutton.SetActive(false);
        reset = false;
    }

    //returns whether the text has been there long enough
    protected bool ShowText(string text, float displaytime = 0.0f, float startalpha = 1.0f)
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

            messagepanel.GetComponent<AudioSource>().Play();
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
        }
        else
        {
            okbutton.SetActive(true);
            if (finished) return true;
        }

        return false;

    }

    protected void HideText()
    {
        messagepanel.SetActive(false);
    }

    protected void ResetFade()
    {
        fader.GetComponent<Image>().color = Color.white;
        fader.SetActive(true);
    }

    protected void Fade()
    {
        Color color = fader.GetComponent<Image>().color;
        color.a -= 2f * Time.deltaTime;
        fader.GetComponent<Image>().color = color;

        if (color.a <= 0)
        {
            fader.SetActive(false);
        }
    }
}
