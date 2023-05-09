using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    //프로토 이후로 수정
    /*[Header("Position")]
    public GameObject currentPostion;
    public GameObject checkPointHolder;
    public GameObject[] Bike;
    public Transform[] checkpointPostion;
    public GameObject[] checkPointforeachBike;
    int totalBike;
    int totalCheckPoint;



    public TextMeshProUGUI PositionText;
    private void Start()
    {
        totalBike = Bike.Length;
        totalCheckPoint = checkPointHolder.transform.childCount;
        SetCheckPoints();
        SetBikePosition();
    }
  public  void SetCheckPoints()
    {
        checkpointPostion = new Transform[totalCheckPoint];
        for (int i = 0; i < totalCheckPoint; i++)
        {
            checkpointPostion[i] = checkPointHolder.transform.GetChild(i).transform;
        }
        checkPointforeachBike = new GameObject[totalBike];

        for (int i = 0; i < totalBike - 1; i++)
        {
            checkPointforeachBike[i] = Instantiate(currentPostion, checkpointPostion[0].position, checkpointPostion[0].rotation);
            checkPointforeachBike[i].name = "i에 들어간 바이크 이름" + i;
            checkPointforeachBike[i].layer = 10 + i;
        }

    }
   public void SetBikePosition()
    {
        for (int i = 0; i < totalBike; i++)
        {
            Bike[i].GetComponent<CheckpointSingle>().bikePosition = i + 1;
            Bike[i].GetComponent<CheckpointSingle>().PlayerNumbers = i;
        }
        PositionText.text = "POS" + Bike[0].GetComponent<CheckpointSingle>().bikePosition + "/" + totalBike;
    }
    public void UpdatedBikeCurrentPosition(int bikenumber, int currentpositionnumber)
    {
        print(currentpositionnumber);
        checkPointforeachBike[bikenumber].transform.position = checkpointPostion[currentpositionnumber].transform.position;
        checkPointforeachBike[bikenumber].transform.rotation = checkpointPostion[currentpositionnumber].transform.rotation;
        CompareBikePosition(bikenumber);
    }
  public  void CompareBikePosition(int bikenumber)
    {
        if (Bike[bikenumber].GetComponent<CheckpointSingle>().bikePosition > 1)
        {
            GameObject currentBike = Bike[bikenumber];
            int currentBikePos = currentBike.GetComponent<CheckpointSingle>().bikePosition;
            int currentCrossedBike = currentBike.GetComponent<CheckpointSingle>().crossedBike;

            GameObject BikeinFront = null;
            int bikeinFrontPos = 0;
            int bikeinFrontcrossedPos = 0;
            for (int i = 0; i < totalBike; i++)
            {
                if (Bike[i].GetComponent<CheckpointSingle>().bikePosition == currentBikePos - 1)
                {
                    BikeinFront = Bike[i];
                    bikeinFrontcrossedPos = BikeinFront.GetComponent<CheckpointSingle>().crossedBike;
                    bikeinFrontPos = BikeinFront.GetComponent<CheckpointSingle>().bikePosition;
                    break;
                }
            }
            if (currentCrossedBike > bikeinFrontcrossedPos)
            {
                currentBike.GetComponent<CheckpointSingle>().bikePosition = currentBikePos - 1;
                BikeinFront.GetComponent<CheckpointSingle>().bikePosition = bikeinFrontPos + 1;

                Debug.Log("자전거_수" + bikenumber + "/" + BikeinFront.GetComponent<CheckpointSingle>().PlayerNumbers);
            }
        }
    }*/
}
