using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;
using System.Collections;

public class ChatManager : MonoBehaviourPun
{

    //chatItem ������ �������� ����
    public GameObject chatItemFacotry;
    //InputChat 
    public InputField inputchattingText;
    //ScrollView Contents�� transform
    public RectTransform trContent;
    Color idColor;
    private void Start()
    {
        //inputChat ���� Enter Ű�� �������� ȣ��Ǵ� �Լ� ���
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
        // "Tab"Ű ������ Ŀ���� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            //���࿡ Ŀ���� �ش� ��ġ�� UI�� ������ PCmode
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Cursor.visible = false;
            }

            //����� ����
            /*if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) == false)
            { }*/

        }
    }
    //inputChat ���� Enter Ű�� �������� ȣ��Ǵ� �Լ�
    void OnEntertoSend(string s)
    {
        string chatText = "<color=#" + ColorUtility.ToHtmlStringRGB(idColor) + ">" + PhotonNetwork.NickName + "</color>" + " : " + s;
        photonView.RPC("RPCChat", RpcTarget.All, chatText);
        // inputCHat�� ������ �ʱ�ȭ�Ѵ�.
        inputchattingText.text = "";
        // inputChat ���õǵ��� �Ѵ�.
        inputchattingText.ActivateInputField();
    }
    //��ũ�� �� H
    public RectTransform rtScrollview;
    //����Ǳ��� content H
    float previousContentHValue;
    [PunRPC]
    void RPCChat(string chatText)
    {
        // ���� content�� H ���� ��������(�߰����� ������)
        previousContentHValue = trContent.sizeDelta.y;
        //1. ChatItem�� �����(�θ� Scorllview�� Content)
        GameObject item = Instantiate(chatItemFacotry, trContent);
        ChatItem chatItem = item.GetComponent<ChatItem>();
        chatItem.SetText(chatText);
        // ������ �ٴڿ� ����ִٸ�
        StopAllCoroutines();
        StartCoroutine("AutoScrolltoBelow");
    }
    IEnumerator AutoScrolltoBelow()
    {
        yield return null;
        //��ũ�� �� H���� content H ���� Ŭ���� (��ũ�� ������ ���¶��)
        if (trContent.sizeDelta.y > rtScrollview.sizeDelta.y)
        {
            // ������ �ٴڿ� ��� �־��ٸ� (����Ǳ��� content H - ��ũ�� �� H <= content y)(�߰��� �Ǳ����� ���� �̿��� ���) H2-H1
            if (trContent.anchoredPosition.y >= previousContentHValue - rtScrollview.sizeDelta.y)
            {
                // �߰��� ���� ��ŭ content y ���� �����ϰڴ�. (�߰�����, ���� �̿��Ͽ� ���) H3-H1
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - rtScrollview.sizeDelta.y);
            }
        }
        yield break;
    }
}

