using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//��ŸƮ����,�ǴϽ����� ī��Ʈ �ٿ�
public class ControlTimer : MonoBehaviourPun
{
    Timer playerTimer;


    //�ε����� �����ϳ� �ı��� OnCollisionEnter
    //�ε����� ���� �÷���,���̳ʽ� �Ǵ°�� or ������Ʈ �о�ߵǴ� ��� OnControllerColliderHit
    private void Start()
    {
        playerTimer = GetComponent<Timer>();

    }
    private void OnTriggerEnter(Collider other)
    {

        //��ŸƮ ���� ����
        if (other.gameObject.name == "StartRace")
        {
                SetTimer(1);
  
            
        }
        //�ǴϽ� ���� ����
        if (other.gameObject.name == "FinishRace")
        {
            SetTimer(0);


        }
    }

    void SetTimer(int t)
    {
        playerTimer = GetComponent<Timer>();
        playerTimer.startTimer = t;
    }

}
