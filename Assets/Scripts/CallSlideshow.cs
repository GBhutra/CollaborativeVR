using UnityEngine;
using System.Collections;

public class CallSlideshow : MonoBehaviour {


    public GameObject slideCanvas;
    public GameObject titleCanvas;
    public GameObject[] slides = new GameObject[3]; //change this number as needed
    public float transitionDelay = 10.0f;
    public float fadeTime = 2.5f;
    public bool hasTitle = true;
    private bool slidesDone = true;
    private float audioClipLength = 0.0f;

    //Deprecated, but keeping if needed
    /*
    void getSlides()
    {
        slides = new GameObject[slideCanvas.transform.childCount];
        int i = 0;
        foreach(Transform child in slideCanvas.transform)
        {
            slides[i] = child.gameObject;
            i++;
        }
    }
    */

    IEnumerator startAudio(GameObject go)
    {
        AudioSource audio = go.transform.FindChild("SlideAudio").GetComponent<AudioSource>();
        audio.Play();
        audioClipLength = audio.clip.length;
        yield return null;
    }

    IEnumerator fadeObjectOut(GameObject go, float fadeTime)
    {
        go.SetActive(true);
        FadeInSlides fil = (FadeInSlides)go.GetComponent(typeof(FadeInSlides));
        fil.FadeOut(fadeTime);
        //yield return new WaitForSecondsRealtime(10);
        yield return null;
    }

    IEnumerator fadeObjectIn(GameObject go, float fadeTime)
    {
        go.SetActive(true);
        FadeInSlides fil = (FadeInSlides)go.GetComponent(typeof(FadeInSlides));
        fil.FadeIn(fadeTime);
        yield return null;
    }
    
    //create simple routine for calling fade-out for differing objects
    IEnumerator fadeEachOut(float ft)
    {
        foreach (GameObject slide in slides)
        {
            StartCoroutine(fadeObjectOut(slide, ft));
            //yield return new WaitForSecondsRealtime(transitionDelay);
            yield return null;
        }
    }

    //fade every slide in and play its audio
    IEnumerator fadeEachIn(float ft)
    {
        foreach (GameObject slide in slides)
        {
            StartCoroutine(startAudio(slide));
            StartCoroutine(fadeObjectIn(slide, ft));
            if (audioClipLength >= transitionDelay)
            {
                yield return new WaitForSecondsRealtime(audioClipLength);
            }
            else
            {
                yield return new WaitForSecondsRealtime(transitionDelay);
            }
            //yield return new WaitForSecondsRealtime(transitionDelay);
            yield return null;
        }
        slidesDone = true;
    }

    IEnumerator startSlideshow()
    {
        //play slideshow
        slidesDone = false;
        StartCoroutine(fadeEachIn(fadeTime));
        while (!slidesDone) {
            yield return new WaitForSecondsRealtime(1);
        }
        //if this is the title sequence, play title as well
        if (hasTitle)
        {
            //titleCanvas.SetActive(true);
            StartCoroutine(fadeEachOut(fadeTime));
            StartCoroutine(fadeObjectIn(titleCanvas, fadeTime));
            yield return new WaitForSecondsRealtime(5);
            StartCoroutine(fadeObjectOut(titleCanvas, fadeTime));
        }
        yield return null;
    }

    // Use this for initialization
    void Start () {
        //activate slideCanvas if not active
        slideCanvas.SetActive(true);
        //getSlides();  //get all slides if need be
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(fadeEachOut(fadeTime));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            //StartCoroutine(fadeEachIn(fadeTime));
            StartCoroutine(startSlideshow());
        }
	}
}
