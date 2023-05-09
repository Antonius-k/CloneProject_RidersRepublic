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

    //플레이어 접속 시 대기위치 
    [Header("readyPos")]
    public Vector3[] readyPos;
    public GameObject stpos;

    //레이스 스타트 위치
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

    //타이머 테스트
    public float currentTime;


    int currentRoomPlayerCount;
    private void Start()
    {
        state = GameState.WaitingPlayer;

        //OnPhotonSerializeView 호출 빈도
        PhotonNetwork.SerializationRate = 60;
        //Rpc 호출 빈도
        PhotonNetwork.SendRate = 60;

        readyPos = new Vector3[(int)PhotonNetwork.CurrentRoom.MaxPlayers];

        rb = bike.gameObject.GetComponent<Rigidbody>();
        controlTimer = bike.gameObject.GetComponent<ControlTimer>();
        controller = bike.gameObject.GetComponent<BicycleControllerYDW>();

        //생성 위치 지정
        for (int i = 0; i < readyPos.Length; i++)
        {
            readyPos[i] = transform.position + transform.right * 5;

            //readyPos[i] = stpos.transform.position;

        }
        //현재 방에 들어와있는 인원수를 이용해서 idx 구하자
        currentRoomPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        Vector3 startPostion = new Vector3(69, 1, 1125);

        //====================================수정 전
        //생성위치
        //플레이어를 생성한다(전체인원 다 들어오기 전 위치)
        //if(player.Length <= PhotonNetwork.CurrentRoom.MaxPlayers)
        PhotonNetwork.Instantiate("BikeYDW", startPostion, Quaternion.identity);
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Owner.UserId == PhotonNetwork.MasterClient.UserId)
            {
                players[i].gameObject.name = "이동우";
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

                    //플레이어 리스트의 배열에 담긴 닉네임과 laptime 을 가져오고싶다
                    dictionary.Add(players[i].Owner.NickName, players[i].gameObject.GetComponent<Timer>().time);
                }
                check = true;
            }
        }

        if (check)
        {
            for (int i = 0; i < players.Count; i++)
            {

                //플레이어 리스트의 배열에 담긴 닉네임과 laptime 을 가져오고싶다
                dictionary[players[i].Owner.NickName] = players[i].gameObject.GetComponent<Timer>().time;
            }

        }


        /*      foreach (KeyValuePair<string, float> data in dictionary)
              {
                  Debug.Log(data.Key + "의 플레이어 이름" + data.Value + " 시간 값");
              }*/
        #region 딕션너리 제외 부분/ 상태머신
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

        //속력값 출력
        //currentSpeedText.text = ((int)rb.velocity.magnitude).ToString();
        ap = targetPosition.GetComponent<RectTransform>().anchoredPosition.x;
        ap2 = targetPositionforEnd.GetComponent<RectTransform>().anchoredPosition.x;
        switch (state)
        {

            //접속 대기 위치
            case GameState.WaitingPlayer:
                WaitingPlayer();
                break;

            //카운트 다운
            case GameState.Ready:
                GameReady();
                break;

            //바이크 레이스 시작 위치
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

        //value 로 정렬 
        sortdic = new Dictionary<string, float>();
        sortdic = sortedDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        //현재 자신의 순위?
        //ismind일때의 순위?
        // 여기 수정 부분


        


    }
    public TextMeshProUGUI rank1st;
    public GameObject rank1stGameObject;

    public SortedDictionary<string, float> sortedDictionary;
    //1. 방 인원수만큼 play씬 입장(구석탱이에 생성)
    //2. 모든 인원 방 입장 완료되면 
    //3. 시네머신 빠밤
    //4. 시네머신 후 스타트위치에 배치후 GameState.
    private void WaitingPlayer()
    {
        //플레이어수와 = 맵인원수가 맞으면 State 이동
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            movieObj.SetActive(true);
            video.Play();
            video.SetDirectAudioVolume(0, 0);
            
            state = GameState.Start;
            //오너의 이름은 이동우로
        }
    }

    //인원 다 들어오면 체크 시키고 넘겨
    bool maxPlayersCheck;

    //씨네머신 영상 뿌리고 바이크들 레이스 위치 시키고
    private void GameStart()
    {
        currentTime += Time.deltaTime;

        

        //인원수 다 들어오면 카운트 다운 시작
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //게임 준비 카운트 다운 -> 인원이 꽉 찼다ui
            //StartCoroutine("ReadyCount");
        }
        
        //플레이어 체크 되면 끝나면 경기 시작
        //if (currentTime > 10f && maxPlayersCheck == true)
        if (currentTime > 27f)
        {
            movieObj.SetActive(false);


            state = GameState.Ready;
            StartCoroutine("StartCount");
            //만약에 인원이 다들어왔으면 


            StartCoroutine("Caption");



            currentTime = 0;
        }
    }

    //레이스시작 카운트 다운 시작
    private void GameReady()
    {
        //자막재생
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

    //피니시 라인 도착
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
        //차량 도착 마지막 게이트 라인
        Transform checkpointTransform = transform.Find("EndGate");
        foreach (Transform transform in checkpointTransform)
        {
            //도착하는 차량의 시간을 가져온다
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

        
        //카운트 다운이 끝나면
        maxPlayersCheck = true;


        yield return new WaitForSeconds(1);
    }

    RectTransform[] rectTransform;

    IEnumerator ReadyCount()
    {
        //참가 인원 대기중 입니다.
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
            print(item.Key + " 1등 등수");
            print(item.Value + " 1등 시간");
            rank1st.GetComponent<TextMeshProUGUI>().text += $"{count}등 {item.Key} : {item.Value.ToString()} \n";

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
        GameManagerforProject.Instance.captionText.text = "레드불이 날개를 달아줘요!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "레드불 척추를 끊어줘요!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "예명공쥬: 1UBD == 17만!!";
        yield return new WaitForSeconds(5f);
        GameManagerforProject.Instance.captionText.text = "동우갓: 집에 보내줘...";
        yield return new WaitForSeconds(5f);

        //StopAllCoroutines();

        

    }

    //현재 방에 있는 Player를 담아놓자.
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
