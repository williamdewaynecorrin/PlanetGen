using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    [SerializeField]
    private float thruststrength = 20;
    [SerializeField]
    private float rotspeed = 5;
    [SerializeField]
    private float rollspeed = 30;
    [SerializeField]
    private float rotsmoothspeed = 10;
    [SerializeField]
    private bool lockcursor;
    [SerializeField]
    private LayerMask terrainmask;

    private Rigidbody rb;
    private int numCollisionTouches;
    private Quaternion targetRot;
    private Quaternion smoothedRot;
    private Vector3 thrusterInput;
    private Camera cam;

    // -- controls
    private KeyCode forwardkey = KeyCode.W;
    private KeyCode backwardkey = KeyCode.S;
    private KeyCode leftkey = KeyCode.A;
    private KeyCode rightkey = KeyCode.D;

    void Awake()
    {
        cam = GetComponent<Camera>();

        InitRigidbody();
        targetRot = transform.rotation;
        smoothedRot = transform.rotation;

        if (lockcursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void InitRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.centerOfMass = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void HandleMovement()
    {
        // Thruster input
        int thrustInputX = GetInputAxis(leftkey, rightkey);
        int thrustInputZ = GetInputAxis(backwardkey, forwardkey);
        thrusterInput = new Vector3(thrustInputX, 0f, thrustInputZ);

        // Rotation input
        float yawInput = Input.GetAxisRaw("Mouse X") * rotspeed;
        float pitchInput = Input.GetAxisRaw("Mouse Y") * rotspeed;

        // Calculate rotation
        if (numCollisionTouches == 0)
        {
            Quaternion yaw = Quaternion.AngleAxis(yawInput, transform.up);
            Quaternion pitch = Quaternion.AngleAxis(-pitchInput, transform.right);

            targetRot = yaw * pitch * targetRot;
            smoothedRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotsmoothspeed);
        }
        else
        {
            targetRot = transform.rotation;
            smoothedRot = transform.rotation;
        }
    }

    void FixedUpdate()
    {
        //// Gravity
        //Vector3 gravity = GalaxyManager.CalculateAcceleration(rb.position);
        //rb.AddForce(gravity, ForceMode.Acceleration);

        // Thrusters
        Vector3 thrustDir = transform.TransformVector(thrusterInput);
        rb.MovePosition(this.transform.position + thrustDir * thruststrength);

        //rb.AddForce(thrustDir * thruststrength, ForceMode.Acceleration);

        if (numCollisionTouches == 0)
        {
            rb.MoveRotation(smoothedRot);
        }
    }

    int GetInputAxis(KeyCode negativeAxis, KeyCode positiveAxis)
    {
        int axis = 0;
        if (Input.GetKey(positiveAxis))
            axis++;
        if (Input.GetKey(negativeAxis))
            axis--;

        return axis;
    }

    void OnCollisionEnter(Collision other)
    {
        if (terrainmask == (terrainmask | (1 << other.gameObject.layer)))
        {
            numCollisionTouches++;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (terrainmask == (terrainmask | (1 << other.gameObject.layer)))
        {
            numCollisionTouches--;
        }
    }
}
