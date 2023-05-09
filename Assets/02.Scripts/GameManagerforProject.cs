using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using UnityEngine.UI;
using YDWcripts;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine.Video;


public class GameManagerforProject : MonoBehaviourPunCallbacks
{
    public static GameManagerforProject Instance;
    ControlTimer controlTimer;
    public BicycleControllerYDW controller;
    public Rigidbody rb;
    public GameObject bike;


    //MapView
    public GameObject movieObj;
    public VideoPlayer video;

    /* public GameObject player;
     List<GameObject> playerList = new List<GameObject>();*/

    // public GameObject[];
    private void Awake()
    {
        if (Instance == null)
            Instance = this;


    }
    public enum GameState
    {
        None,
        Start,
        WaitingPlayer,
        Play,
        Stop,
        Ready
    }

    public GameState state;
    [Header("StartCountDown")]
    public GameObject[] imageCount;
    public GameObject targetPosition;

    [Header("UI")]
    public GameObject currentSpeedObj;
    public TextMeshProUGUI currentSpeedText;
    public GameObject mapNameObj;
    public GameObject RankObj;
    public GameObject countDownObj;
    public GameObject checkPointDisObj;

    float ap;

    //startricepos countdown
    public RectTransform[] rt;

    [Header("PlayTime")]
    public GameObject curTimeObj;

    //�÷��̾� ���� �� �����ġ 
    [Header("readyPos")]
    public Vector3[] readyPos;
    public GameObject stpos;

    //���̽� ��ŸƮ ��ġ
    [Header("StartLine")]
    public GameObject[] startLine;


    //EndLineUI
    public RectTransform[] endLine;
    public GameObject[] imageEndcount;
    float ap2;
    public GameObject targetPositionforEnd;


    [Header("Caption")]
    public GameObject captionObj;
    public TextMeshProUGUI captionText;
    public TextMeshProUGUI currentTimeUIINCOntrollTimer;

    public static Dictionary<string, float> dictionary = new Dictionary<string, float>();
    Dictionary<string, float> sortdic;

    //Ÿ�̸� �׽�Ʈ
    public float currentTime;


    int currentRoomPlayerCount;
    private void Start()
    {
        state = GameState.WaitingPlayer;

        //OnPhotonSerializeView ȣ�� ��
        PhotonNetwork.SerializationRate = 60;
        //Rpc ȣ�� ��
        PhotonNetwork.SendRate = 60;

        readyPos = new Vector3[(int)PhotonNetwork.CurrentRoom.MaxPlayers];

        rb = bike.gameObject.GetComponent<Rigidbody>();
        controlTimer = bike.gameObject.GetComponent<ControlTimer>();
        controller = bike.gameObject.GetComponent<BicycleControllerYDW>();

        //���� ��ġ ����
        for (int i = 0; i < readyPos.Length; i++)
        {
            readyPos[i] = transform.position + transform.right * 5;

            //readyPos[i] = stpos.transform.position;

        }
        //���� �濡 �����ִ� �ο����� �̿��ؼ� idx ������
        currentRoomPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        Vector3 startPostion = new Vector3(69, 1, 1125);

        //====================================���� ��
        //������ġ
        //�÷��̾ �����Ѵ�(��ü�ο� �� ������ �� ��ġ)
        //if(player.Length <= PhotonNetwork.CurrentRoom.MaxPlayers)
        PhotonNetwork.Instantiate("BikeYDW", startPostion, Quaternion.identity);
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Owner.UserId == PhotonNetwork.MasterClient.UserId)
            {
                players[i].gameObject.name = "�̵���";
            }
        }



    }

    bool check = false;

    // Update is called once per frame
    void Update()
    {
        if(players.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if(check == false)
            {
                for (int i = 0; i < players.Count; i++)
                {

                    //�÷��̾� ����Ʈ�� �迭�� ��� �г��Ӱ� laptime �� ��������ʹ�
                    dictionary.Add(players[i].Owner.NickName, players[i].gameObject.GetComponent<Timer>().time);
                }
                check = true;
            }
        }

        if (check)
        {
            for (int i = 0; i < players.Count; i++)
            {

                //�÷��̾� ����Ʈ�� �迭�� ��� �г��Ӱ� laptime �� ��������ʹ�
                dictionary[players[i].Owner.NickName] = players[i].gameObject.GetComponent<Timer>().time;
            }

        }


        /*      foreach (KeyValuePair<string, float> data in dictionary)
              {
                  Debug.Log(data.Key + "�� �÷��̾� �̸�" + data.Value + " �ð� ��");
              }*/
        #region ��ǳʸ� ���� �κ�/ ���¸ӽ�
        if (state == GameManagerforProject.GameState.Play)
        {
            currentSpeedObj.SetActive(true);
            curTimeObj.SetActive(true);
            captionObj.SetActive(true);
            mapNameObj.SetActive(true);
            //RankObj.SetActive(true);
            checkPointDisObj.SetActive(true);
        }
        if (state == GameManagerforProject.GameState.Ready)
        {
            countDownObj.SetActive(true);
        }

        //�ӷ°� ���
        //currentSpeedText.text = ((int)rb.velocity.magnitude).ToString();
        ap = targetPosition.GetComponent<RectTransform>().anchoredPosition.x;
        ap2 = targetPositionforEnd.GetComponent<RectTransform>().anchoredPosition.x;
        switch (state)
        {

            //���� ��� ��ġ
            case GameState.WaitingPlayer:
                WaitingPlayer();
                break;

            //ī��Ʈ �ٿ�
            case GameState.Ready:
                GameReady();
                break;

            //����ũ ���̽� ���� ��ġ
            case GameState.Start:
                GameStart();
                break;

            case GameState.Play:
                GamePlay();
                break;

            case GameState.Stop:
                GameStop();
                break;
        }
        #endregion
        //
        sortedDictionary = new SortedDictionary<string, float>(dictionary);
        //List<float> listForDic = dictionary.Values.ToList();
        //listForDic.Sort();

        //value �� ���� 
        sortdic = new Dictionary<string, float>();
        sortdic = sortedDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        //���� �ڽ��� ����?
        //ismind�϶��� ����?
        // ���� ���� �κ�


        


    }
    public TextMeshProUGUI rank1st;
    public GameObject rank1stGameObject;

    public SortedDictionary<string, float> sortedDictionary;
    //1. �� �ο�����ŭ play�� ����(�������̿� ����)
    //2. ��� �ο� �� ���� �Ϸ�Ǹ� 
    //3. �ó׸ӽ� ����
    //4. �ó׸ӽ� �� ��ŸƮ��ġ�� ��ġ�� GameState.
    private void WaitingPlayer()
    {
        //�÷��̾���� = ���ο����� ������ State �̵�
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            movieObj.SetActive(true);
            video.Play();
            video.SetDirectAudioVolume(0, 0);
            
            state = GameState.Start;
            //������ �̸��� �̵����
        }
    }

    //�ο� �� ������ üũ ��Ű�� �Ѱ�
    bool maxPlayersCheck;

    //���׸ӽ� ���� �Ѹ��� ����ũ�� ���̽� ��ġ ��Ű��
    private void GameStart()
    {
        currentTime += Time.deltaTime;

        

        //�ο��� �� ������ ī��Ʈ �ٿ� ����
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //���� �غ� ī��Ʈ �ٿ� -> �ο��� �� á��ui
            //StartCoroutine("ReadyCount");
        }
        
        //�÷��̾� üũ �Ǹ� ������ ��� ����
        //if (currentTime > 10f && maxPlayersCheck == true)
        if (currentTime > 27f)
        {
            movieObj.SetActive(false);


            state = GameState.Ready;
            StartCoroutine("StartCount");
            //���࿡ �ο��� �ٵ������� 


            StartCoroutine("Caption");



            currentTime = 0;
        }
    }

    //���̽����� ī��Ʈ �ٿ� ����
    private void GameReady()
    {
        //�ڸ����
        if (lastimageCount == true)
        {
            state = GameState.Play;
            currentTime = 0;

        }
    }
    void invokeFOrcaption()
    {
        captionObj.SetActive(false);
        StopCoroutine("Caption");

    }
    private void GamePlay()
    {
        currentTime += Time.deltaTime;
        if(currentTime > 10f)
        {
            print("222222222222222");
            captionObj.SetActive(false);
            StopAllCoroutines();
        }

        //Invoke("invokeFOrcaption", 10.0f);

        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine("FinishGameUI");
            }
        }
        if (gameEnd == true)
        {
            StartCoroutine("FinishGameUI");
            state = GameState.Stop;
        }
    }

    //�ǴϽ� ���� ����
    private void GameStop()
    {
        rb.velocity = new Vector3(0, 0, 0);
        
        //StopAllCoroutines();
        //Invoke("ChangeScene", 2.0f);
    }

    void ChangeScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartCoroutine("FinishGameUI");
            }
        }
    }

    void RacingStart()
    {
        //���� ���� ������ ����Ʈ ����
        Transform checkpointTransform = transform.Find("EndGate");
        foreach (Transform transform in checkpointTransform)
        {
            //�����ϴ� ������ �ð��� �����´�
            print("endLine check!");
        }
    }


    //Smooth time UI
    float duration = 2f;
    float second = 2f;
    public bool lastimageCount;

    //StartCountdown
    IEnumerator StartCount()
    {

        //tweening
        for (int i = 0; i < rt.Length; i++)
        {
            rt[i].gameObject.SetActive(true);
            rt[i].DOAnchorPosX(ap, duration);
            yield return new WaitForSeconds(second);
            rt[i].gameObject.SetActive(false);

        }

        //Start CountDown UI off
        imageCount[0].gameObject.SetActive(false);
        lastimageCount = true;
        countDownObj.SetActive(false);

        
        //ī��Ʈ �ٿ��� ������
        maxPlayersCheck = true;


        yield return new WaitForSeconds(1);
    }

    RectTransform[] rectTransform;

    IEnumerator ReadyCount()
    {
        //���� �ο� ����� �Դϴ�.
        for (int i = 0; i < rectTransform.Length; i++)
        {
            rectTransform[i].gameObject.SetActive(true);
            rectTransform[i].DOAnchorPosX(ap, 3f);

        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator FinishGameUI()
    {
        for (int i = 0; i < endLine.Length; i++)
        {
            endLine[i].gameObject.SetActive(true);
            endLine[i].DOAnchorPosX(ap2, duration);
            yield return new WaitForSeconds(second);
            endLine[i].gameObject.SetActive(false);
        }
        imageEndcount[0].gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);


        rank1stGameObject.SetActive(true);
        //rank1st = rank1stGameObject.GetComponentInChildren<TextMeshProUGUI>();
        //rank1st.GetComponent<TextMeshProUGUI>().text;

        int count = 1;
        foreach (KeyValuePair<string, float> item in sortdic)
        {
            print(item.Key + " 1�� ���");
            print(item.Value + " 1�� �ð�");
            rank1st.GetComponent<TextMeshProUGUI>().text += $"{count}�� {item.Key} : {item.Value.ToString()} \n";

            //rank1st.GetComponent<TextMeshProUGUI>().text = "\n";
            count += 1;

            //print(sortdic.Values.ToString() + "value");
        }

        gameEnd = false;

        yield return new WaitForSeconds(10f);

        PhotonNetwork.LoadLevel(3);
    }

    int count = 0;
    IEnumerator Caption()
    {
        count += 1;
        GameManagerforProject.Instance.captionObj.SetActive(true);
        GameManagerforProject.Instance.captionText.text = "������� ������ �޾����!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "����� ô�߸� �������!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "�������: 1UBD == 17��!!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "���찫: ���� ������...";
        yield return new WaitForSeconds(5f);

        //StopAllCoroutines();

        

    }

    //���� �濡 �ִ� Player�� ��Ƴ���.
    public List<PhotonView> players = new List<PhotonView>();
    //int b = 0;
    public void AddPlayer(PhotonView pv)
    {

        players.Add(pv);

        /* b++;
         if (b==2)
         {
             photonView.RPC("SendRPCForAllPlayers", RpcTarget.All, players[0], players[1]);
         }*/
        //if(PhotonNetwork.IsMasterClient)
    }
    /*  [PunRPC]
      void SendRPCForAllPlayers(PhotonView a, PhotonView b)
      {
          players[0] = a;
          players[1] = b;
      }*/
    public bool gameEnd;
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bike")
        {
            gameEnd = true;
        }
    }
}
