using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;
using System.Collections;

public class ChatManager : MonoBehaviourPun
{

    //chatItem 공장을 가여오는 변수
    public GameObject chatItemFacotry;
    //InputChat 
    public InputField inputchattingText;
    //ScrollView Contents의 transform
    public RectTransform trContent;
    Color idColor;
    private void Start()
    {
        //inputChat 에서 Enter 키를 눌렀을때 호출되는 함수 등력
        inputchattingText.onSubmit.AddListener(OnEntertoSend);
        Cursor.visible = false;
        idColor = new Color(
           Random.Range(0.0f, 1.0f),
           Random.Range(0.0f, 1.0f),
           Random.Range(0.0f, 1.0f)
       );
    }
    private void Update()
    {
        // "Tab"키 누르면 커서를 활성화
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            //만약에 커서가 해당 위치에 UI가 없을때 PCmode
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Cursor.visible = false;
            }

            //모바일 버전
            /*if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false)
            { }*/

        }
    }
    //inputChat 에서 Enter 키를 눌렀을때 호출되는 함수
    void OnEntertoSend(string s)
    {
        string chatText = "<color=#" + ColorUtility.ToHtmlStringRGB(idColor) + ">" + PhotonNetwork.NickName + "</color>" + " : " + s;
        photonView.RPC("RPCChat", RpcTarget.All, chatText);
        // inputCHat의 내용을 초기화한다.
        inputchattingText.text = "";
        // inputChat 선택되도록 한다.
        inputchattingText.ActivateInputField();
    }
    //스크롤 뷰 H
    public RectTransform rtScrollview;
    //변경되기전 content H
    float previousContentHValue;
    [PunRPC]
    void RPCChat(string chatText)
    {
        // 이전 content의 H 값을 저장하자(추가전의 사이즈)
        previousContentHValue = trContent.sizeDelta.y;
        //1. ChatItem을 만든다(부모를 Scorllview의 Content)
        GameObject item = Instantiate(chatItemFacotry, trContent);
        ChatItem chatItem = item.GetComponent<ChatItem>();
        chatItem.SetText(chatText);
        // 이전의 바닥에 닿아있다면
        StopAllCoroutines();
        StartCoroutine("AutoScrolltoBelow");
    }
    IEnumerator AutoScrolltoBelow()
    {
        yield return null;
        //스크롤 뷰 H보다 content H 값이 클때만 (스크롤 가능한 상태라면)
        if (trContent.sizeDelta.y > rtScrollview.sizeDelta.y)
        {
            // 이전에 바닥에 닿아 있었다면 (변경되기전 content H - 스크롤 뷰 H <= content y)(추가가 되기전의 값을 이용해 계산) H2-H1
            if (trContent.anchoredPosition.y >= previousContentHValue - rtScrollview.sizeDelta.y)
            {
                // 추가된 높이 만큼 content y 값을 변경하겠다. (추가된후, 값을 이용하여 계산) H3-H1
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - rtScrollview.sizeDelta.y);
            }
        }
        yield break;
    }
}

