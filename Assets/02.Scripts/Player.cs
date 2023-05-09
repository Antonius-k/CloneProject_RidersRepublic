using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    [Header("TrickScore")]
    public TextMeshProUGUI trickScoreText;
    public GameObject trickPointUI;

    public float TrickScore { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        TrickScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
