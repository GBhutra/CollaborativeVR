﻿using UnityEngine;
using System.Collections;

public class DetermineEnding : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject gePlayer = GameObject.Find("GESlideshow Controller");
        GameObject bePlayer = GameObject.Find("BESlideshow Controller");

        gePlayer.SetActive(false);
        bePlayer.SetActive(false);

        if (GameObject.Find("goodEnd") != null)
        {
            //play good ending
            //Console.Log("We have a good ending here!");
            gePlayer.SetActive(true);
            CallSlideshow cshow = (CallSlideshow)gePlayer.GetComponent(typeof(CallSlideshow));
            StartCoroutine(cshow.startSlideshow());
        }
        else if (GameObject.Find("badEnd") != null)
        {
            //play bad ending
            //Console.Log("We have a bad ending here!");
            bePlayer.SetActive(true);
            CallSlideshow cshow = (CallSlideshow)bePlayer.GetComponent(typeof(CallSlideshow));
            StartCoroutine(cshow.startSlideshow());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
