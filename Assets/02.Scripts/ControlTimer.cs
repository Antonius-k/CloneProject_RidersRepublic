using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//스타트라인,피니쉬라인 카운트 다운
public class ControlTimer : MonoBehaviourPun
{
    Timer playerTimer;


    //부딪혀서 둘중하나 파괴는 OnCollisionEnter
    //부딪혀서 점수 플러스,마이너스 되는경우 or 오브젝트 밀어야되는 경우 OnControllerColliderHit
    private void Start()
    {
        playerTimer = GetComponent<Timer>();

    }
    private void OnTriggerEnter(Collider other)
    {

        //스타트 라인 도착
        if (other.gameObject.name == "StartRace")
        {
                SetTimer(1);
  
            
        }
        //피니쉬 라인 도착
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
