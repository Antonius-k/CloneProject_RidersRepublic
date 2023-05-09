using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Timer : MonoBehaviourPun
{

    public TextMeshProUGUI curTimeText;
    public float startTimer = 0;
    public float time;
    public GameObject canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
        curTimeText =  canvas.transform.GetChild(3).transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        if (startTimer > 0)
        {
            time += Time.deltaTime;
            var minutes = time / 60;
            var seconds = time % 60;
            var fraction = (time * 100) % 100;
            if(photonView.IsMine)
                curTimeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction); 
            
        }
    }
}
