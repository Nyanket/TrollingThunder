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

    [SerializeField] private float thrusterFuelBurnSpeed = 1f;

    [SerializeField] private float thrusterFuelRecovery = 0.3f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    private float thrusterFuelAmount = 1f;

    private ConfigurableJoint joint;

    [SerializeField] private LayerMask environtmentMask;

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

        if (PauseMenu.isOn)
            return;

        RaycastHit _hit;
        if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f,environtmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }else
            joint.targetPosition = new Vector3(0f, 0f, 0f);

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

        if (Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01f)
            {
                _thrustherForce = Vector3.up * thrusterforce;
                SetJointSettings(0f);
            }

        } else
        {
            thrusterFuelAmount += thrusterFuelRecovery * Time.deltaTime;
            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);
        

        motor.ApplyThruster(_thrustherForce);

	}

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }
}
