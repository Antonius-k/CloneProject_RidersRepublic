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
    [Header("������Ÿ��")]
    public AirTimeSettings airTimeSettings;
    public WheelFrictionSettings wheelFrictionSettings;
    [Header("���ӷ�")]
    public AnimationCurve accelerationCurve;
    [Header("�ڵ� ������ ���� �ӷ� ��")]
    public AnimationCurve steerAngle;
    public float axisAngle;
    [Header("������ �¿� ���� ��")]
    public AnimationCurve leanCurve;
    public float torque, topSpeed;
    [Range(0.1f, 0.9f)]
    [Header("�ִ� �ӷ� �� �Ϲ� �ӷ� (�뽬 ���)")]
    public float relaxedSpeed;
    public float reversingSpeed;
    public Vector3 centerOfMassOffset;
    [HideInInspector]
    public Rigidbody rb;
    //----------------------------------------
    public float currentTopSpeed, pickUpSpeed;
    //------------ Input �� -----------------
    public float customSteerAxis, customLeanAxis, customAccelerationAxis, rawCustomAccelerationAxis, bunnyHopInputState, bunnyHopAmount;
    public bool isReversing, isAirborne, stuntMode, isBunnyHopping;
    public float bunnyHopStrength = 25;
    bool isRaw, sprint;
    //---------ũ��ũ ------
    public float crankSpeed, crankCurrentQuat, crankLastQuat, restingCrank;
    Quaternion initialHandlesRotation;
    RaycastHit hit;
    public float turnturnLeanAmount;
    public bool groundConformity;
    float groundZ;

    //--------------Photon------------------
    //���� ��ġ
    Vector3 receivePos;
    //ȸ���Ǿ� �ϴ� ��
    Quaternion receiveRot;
    //���� �ӷ�
    public float lerpSpeed = 100;
    [Header("Animation")]
    Animator anim;
    //---------------------CAM
    public GameObject cameraParents;

    //��ġ
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
            //camPos�� Ȱ��ȭ �Ѵ�.
            cameraParents.gameObject.SetActive(true);
        }
      
        //�� �������� �г����� ����Ѵ�.
        nameText.text = photonView.Owner.NickName;
        playerNumText.text = photonView.ViewID.ToString();

        if (photonView.IsMine)//�ڽ��� �̸��� ���, �ٸ� ����� �̸��� ���������� ����Ѵ�.
        {
            nameText.color = Color.green;
        }else
        {
            nameText.color = Color.red;
        }
        
      
            //GameManager���� ���� PhotonView�� ����
         //   GameManagerforProject.Instance.AddPlayer(photonView);
        
    }
  
    //int i = photonView.ControllerActorNr - 1;

    void Update()
    {
        //���࿡ ������ �ƴ϶�� �Լ��� ������.
       /* if (photonView.IsMine == false) return;
        if (Cursor.visible == true) return;
        */
        if (photonView.IsMine) //(isCreater)
        {
            if (Cursor.visible == false)
            {
                //���ӻ��°� �÷��� ���°� �ƴ϶�� ��Ʈ������ �ʴ´�.
                /*if(GameManagerforProject.Instance.state != GameManagerforProject.GameState.Play)
                {
                    return;
                }*/
               

                //���� �÷��̻��°ų�, waitingPlayer�����϶��� �����Ѵ�.
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
                    //���࿡ �����δٸ�
                    if (customAccelerationAxis != 0 || customSteerAxis != 0)
                    {
                        //���¸� Move��
                        anim.SetTrigger("Move");
                    }
                    //�׷��� �ʴٸ�
                    else
                    {
                        //���¸� Idle��
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

        //�չ��� ���ư��� �κ� 
        cycleGeometry.FrontWheelVisual.transform.localRotation = Quaternion.Euler(0, customSteerAxis * steerAngle.Evaluate(currentSpeed) * 5, 0) * initialHandlesRotation;

     
        // ���� ���
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

        // �뽬
        sprint = Input.GetKey(KeyCode.LeftShift);

        // ����Ʈ ���
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
        //������ ������
        if (stream.IsWriting) // isMine == true
        {
            //position, rotation
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.position);
        }
        //������ �ޱ�
        else if (stream.IsReading) // ismMine == false
        {
            receiveRot = (Quaternion)stream.ReceiveNext();
            receivePos = (Vector3)stream.ReceiveNext();
        }
    }

}