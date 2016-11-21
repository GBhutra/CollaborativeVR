using UnityEngine;
using System.Collections;

public class CallSlideshow : MonoBehaviour
{

    //-----------------------------------------------------------------------------------------------------------------
    //READ ME!!
    //
    //SLIDE FADE-IN CALLED BY FadeInSlides.cs; look there for information on how fading works
    //Slides are currently structured as a plain child of a canvas, with a copy of FadeInSlides attached to it
    //some sort of visual object (I ran 3D models and 2D sprites) as a child of a slide (named SlideMaterial) and
    //an audio source (named SlideAudio) that has associated snippets of audio for a slide.
    //
    //Be wary of breaking coroutines!
    //
    //TO-DO: Set starting alpha of all objects that need to fade in to 0; may be done in FadeInSlides.cs or here
    //-----------------------------------------------------------------------------------------------------------------

    //public GameObject slideCanvas;  //what it says; parent object of all slides
    public GameObject[] titleCanvases = new GameObject[1];  //parent object of title, if we have one.
    public GameObject[] slides = new GameObject[3]; //change this number in the Unity Environment. Define slides there as well.
    public float transitionDelay = 10.0f; //minimum time between slide fades; if audio source time is longer, go with that.
    public float fadeTime = 2.5f; //duration of a given fade
    public float titleTime = 2.5f; //duration to hold title or credits
    public bool hasTitle = true;    //determine whether this is a title or ending sequence
    private bool slidesDone = true;
    public bool fadedOut = false;
    public bool midfade = false;
    public bool sceneComplete = false;
    public bool isEnding = false;
    private float audioClipLength = 0.0f;
    //private int fadedSlides = 0;

    //Deprecated, but keeping if needed
    /*
    //get child slides from parent slide canvas
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


    /*bool runFunc(){
	}*/

    //coroutine for playing audio; also sets length of currently playing audio for other functions
    public IEnumerator startAudio(GameObject go)
    {
        AudioSource audio = go.transform.FindChild("SlideAudio").GetComponent<AudioSource>();
        audio.Play();
        audioClipLength = audio.clip.length;
        yield return null;
    }

    //call FadeOut on a given object and its children (prefer to use on slides, but can also be used on canvases)
    public IEnumerator fadeObjectOut(GameObject go, float fadeTime)
    {
        go.SetActive(true);
        FadeInSlides fil = (FadeInSlides)go.GetComponent(typeof(FadeInSlides));
        fil.FadeOut(fadeTime);
        //yield return new WaitForSecondsRealtime(10);
        //fadedSlides++;
        yield return null;
    }

    //call FadeIn on a given object and its children (prefer to use on slides, but can also be used on canvases)
    public IEnumerator fadeObjectIn(GameObject go, float fadeTime)
    {
        go.SetActive(true);
        FadeInSlides fil = (FadeInSlides)go.GetComponent(typeof(FadeInSlides));
        //fil.FadeIn(fadeTime);
        fil.FadeIn(fadeTime);
        yield return null;
    }

    //create simple routine for calling fade-out for differing objects
    public IEnumerator fadeEachOut(float ft)
    {
        midfade = true;
        foreach (GameObject slide in slides)
        {
            StartCoroutine(fadeObjectOut(slide, ft));
            //yield return new WaitForSecondsRealtime(transitionDelay);
            yield return null;
        }
        fadedOut = true;
        midfade = false;
    }



    //fade every slide in and play its audio
    public IEnumerator fadeEachIn(float ft)
    {
        midfade = true;
        foreach (GameObject slide in slides)
        {
            StartCoroutine(startAudio(slide));
            StartCoroutine(fadeObjectIn(slide, ft));
            //determine which delay to use between slide transitions (use longer of audio length or transition delay)
            if (audioClipLength >= transitionDelay)
            {
                yield return new WaitForSecondsRealtime(audioClipLength);
            }
            else
            {
                yield return new WaitForSecondsRealtime(transitionDelay);
            }
            yield return null;
        }
        //alert world that the slideshow has finished playing
        slidesDone = true;
        fadedOut = false;
        midfade = false;
    }

    public IEnumerator fadeTitlesIn(float ft)
    {
        midfade = true;
        foreach (GameObject page in titleCanvases)
        {
            StartCoroutine(fadeObjectIn(page, ft));
            yield return new WaitForSecondsRealtime(7);
            StartCoroutine(fadeObjectOut(page, titleTime));
            //yield return new WaitForSecondsRealtime(5);
        }
        midfade = false;
    }

    //play slideshow and audio, show title, then fade all three.
    public IEnumerator startSlideshow()
    {
        //play slideshow
        //fades in each slide sequentially until slides are done, and waits'
        yield return new WaitForSecondsRealtime(2);
        StartCoroutine(fadeEachIn(fadeTime));
        slidesDone = false;

        while (!slidesDone)
        {
            yield return new WaitForSecondsRealtime(1);
        }
        //if this is the title or ending sequence, play title as well
        if (hasTitle)
        {

            //fade out slides and fade in title at the same time
            StartCoroutine(fadeEachOut(fadeTime));
            StartCoroutine(fadeTitlesIn(fadeTime));
            yield return new WaitForSecondsRealtime(5);

            /*
            StartCoroutine(fadeObjectIn(titleCanvas, fadeTime));
            yield return new WaitForSecondsRealtime(5); //tweak time as neededs
            */
            //fade out titles
            StartCoroutine(fadeObjectOut(titleCanvases[titleCanvases.Length - 1], titleTime));

        }
        yield return null;
        //update scene completion status
        sceneComplete = true;
        print("Slideshow done; time to transition");
    }

    // Use this for initialization
    void Start()
    {
        //activate slideCanvas if not active
        //slideCanvas.SetActive(true);
        //getSlides();  //get all slides if need be
        StartCoroutine(startSlideshow());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(fadeEachOut(fadeTime));
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            //StartCoroutine(fadeEachIn(fadeTime));
            StartCoroutine(startSlideshow());
        }

        if (sceneComplete && !isEnding)
        {
            //GameObject endingType = new GameObject ();
            //endingType.name = "goodEnd";
            //DontDestroyOnLoad (endingType);
            //Application.LoadLevel(Application.loadedLevel + 1);
            //make sure that we don't go too far when using this script for outro.
        }
        else if (sceneComplete && isEnding)
        {
            //trying to play credits
            //if (fadedSlides > slides.Length + titleCanvases.Length)
            //credits done, quit
            //Application.Quit();
        }
    }
}