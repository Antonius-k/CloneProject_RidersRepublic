using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;


public class TrackCheckpoints : MonoBehaviourPun
{
    [Header("CheckPointDistance")]
    public TextMeshProUGUI checkPointText;

    //체크 포인트리스트
    List<CheckpointSingle> checkpointSingleList;
    //다음 포인트 인덱스 넘버
    int nextCheckpointSingleIndex;

    //여러명의 플레이어가 있을경우??
    [SerializeField]
    List<Transform> carTransformList;

    List<int> nextCheckpointSingleIndexList;

    public Player player;
    public GameObject bike;


    public float distance;
    public GameObject playerPos;
    public GameObject a;
    public TextMeshProUGUI trickScoreText;
    public float TrickScore { get; set; }

    private void Awake()
    {

        player = bike.GetComponent<Player>();
        //player = GameObject.FindWithTag("Bike").GetComponent<Player>();

        //맵의 체크포인트의 위치를 받아온다.
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            print(checkpointSingleTransform);
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
        }

        //처음 체크 포인트 인덱스(한대의 차량)
        nextCheckpointSingleIndex = 0;


        //여러대의 차량
        nextCheckpointSingleIndexList = new List<int>();
        foreach (Transform carTransform in carTransformList)
        {
            nextCheckpointSingleIndexList.Add(0);
        }
    }



    private void Update()
    {
        if(GameManagerforProject.Instance.state == GameManagerforProject.GameState.Play)
        {
            PlayerCheckpointDistance();
        }
    }

    //각 체크포인트를 지나치면 호출되는 함수
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        //Debug.Log(checkpointSingle.transform.name);
        //리스트의 인덱스 넘버로 지나간 체크포인트를 0,1,2..순서로 받아온다.
        Debug.Log(checkpointSingleList.IndexOf(checkpointSingle));

        //1.만약 다음 체크포인트의 인덱스와 같다면
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            //2.Correct checkpoint
            print(nextCheckpointSingleIndex);
            //3.맞다면 인덱스 증가 시켜서 다음 가야할 곳을 체크한다.
            //다시 첫번째 체크포인트로 돌아가도 correct체크하기 위해
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            StartCoroutine("TrickPointUI");
        }
        //틀렸다면(wrong checkpoint)
        else
        {
            //잘못갔을때 Image띄우기
            print("wrong");
        }
    }

    IEnumerator TrickPointUI()
    {
        a.SetActive(true);
        TrickScore += 1000;
        trickScoreText.text = TrickScore.ToString() + " PTS";
        yield return new WaitForSeconds(2f);

        a.SetActive(false);
        StopCoroutine("TrickPointUI");
    }

    //플레이어와 포인트지점들의 거리 출력
    public void PlayerCheckpointDistance()
    {
        playerPos = GameObject.FindWithTag("Bike");

        //첫번째 타겟과 플레이어간의 거리
        //첫번째 타겟에 도착하면 다음체크포인트 까지의 거리 갱신
        distance = Vector3.Distance(playerPos.GetComponent<Transform>().transform.position, checkpointSingleList[nextCheckpointSingleIndex].transform.position);
        checkPointText.text = ((int)distance).ToString() + "m/" + PhotonNetwork.NickName;

        //맵 픽스되면 마지막 체크포인트에 도착하면 UI꺼주는 처리 해주고

    }


}
