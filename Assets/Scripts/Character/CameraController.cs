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
        
        //lock mouse and make it invisible
           Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.GetComponent<Camera>())
                {
                    RotateCamera(child);
                    ZoomCamera(child);
                }
            } 
        
        
    }

    void ZoomCamera(GameObject cam)
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))//hold shift to zoom
        {
            float scrollFactor = 5.0f;
            Vector3 pos = cam.transform.position;
            float mouseWheelAction = Input.GetAxis("Mouse ScrollWheel");
            Vector3 headPos = transform.position;
            headPos.y+= 0.5f; // zoom towards the top of the player

            //dont move closer if the distance between mouse and player gets below 0, this is done to allow the player to zoom out again
            if (Vector3.Distance(pos, headPos) > mouseWheelAction * scrollFactor)
                cam.transform.position = Vector3.MoveTowards(pos, headPos, scrollFactor * mouseWheelAction);
        }
    }

    void RotateCamera(GameObject cam)
    {
        yRot = -Input.GetAxis("Mouse Y") * rotSpeed / 8 / 10;
        yRot = Mathf.Clamp(yRot, -10, 10);//max rotation speed
        xRot += Input.GetAxis("Mouse X") * rotSpeed * Time.deltaTime;
        xRot = xRot % 360;


        float newAngle = cam.transform.eulerAngles.x + yRot;
        if (newAngle > 90)
            newAngle -= 360;




        if (newAngle >= minAngle && newAngle <= maxAngle)
        {
            cam.transform.RotateAround(transform.position, transform.right, yRot);
        }


        transform.localEulerAngles = new Vector3(0, xRot, 0);
    }
}
