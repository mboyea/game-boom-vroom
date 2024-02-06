using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
public class VehicleController : MonoBehaviour {
    [System.Serializable] private struct Axle {
        [Header("Axle Wheel Properties")]
        public bool isMotorized;
        public bool canSteer;
        public bool isBrake;
        public bool isHandbrake;

        [Header("Axle Wheel Colliders")]
        public WheelCollider wheelColliderLeft;
        public WheelCollider wheelColliderRight;

        [Header("Axle Wheel Models")]
        public GameObject wheelModelLeft;
        public GameObject wheelModelRight;
    }

    [Header("Properties")]

    [Tooltip("Center of mass of the car")]
    [SerializeField] private Transform centerOfMass;

    [Tooltip("Maximum torque the motor can apply to motorized wheels")]
    public float maxMotorTorque = 1200f;

    [Tooltip("Maximum steer angle of steering wheels")]
    public float maxSteerAngle = 30f;

    [Tooltip("Maximum torque the brakes can apply to braking wheels")]
    public float maxBrakeTorque = 600f;

    [Tooltip("Information about each of the wheels")]
    [SerializeField] private List<Axle> axles;


    [Header("Current State")]

    [SerializeField] private float currentSteerAngle = 0f;

    [SerializeField] private float currentMotorTorque = 0f;

    [SerializeField] private float currentBrakeTorque = 0f;

    [SerializeField] private float currentHandbrakeTorque = 0f;

    /* Private References */
    private Rigidbody vehicleRigidbody;

    public void Start() {
        vehicleRigidbody = axles[0].wheelColliderLeft.attachedRigidbody;
        vehicleRigidbody.centerOfMass = centerOfMass.position;
    }

    public void Update() {
        UpdateInput();
    }
        
    public void FixedUpdate() {
        UpdateWheels();
    }

    private void UpdateInput() {
        currentSteerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
        currentMotorTorque = maxMotorTorque * Input.GetAxis("Vertical");
        currentHandbrakeTorque = maxBrakeTorque * Input.GetAxis("Handbrake");
        
        if (currentMotorTorque < 0) {
            if (isVehicleMovingForward()) {
                currentMotorTorque = 0;
                currentBrakeTorque = maxBrakeTorque * -Input.GetAxis("Vertical");
            }
            else {
                currentBrakeTorque = 0;
            }
        }
        else {
            currentBrakeTorque = 0;
        }
    }

    private void UpdateWheels() {
        foreach (Axle axle in axles) {
            /* UPDATE WHEEL COLLIDERS */
            if (axle.canSteer) {
                axle.wheelColliderLeft.steerAngle = currentSteerAngle;
                axle.wheelColliderRight.steerAngle = currentSteerAngle;
            }
            if (axle.isBrake) {
                axle.wheelColliderLeft.brakeTorque = currentBrakeTorque;
                axle.wheelColliderRight.brakeTorque = currentBrakeTorque;
            }
            if (axle.isHandbrake) {
                axle.wheelColliderLeft.brakeTorque = currentHandbrakeTorque;
                axle.wheelColliderRight.brakeTorque = currentHandbrakeTorque;
            }
            if (axle.isMotorized) {
                axle.wheelColliderLeft.motorTorque = currentMotorTorque;
                axle.wheelColliderRight.motorTorque = currentMotorTorque;
            }

            /* UPDATE WHEEL MESHES */
            // Update left wheel mesh
            Vector3 wheelLeftPos = Vector3.zero;
            Quaternion wheelLeftRot = Quaternion.identity;
            axle.wheelColliderLeft.GetWorldPose(out wheelLeftPos, out wheelLeftRot);
            axle.wheelModelLeft.transform.position = wheelLeftPos;
            axle.wheelModelLeft.transform.rotation = wheelLeftRot;
            // Update right wheel mesh
            Vector3 wheelRightPos = Vector3.zero;
            Quaternion wheelRightRot = Quaternion.identity;
            axle.wheelColliderRight.GetWorldPose(out wheelRightPos, out wheelRightRot);
            axle.wheelModelRight.transform.position = wheelRightPos;
            axle.wheelModelRight.transform.rotation = wheelRightRot;
        }
    }

    public bool isVehicleMovingForward() {
        return Vector3.Dot(vehicleRigidbody.velocity, transform.forward) > 0.1;
    }

    public bool isVehicleGrounded() {
        foreach(Axle axle in axles) {
            
        }
        return false;
    }
}