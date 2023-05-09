using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("GameObject")]
    public GameObject player;
    public GameObject trickPointsUI;
    public GameObject trickcheckDistance;

    [Header("UI")]
    public TextMeshProUGUI checkPointStart;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI trickCheckPoints;
    public TextMeshProUGUI checkPointDistance;

    Camera mainCam;
    GameObject cam;
    

    
    void Start()
    {
        //player.GetComponent<Timer>().curTimeText = checkPointStart;
    }

    void Update()
    {
        speed.text = player.GetComponent<Controller>().currentSpeed.ToString();
        player.GetComponent<Player>().trickScoreText = trickCheckPoints;
        player.GetComponent<Player>().trickPointUI = trickPointsUI;
        //trickcheckDistance.gameObject.GetComponent<TrackCheckpoints>().distance = checkPointDistance.text;
    }
}
