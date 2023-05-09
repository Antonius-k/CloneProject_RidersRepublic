using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Realtime;

public class RoomItem : MonoBehaviour
{
    //방정보 Text
    public Text roomInfo;
    //방설명 Text
    public Text roomDescription;

    //클릭 되었을 때 호출되야 하는 함수를 담을 변수
    public Action<string, int> onClickAction;

    //map Id
    int mapId;

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void SetInfo(string roomName, int currPlayer, byte maxPlayer)
    {
        //자신의 게임오브젝트 이름을 roomName으로
        name = roomName;
        //방정보 셋팅 => 방 이름 ( 1 / 10) 
        roomInfo.text = roomName + " (" + currPlayer + " / " + maxPlayer + ")";
    }

    public void SetInfo(RoomInfo info)
    {
        SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);

        //방 설명 셋팅
        roomDescription.text = (string)info.CustomProperties["description"];

        //맵 id 셋팅
        mapId = (int)info.CustomProperties["mapId"];
    }



    public void OnClick()
    {
        //만약에 onClickAction이 null이 아니라면
        if(onClickAction != null)
        {
            //onClickAction 에 들어있는 함수를 호출
            onClickAction(name, mapId);
        }

        ////InputRoomName 에 roomName을 전달해서 셋팅
        ////1. InputRoomName 게임오브젝트 찾자
        //GameObject 인풋룸네임 = GameObject.Find("InputRoomName"); 
        ////2. 찾은 게임 오브젝트에서 InputField 컴포넌트 가져오자
        //InputField input = 인풋룸네임.GetComponent<InputField>();
        ////3. 가져온 컴포넌트에서 text을 roomName으로 하자
        //input.text = name;
    }
}
