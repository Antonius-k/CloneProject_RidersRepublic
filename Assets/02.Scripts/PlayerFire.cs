using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerFire : MonoBehaviourPun
{
    //파편효과공장
    public GameObject bulletImapctFactory;

    //총알공장
    public GameObject bulletFactory;

    //총구
    public Transform firePos;

    //총소리
    public AudioSource bulletSound;

    void Start()
    {
        bulletSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        //내것이 아니라면 함수를 끝낸다.
        if (photonView.IsMine == false) return;

        //1. 왼쪽 컨트롤키 누르면 
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            FireRay();
        }

        // 1. 왼쪽 알트키를 누르면
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            //// 2. 총알공장 총알 만든다.
            //GameObject bullet = Instantiate(bulletFactory);
            //// 3. 총알을 총구에 위치 시킨다.
            //bullet.transform.position = firePos.position;
            //// 4. 총알의 앞방향을 총구방향으로 한다.
            //bullet.transform.forward = firePos.forward;
            PhotonNetwork.Instantiate("Bullet", firePos.position, firePos.rotation);
        }
    }

    void FireRay()
    {
        //2. 카메라 중심, 카메라 앞방향으로 나가는 Ray를 생성
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        //3. 생성된 Ray를 발사해서 어딘가에 부딪혔다면
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            //맞은 놈에게 PlayerMove 컴포넌트를 가져온다.
            PlayerMove pm = hit.transform.GetComponent<PlayerMove>();
            //만약에 가져온 컴포넌트가 null이 아니라면
            if(pm != null)
            {
                //OnDamaged 실행
                pm.OnDamaged();
            }

            photonView.RPC("RpcShowBulletImpact", RpcTarget.All, hit.point, hit.normal);
        }
        else
        {
            //총소리 -> 동기화
            photonView.RPC("RpcBulletSound", RpcTarget.All);            
        }

        //총쏘는 애니메이션
        photonView.RPC("RpcSetTrigger", RpcTarget.All, "Fire");
    }

    [PunRPC]
    void RpcBulletSound()
    {
        bulletSound.PlayOneShot(bulletSound.clip);
    }


    [PunRPC]
    void RpcShowBulletImpact(Vector3 point, Vector3 normal)
    {
        //4. bulletImpact 효과를 만든다.
        GameObject bulletImapct = Instantiate(bulletImapctFactory);
        //5. 만든효과를 부딪힌 위치에 놓는다.
        bulletImapct.transform.position = point;
        //6. 만든효과의 앞방향을 normal방향으로 한다.
        bulletImapct.transform.forward = normal;
        //7. 2초뒤에 파괴한다.
        Destroy(bulletImapct, 2);


    }
    
}
