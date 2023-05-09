using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class CheckpointSingle : MonoBehaviourPun
{
    //���� �÷��̾�� üũ ����Ʈ ����Ʈ�� �޾Ƽ�
    //�������� �ϳ��ϳ� üũ����
    TrackCheckpoints trackCheckpoints;


    [SerializeField]
    public float TrickScore { get; set; }

    //public RaceManager raceManager;
    private void Awake()
    {
        trackCheckpoints = GetComponent<TrackCheckpoints>();
    }

    //üũ ����Ʈ�� �ε��� ����� �÷��̾� ���� Ȯ���Ѵ�.
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player player))
        {
            trackCheckpoints.PlayerThroughCheckpoint(this);

        }
        print("TrickScore: " + TrickScore);
        //������ ���ķ� ����
        /*else if (other.gameObject.CompareTag("CurrentPostion"))
        {
            crossedBike += 1;
            raceManager.UpdatedBikeCurrentPosition(PlayerNumbers, crossedBike);
        }*/
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }


    private void Update()
    {
        DictanceArrayColletion();

    }

    public float[] DistanceArrays;
    [Header("����ũ1 == �÷��̾��� ����ũ 1")]
    public Transform[] Bike;
    public TextMeshProUGUI Bike01Text;
    public Transform Bike01;
    public Transform Bike02;

    float first;
    float second;
    public void DictanceArrayColletion()
    {


        DistanceArrays[0] = Vector3.Distance(transform.position, Bike01.position);
        DistanceArrays[1] = Vector3.Distance(transform.position, Bike02.position);

        Array.Sort(DistanceArrays);

        first = DistanceArrays[0];
        second = DistanceArrays[1];

        float Bike01Dist = Vector3.Distance(transform.position, Bike01.position);
        float Bike02Dist = Vector3.Distance(transform.position, Bike02.position);
        if (Bike01Dist == first)
        {
            Bike01Text.text = "1/2";
        }
        if (Bike01Dist == second)
        {
            Bike01Text.text = "2/2";
        }
    }
}
