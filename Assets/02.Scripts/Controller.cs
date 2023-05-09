using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[System.Serializable]
public class BicycleGameObject
{
    public GameObject handles, FrontWheelVisual, RearWheel, Creank, lPedal, rPedal;
}

[System.Serializable]
public class PedalAdjustments
{
    public float crankRadius;
    public Vector3 lPedalOffset, rPedalOffset;
    public float pedalingSpeed;
}

[System.Serializable]
public class WheelFrictionSettings
{
    public PhysicMaterial fPhysicMaterial, rPhysicMaterial;
    public Vector2 fFriction, rFriction;
}

[System.Serializable]
public class AirTimeSettings
{
    public bool freestyle;
    public float airTimeRotationSensitivity;
    [Range(0.5f, 10)]
    public float heightThreshold;
    public float groundSnapSensitivity;
}

[System.Serializable]
public class MeshTrailStruct
{
    public GameObject Container;
    
    public MeshFilter BodyMeshFilter;
    public MeshFilter HeadMeshFilter;
    public MeshFilter ClothMeshFilter;
    
    public Mesh bodyMesh;
    public Mesh headMesh;
    public Mesh clothMesh;
}

public class Controller : MonoBehaviourPun, IPunObservable
{
    //photon userName
    public Text nameText;
    public Text playerNumText;


    public BicycleGameObject cycleGeometry;
    [Header("프리스타일")]
    public AirTimeSettings airTimeSettings;
    public WheelFrictionSettings wheelFrictionSettings;
    [Header("가속력")]
    public AnimationCurve accelerationCurve;
    [Header("핸들 각도에 따른 속력 값")]
    public AnimationCurve steerAngle;
    public float axisAngle;
    [Header("자전거 좌우 기울기 값")]
    public AnimationCurve leanCurve;
    public float torque, topSpeed;
    [Range(0.1f, 0.9f)]
    [Header("최대 속력 및 일반 속력 (대쉬 기능)")]
    public float relaxedSpeed;
    public float reversingSpeed;
    public Vector3 centerOfMassOffset;
    [HideInInspector]
    public Rigidbody rb;
    //----------------------------------------
    public float currentTopSpeed, pickUpSpeed;
    //------------ Input 값 -----------------
    public float customSteerAxis, customLeanAxis, customAccelerationAxis, rawCustomAccelerationAxis, bunnyHopInputState, bunnyHopAmount;
    public bool isReversing, isAirborne, stuntMode, isBunnyHopping;
    public float bunnyHopStrength = 25;
    bool isRaw, sprint;
    //---------크랭크 ------
    public float crankSpeed, crankCurrentQuat, crankLastQuat, restingCrank;
    Quaternion initialHandlesRotation;
    RaycastHit hit;
    public float turnturnLeanAmount;
    public bool groundConformity;
    float groundZ;

    //--------------Photon------------------
    //도착 위치
    Vector3 receivePos;
    //회전되야 하는 값
    Quaternion receiveRot;
    //보간 속력
    public float lerpSpeed = 100;
    [Header("Animation")]
    Animator anim;
    //---------------------CAM
    public GameObject cameraParents;

    //위치
    Vector3[] bikePos;
    private void Start()
    {
        isCreater = photonView.IsMine;
        rb = GetComponent<Rigidbody>();
        initialHandlesRotation = cycleGeometry.handles.transform.localRotation;
        currentTopSpeed = topSpeed;
        anim = GetComponentInChildren<Animator>();

        if (photonView.IsMine == true)
        {
            //camPos를 활성화 한다.
            cameraParents.gameObject.SetActive(true);
        }
      
        //각 접속자의 닉네임을 출력한다.
        nameText.text = photonView.Owner.NickName;
        playerNumText.text = photonView.ViewID.ToString();

        if (photonView.IsMine)//자신의 이름은 녹색, 다른 사람의 이름은 빨간색으로 출력한다.
        {
            nameText.color = Color.green;
        }else
        {
            nameText.color = Color.red;
        }
        
      
            //GameManager에게 나의 PhotonView를 주자
         //   GameManagerforProject.Instance.AddPlayer(photonView);
        
    }
  
    //int i = photonView.ControllerActorNr - 1;

    void Update()
    {
        //만약에 내것이 아니라면 함수를 나간다.
       /* if (photonView.IsMine == false) return;
        if (Cursor.visible == true) return;
        */
        if (photonView.IsMine) //(isCreater)
        {
            if (Cursor.visible == false)
            {
                //게임상태가 플레이 상태가 아니라면 컨트롤하지 않는다.
                /*if(GameManagerforProject.Instance.state != GameManagerforProject.GameState.Play)
                {
                    return;
                }*/
               

                //만약 플레이상태거나, waitingPlayer상태일때만 실행한다.
                if (GameManagerforProject.Instance.state == GameManagerforProject.GameState.Play || GameManagerforProject.Instance.state == GameManagerforProject.GameState.WaitingPlayer)
                {
                    ApplyCustomInput();

                    //GetKeyUp/Down requires an Update Cycle
                    //BunnyHopping
                    if (bunnyHopInputState == 1)
                    {
                        isBunnyHopping = true;
                        bunnyHopAmount += Time.deltaTime * 8f;
                    }
                    else if (bunnyHopInputState == -1 && !isAirborne)
                    {
                        rb.AddForce(transform.up * bunnyHopAmount * bunnyHopStrength, ForceMode.VelocityChange);
                        isAirborne = false;
                        isBunnyHopping = false;
                    }
                    else
                    {
                        bunnyHopAmount = 0;
                        isAirborne = false;
                        isBunnyHopping = false;
                    }


                    bunnyHopAmount = Mathf.Lerp(bunnyHopAmount, 0, Time.deltaTime * 8f);
                    BikeForce();
                    //만약에 움직인다면
                    if (customAccelerationAxis != 0 || customSteerAxis != 0)
                    {
                        //상태를 Move로
                        anim.SetTrigger("Move");
                    }
                    //그렇지 않다면
                    else
                    {
                        //상태를 Idle로
                        anim.SetTrigger("Idle");
                    }


                    
                   /* if (GameManagerforProject.Instance.state == GameManagerforProject.GameState.Ready)
                    {
                        List<PhotonView> players = GameManagerforProject.Instance.players;
                        GameObject[] startLine = GameManagerforProject.Instance.startLine;

                        int i = photonView.ControllerActorNr -1 ;
                        players[i].transform.position = startLine[i].transform.position;

                        print(i + "11111111111111");
                        //print("players[i]: " + players[i]);
                        //print("players[i].transform.position:" + players[i].transform.position);
                        //print("startLine[i].transform.position: " + startLine[i].transform.position);
                    }*/
                    
                }


                /*    if(Input.GetKeyDown(KeyCode.Alpha9))
                    {
                    if (!PhotonNetwork.IsMasterClient && !photonView.IsMine)
                    {

                        photonView.TransferOwnership(PhotonNetwork.MasterClient);
                    }

                    }
                */
            }
        }
        else if(!photonView.IsMine)
        {       
            cameraParents.gameObject.SetActive(false);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, lerpSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, receivePos, lerpSpeed * Time.deltaTime);
        }
    }

    bool statrtPos = true;

    bool isCreater;
    public float currentSpeed;
    public void BikeForce()
    {
        currentSpeed = rb.velocity.magnitude;
        if (stuntMode)
            rb.centerOfMass = GetComponent<BoxCollider>().center;
        else
            rb.centerOfMass = Vector3.zero + centerOfMassOffset;

        if (!sprint)
        {
            currentTopSpeed = Mathf.Lerp(currentTopSpeed, topSpeed * relaxedSpeed, Time.deltaTime);
        }
        else
            currentTopSpeed = Mathf.Lerp(currentTopSpeed, topSpeed, Time.deltaTime);

        if (currentSpeed < currentTopSpeed && rawCustomAccelerationAxis > 0 && !isAirborne && !isBunnyHopping)
        {
            if (hit.distance < 3.0f)
            {
                rb.AddForce(transform.forward * accelerationCurve.Evaluate(customAccelerationAxis));
                Vector3 handleviarbRotation = cycleGeometry.handles.transform.forward;
                handleviarbRotation.y = transform.forward.y;
                transform.forward = Vector3.Lerp(transform.forward, handleviarbRotation, 20.0f * Time.deltaTime);

            }
        }
        if (currentSpeed < reversingSpeed && rawCustomAccelerationAxis < 0 && !isAirborne && !isBunnyHopping)
            rb.AddForce(-transform.forward * accelerationCurve.Evaluate(customAccelerationAxis) * 0.5f);

     
        cycleGeometry.handles.transform.localRotation = Quaternion.Euler(0, customSteerAxis * steerAngle.Evaluate(currentSpeed) * 5, 0) * initialHandlesRotation;

        //앞바퀴 돌아가는 부분 
        cycleGeometry.FrontWheelVisual.transform.localRotation = Quaternion.Euler(0, customSteerAxis * steerAngle.Evaluate(currentSpeed) * 5, 0) * initialHandlesRotation;

     
        // 높이 계산
        if (Physics.Raycast(transform.position + new Vector3(0, 1f, 0), Vector3.down, out hit))
        {
            if (hit.distance > 1.5f)
            {
                isAirborne = true;
                restingCrank = 100;
            }
            else if (isBunnyHopping)
            {
                restingCrank = 100;
            }
            else
            {
                isAirborne = false;
                restingCrank = 10;
            }
            if (isAirborne)//(hit.distance > airTimeSettings.heightThreshold && airTimeSettings.freestyle)
            {
                stuntMode = true;

                rb.AddTorque(Vector3.up * customSteerAxis * airTimeSettings.airTimeRotationSensitivity*Time.deltaTime, ForceMode.Impulse);
                /* rb.AddTorque(transform.right * rawCustomAccelerationAxis * -3 * airTimeSettings.airTimeRotationSensitivity, ForceMode.Impulse);*/
            }
            else
                stuntMode = false;
        }
    }

    float GroundConformity(bool toggle)
    {
        if (toggle)
        {
            groundZ = transform.rotation.eulerAngles.z;
        }
        return groundZ;
    }

    void ApplyCustomInput()
    {
        CustomInput("Horizontal", ref customSteerAxis, 5, 5, false);
        CustomInput("Vertical", ref customAccelerationAxis, 1, 1, false);
        CustomInput("Horizontal", ref customLeanAxis, 1, 1, false);
        CustomInput("Vertical", ref rawCustomAccelerationAxis, 1, 1, true);

        // 대쉬
        sprint = Input.GetKey(KeyCode.LeftShift);

        // 스턴트 모드
        if (Input.GetKey(KeyCode.Space))
            bunnyHopInputState = 1;
        else if (Input.GetKeyUp(KeyCode.Space))
            bunnyHopInputState = -1;
        else
            bunnyHopInputState = 0;
    }

    float CustomInput(string name, ref float axis, float sensitivity, float gravity, bool isRaw)
    {
        var r = Input.GetAxisRaw(name);
        var s = sensitivity;
        var g = gravity;
        var t = Time.unscaledDeltaTime;
        if (isRaw)
        {
            axis = r;
        }
        else
        {
            if (r != 0)
            {
                axis = Mathf.Clamp(axis + r * s * t, -1f, 1f);
            }
            else
            {
                axis = Mathf.Clamp01(Mathf.Abs(axis) - g * t) * Mathf.Sign(axis);
            }
        }
        return axis;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //데이터 보내기
        if (stream.IsWriting) // isMine == true
        {
            //position, rotation
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.position);
        }
        //데이터 받기
        else if (stream.IsReading) // ismMine == false
        {
            receiveRot = (Quaternion)stream.ReceiveNext();
            receivePos = (Vector3)stream.ReceiveNext();
        }
    }

}