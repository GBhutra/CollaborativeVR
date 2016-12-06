using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneAssetManager : MonoBehaviour {

    private string caveScene = "caveScene";
    private string introScene = "IntroScene";
    private string outroScene = "OutroScene";
    private Vector3 caveViewer1Position = new Vector3(-26.93252f, 0.3000002f, -26.63223f);
    private Vector3 caveViewer2Position = new Vector3(-29.93252f, 0.3000002f, -24.63223f);
    private bool caveSceneViewerPositioning = false;
    private bool caveViewer1Positioning = false;
    private bool caveViewer2Positioning = false;
    private bool introSceneViewerPositioning = false;
    private bool introViewer1Positioning = false;
    private bool introViewer2Positioning = false;
    private Vector3 introViewer1Position = new Vector3(-3.39525f, 0.96f, 4.989603f);
    private Vector3 introViewer2Position = new Vector3(-1.61f, 0.96f, 3.53f);

    void Awake ()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!caveSceneViewerPositioning)
        {
            
            if (SceneManager.GetActiveScene().name == caveScene)
            {
                if (!caveViewer1Positioning && TagPlayers.foundFirstPlayer)
                {
                    GameObject.FindGameObjectWithTag("FirstPlayer").transform.position = caveViewer1Position;
                    GameObject.FindGameObjectWithTag("FirstPlayer").GetComponent<PlayersBehaviourScript>().enabled = true;
                    caveViewer1Positioning = true;
                }
                if (!caveViewer2Positioning && TagPlayers.foundSecondPlayer)
                {
                    GameObject.FindGameObjectWithTag("SecondPlayer").transform.position = caveViewer2Position;
                    GameObject.FindGameObjectWithTag("SecondPlayer").GetComponent<PlayersBehaviourScript>().enabled = true;
                    caveViewer2Positioning = true;
                }
                if (caveViewer1Positioning && caveViewer2Positioning)
                {
                    caveSceneViewerPositioning = true;
                }

            }
  
        }

        if (!introSceneViewerPositioning)
        {
            if (SceneManager.GetActiveScene().name == introScene)
            {
                if (!introViewer1Positioning && TagPlayers.foundFirstPlayer)
                {
                    GameObject.FindGameObjectWithTag("FirstPlayer").transform.position = introViewer1Position;
                    introViewer1Positioning = true;
                }
                if (!introViewer2Positioning && TagPlayers.foundSecondPlayer)
                {
                    GameObject.FindGameObjectWithTag("SecondPlayer").transform.position = introViewer2Position;
                    introViewer2Positioning = true;
                }
                if (introViewer1Positioning && introViewer2Positioning)
                {
                    introSceneViewerPositioning = true;
                }

            }
           
        }



    }
}
