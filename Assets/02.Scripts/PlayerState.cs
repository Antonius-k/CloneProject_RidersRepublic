using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerState : MonoBehaviourPun
{
    //플레이어 상태 Enmu
    public enum State
    {
        IDLE,
        MOVE,
        DIE
    }

    //현재 상태
    public State currState;
    //Animator
    public Animator anim;

    //죽었을 때 off해줘야 하는 GameObject들
    public GameObject[] disableGo;
    //죽었을 때 off해줘야 하는 Component들
    public MonoBehaviour[] disableCom;

    //상태 변경 함수
    public void ChangeState(State s)
    {        
        //현재 상태가 s와 같다면 함수를 나가라
        if (currState == s) return;

        //현재 상태를 s상태로!
        currState = s;
        //상태에 따라서 animation 처리
        switch(s)
        {
            case State.IDLE:
                photonView.RPC("RpcSetTrigger", RpcTarget.All, "Idle");
                break;
            case State.MOVE:
                photonView.RPC("RpcSetTrigger", RpcTarget.All, "Move");
                break;
            case State.DIE:
                //모델, ui off, PlayerFire 컴포넌트 off
                //1. GameObject Off
                for (int i = 0; i < disableGo.Length; i++)
                    disableGo[i].SetActive(false);
                //2. Component Off
                for (int i = 0; i < disableCom.Length; i++)
                    disableCom[i].enabled = false;
                break;
        }
    }
    
    [PunRPC]
    void RpcSetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }
}
