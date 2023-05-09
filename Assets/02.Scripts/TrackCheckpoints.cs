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

    //üũ ����Ʈ����Ʈ
    List<CheckpointSingle> checkpointSingleList;
    //���� ����Ʈ �ε��� �ѹ�
    int nextCheckpointSingleIndex;

    //�������� �÷��̾ �������??
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

        //���� üũ����Ʈ�� ��ġ�� �޾ƿ´�.
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointSingleList = new List<CheckpointSingle>();

        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            print(checkpointSingleTransform);
            CheckpointSingle checkpointSingle = checkpointSingleTransform.GetComponent<CheckpointSingle>();

            checkpointSingle.SetTrackCheckpoints(this);

            checkpointSingleList.Add(checkpointSingle);
        }

        //ó�� üũ ����Ʈ �ε���(�Ѵ��� ����)
        nextCheckpointSingleIndex = 0;


        //�������� ����
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

    //�� üũ����Ʈ�� ����ġ�� ȣ��Ǵ� �Լ�
    public void PlayerThroughCheckpoint(CheckpointSingle checkpointSingle)
    {
        //Debug.Log(checkpointSingle.transform.name);
        //����Ʈ�� �ε��� �ѹ��� ������ üũ����Ʈ�� 0,1,2..������ �޾ƿ´�.
        Debug.Log(checkpointSingleList.IndexOf(checkpointSingle));

        //1.���� ���� üũ����Ʈ�� �ε����� ���ٸ�
        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex)
        {
            //2.Correct checkpoint
            print(nextCheckpointSingleIndex);
            //3.�´ٸ� �ε��� ���� ���Ѽ� ���� ������ ���� üũ�Ѵ�.
            //�ٽ� ù��° üũ����Ʈ�� ���ư��� correctüũ�ϱ� ����
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            StartCoroutine("TrickPointUI");
        }
        //Ʋ�ȴٸ�(wrong checkpoint)
        else
        {
            //�߸������� Image����
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

    //�÷��̾�� ����Ʈ�������� �Ÿ� ���
    public void PlayerCheckpointDistance()
    {
        playerPos = GameObject.FindWithTag("Bike");

        //ù��° Ÿ�ٰ� �÷��̾�� �Ÿ�
        //ù��° Ÿ�ٿ� �����ϸ� ����üũ����Ʈ ������ �Ÿ� ����
        distance = Vector3.Distance(playerPos.GetComponent<Transform>().transform.position, checkpointSingleList[nextCheckpointSingleIndex].transform.position);
        checkPointText.text = ((int)distance).ToString() + "m/" + PhotonNetwork.NickName;

        //�� �Ƚ��Ǹ� ������ üũ����Ʈ�� �����ϸ� UI���ִ� ó�� ���ְ�

    }


}
