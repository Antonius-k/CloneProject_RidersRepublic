using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatItem : MonoBehaviour
{
    //Text 
    public Text chatText;
    //RectTransform
    public RectTransform rt;

    void Awake()
    {
        chatText = GetComponent<Text>();
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rt.sizeDelta.y != chatText.preferredHeight)
        {
            //chatText.text 의  height 크기에 맞게 ContetSize를변경
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, chatText.preferredHeight);
        }
    }
    public void SetText(string chat)
    {
        //텍스트 세팅
        chatText.text = chat;

    }
}
