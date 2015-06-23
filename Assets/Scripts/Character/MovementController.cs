using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public GameObject groundOject;

    private Vector3 moveDirection = Vector3.zero;
    private bool GravityActivated = false;

    void Start()
    {
        Vector3 pos = transform.position;
        /*Instantiate(groundOject, new Vector3(pos.x, pos.y-1, pos.z), Quaternion.identity);*/
    }

    private void activateGravity()
    {
        GravityActivated = true;
    }
	// Update is called once per frame
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

            activateGravity();
        }
        else if (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") != 0)
        {
            activateGravity();
        }
        if(GravityActivated)
            moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

}
