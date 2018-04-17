using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour {

    [SerializeField] private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float camRotateX = 0f;
    [SerializeField]
    private float currCamRotateX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField] private float cameraRotationLimit = 80f;

    [SerializeField] private Transform head;

    private Rigidbody rb;

	void Start () {
        rb = GetComponent<Rigidbody>();
	}

	public void Move (Vector3 _velocity) {
        velocity = _velocity;
	}

    public void Rotation(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void camRotation(float _camRotationX)
    {
        camRotateX = _camRotationX;
    }

    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

    }

    private void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currCamRotateX -= camRotateX;
            currCamRotateX = Mathf.Clamp(currCamRotateX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currCamRotateX, 0, 0);
            head.localEulerAngles = new Vector3(-cam.transform.localEulerAngles.x, 0, 0);

        }
    }

   
    
}
