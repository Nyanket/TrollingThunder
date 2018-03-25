using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour {

    [SerializeField] private float speed = 5f;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float thrusterforce = 1000f;
    private PlayerMotor motor;

    private ConfigurableJoint joint;

    [Header("Spring Settings")]
    [SerializeField] private float jointSpring = 20f;
    [SerializeField] private float jointMaxForce = 40f;

    [SerializeField] private Animator anim;

	void Start () {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        anim = GetComponent<Animator>();

        SetJointSettings(jointSpring);

	}
	
	void Update () {
        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;
        motor.Move(velocity);

        anim.SetFloat("FowardVelocity", zMove);

        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Mathf.Min(50, Mathf.Max(-50, Input.GetAxis("Mouse Y"))); ;

        Vector3 rotation = new Vector3(0, yRot, 0) * sensitivity;
        float camRotationX = xRot * sensitivity;

        motor.Rotation(rotation);
        motor.camRotation(camRotationX);

        Vector3 _thrustherForce = Vector3.zero;

        if (Input.GetButton("Jump"))
        {
            _thrustherForce = Vector3.up * thrusterforce;
            SetJointSettings(0f);
        } else
        {
            SetJointSettings(jointSpring);
        }

        

        motor.ApplyThruster(_thrustherForce);

	}

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
