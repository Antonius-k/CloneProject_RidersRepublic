using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform canvas;

    public int camDis =3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���� �չ����� ī�޶� �չ������� ��������
        canvas.forward = Camera.main.transform.forward * camDis;

    }
}
