using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    //닉네임 InputField
    public InputField inputNickName;
    //접속 Button
    public Button btnConnect;

    void Start()
    {
        //inputNickName 값이 변할때마다 호출되는 함수 등록
        inputNickName.onValueChanged.AddListener(OnValueChanged);
        //inputNickName에서 Enter키 누르면 호출되는 함수 등록
        inputNickName.onSubmit.AddListener(OnSubmit);
        //inputNickName에서 Focusing이 사라졌을 때 호출되는 함수 등록
        inputNickName.onEndEdit.AddListener(OnEndEdit);
    }

    void OnValueChanged(string s)
    {        
        //만약에 s의 길이가 0보다 크면
        //버튼을 동작하게 설정
        //그렇지 않으면
        //버튼을 동작하지 않게 설정
        btnConnect.interactable = s.Length > 0;
        print("OnValueChanged : " + s);
    }

    void OnSubmit(string s)
    {
        if(s.Length > 0)
        {
            OnClickConnect();
        }
        print("OnSubmit : " + s);
    }

    void OnEndEdit(string s)
    {
        print("OnEndEdit : " + s);
    }

    public void OnClickConnect()
    {
        //서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }

    //마스터 서버 접속성공시 호출(Lobby에 진입할 수 없는 상태)
    public override void OnConnected()
    {
        base.OnConnected();
        print(System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    //마스터 서버 접속성공시 호출(Lobby에 진입할 수 있는 상태)
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //내 닉네임 설정
        PhotonNetwork.NickName = inputNickName.text; //"김현진_" + Random.Range(1, 1000);
        //로비 진입 요청
        PhotonNetwork.JoinLobby();
    }

    //로비 진입 성공시 호출
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //LobbyScene으로 이동
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
