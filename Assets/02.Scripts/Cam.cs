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
        //���࿡ �����̶��
        if (photonView.IsMine == true)
        {
            //camPos�� Ȱ��ȭ �Ѵ�.
            camPos.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //���࿡ ������ �ƴ϶�� �Լ��� ������.
        if (photonView.IsMine == false) return;
        if (Cursor.visible == true) return;
    }
}
