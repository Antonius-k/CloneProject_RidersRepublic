using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Cam : MonoBehaviourPun
{
    public Transform camPos;
    // Start is called before the first frame update
    void Start()
    {
        //만약에 내것이라면
        if (photonView.IsMine == true)
        {
            //camPos를 활성화 한다.
            camPos.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //만약에 내것이 아니라면 함수를 나간다.
        if (photonView.IsMine == false) return;
        if (Cursor.visible == true) return;
    }
}
