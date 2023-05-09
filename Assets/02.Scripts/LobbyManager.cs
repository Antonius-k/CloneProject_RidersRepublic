using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    //방 이름 InputField
    public InputField inputRoomName;
    //총 인원 InputField
    public InputField inputMaxPlayer;
    //방 참여 Button
    public Button btnJoin;
    //방 생성 Button
    public Button btnCreate;

    //방 목록 Content
    public Transform roomListContent;

    //방들의 정보
    Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();
    //방정보 아이템 Prefab
    public GameObject roomItemFactory;
    //맵 Thumbnail 들
    public GameObject[] mapThums;

    private void Awake()
    {
       
    }
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        //inputRoomName 값이 변할때마다 호출되는 함수 등록
        inputRoomName.onValueChanged.AddListener(OnRoomNameValueChanged);
        //inputMaxPlayer 값이 변할때마다 호출되는 함수 등록
        inputMaxPlayer.onValueChanged.AddListener(OnMaxPlayerValueChanged);
        string[] s = Microphone.devices;
      /*  if (PhotonNetwork.IsMasterClient)
        {
            nicknames.Add(PhotonNetwork.NickName);
        }*/
    }

    void OnRoomNameValueChanged(string room)
    {
        //참여 버튼 활성화
        btnJoin.interactable = room.Length > 0;
        //생성 버튼 활성화
        btnCreate.interactable = room.Length > 0 && inputMaxPlayer.text.Length > 0;
    }

    void OnMaxPlayerValueChanged(string max)
    {
        //생성 버튼 활성화
        btnCreate.interactable = max.Length > 0 && inputRoomName.text.Length > 0;
    }

    //방 생성
    public void CreateRoom()
    {
        // 방 옵션을 설정
        RoomOptions roomOptions = new RoomOptions();
        // 최대 인원 (0이면 최대인원)
        roomOptions.MaxPlayers = byte.Parse(inputMaxPlayer.text);
        // 룸 리스트에 보이지 않게? 보이게?
        roomOptions.IsVisible = true;
        // Custom 룸 정보를 설정
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

        //hash["description"] = $"{PhotonNetwork.LocalPlayer.NickName}은 인원수 {PhotonNetwork.CurrentRoom.MaxPlayers} 매칭 기다리는 중.. + {Random.Range(1, 1000)}";
        hash["desc"] = "여긴 초보방이다! " + Random.Range(1, 1000);
        hash["mapId"] = Random.Range(0, mapThums.Length);
        roomOptions.CustomRoomProperties = hash;
        // Custom 정보 키값을 보이게 하겠다.
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "description", "mapId" };

        // 방 생성 요청 (해당 옵션을 이용해서)
        PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions);
    }

    //방이 생성되면 호출 되는 함수
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("OnCreatedRoom");
    }

    //방 목록이 변경되었을 때 (생성, 삭제, 정보갱신) 호출 해주는 함수
    //RoomListUpdate는 로비에서만 호출할 수 있음
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("방 목록 변경");
        base.OnRoomListUpdate(roomList);

        //룸리스트 UI 삭제
        RemoveRoomListUI();
        //룸리스트 정보 갱신
        UpdateRoomCache(roomList);
        //룸리스트 UI 생성
        CreateRoomListUI();
    }

    //방 생성이 실패 될때 호출 되는 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("OnCreateRoomFailed , " + returnCode + ", " + message);

    }

    //방 참가 요청
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputRoomName.text);
    }


    //방장이 인원이 다차면 시작 누르면 호출되는함수 
    public void OnStart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //플레이어수와 = 맵인원수가 맞으면 씬 이동
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.LoadLevel("PlayScene");
            }
        }
    }
  /* public static List<string> nicknames = new List<string>();
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        nicknames.Add(newPlayer.NickName);
        for (int i = 0; i < nicknames.Count; i++)
        {
            print(nicknames[i]);
        }
    }*/
    //방 참가가 완료 되었을 때 호출 되는 함수
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("OnJoinedRoom");
        PhotonNetwork.LoadLevel("PlayScene");
    }

    //방 참가가 실패 되었을 때 호출 되는 함수
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("OnJoinRoomFailed, " + returnCode + ", " + message);
    }

    void RemoveRoomListUI()
    {
        foreach (Transform tr in roomListContent)
        {
            Destroy(tr.gameObject);
        }

        //Button[] tr = roomListContent.GetComponentsInChildren<Button>();
        //for (int i = 0; i < tr.Length; i++)
        //    Destroy(tr[i].gameObject);
    }

    void UpdateRoomCache(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            //만약에 roomCache에 info.Name에 해당되는 키값이 있다면
            if(roomCache.ContainsKey(info.Name))
            {
                //만약에 해당 룸이 삭제되는 것이라면
                if(info.RemovedFromList)
                {
                    //roomCache 에서 삭제
                    roomCache.Remove(info.Name);
                }
                //그렇지 않다면
                else
                {
                    //정보갱신
                    roomCache[info.Name] = info;
                }
            }
            //그렇지 않다면
            else
            {
                //roomCache 에 추가
                roomCache[info.Name] = info;
            }
        }
    }

    void CreateRoomListUI()
    {
        foreach(RoomInfo info in roomCache.Values)
        {
            //방정보 아이템 만들고
            GameObject go = Instantiate(roomItemFactory, roomListContent);

            //아이템 정보 셋팅
            RoomItem roomItem = go.GetComponent<RoomItem>();
            roomItem.SetInfo(info);
            roomItem.onClickAction = SelectRoom;

            //Custom 정보 셋팅
            string des = (string)info.CustomProperties["description"];
            int mapId = (int)info.CustomProperties["mapId"];
            print(des + ", " + mapId);

            //람다식
            //roomItem.onClickAction = (string room) => {
            //    inputRoomName.text = room;
            //};
        }
    }

    int prevMapId = -1;
    void SelectRoom(string room, int mapId)
    {
        //방이름 셋팅
        inputRoomName.text = room;

        //만약에 이전에 선택한 맵이 있다면
        if(prevMapId > -1)
        {
            //해당 맵을 비활성화
            mapThums[prevMapId].SetActive(false);
        }

        //맵 thumbnail 셋팅
        mapThums[mapId].SetActive(true);
        //prevMapId <- mapId 셋팅
        prevMapId = mapId;
    }

    

}
