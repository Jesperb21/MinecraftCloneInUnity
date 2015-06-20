using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public float rotSpeed = 80f;
    private float yRot = 0f;
    private float xRot = 0f;
    private float maxAngle = 90;
    private float minAngle = -90;

    void Start()
    {
        //maxAngle = -maxAngle;
        //minAngle = -minAngle;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yRot = -Input.GetAxis ("Mouse Y") *rotSpeed/8 /10 ;
        yRot = Mathf.Clamp(yRot, -10, 10);//max rotation speed
        xRot += Input.GetAxis("Mouse X") * rotSpeed* Time.deltaTime;
        xRot = xRot % 360;


        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<Camera>())
            {
                float newAngle = child.transform.eulerAngles.x + yRot;
                if(newAngle >90)
                    newAngle -= 360;
                    



                if (newAngle >= minAngle && newAngle <= maxAngle)
                {
                    child.transform.RotateAround(transform.position, transform.right, yRot);
                }
                else
                {
                    
                   Debug.Log("new"+newAngle+"min:"+minAngle+"max:"+maxAngle);
                }
            }
        }
        transform.localEulerAngles = new Vector3(0, xRot, 0);
    }
}
