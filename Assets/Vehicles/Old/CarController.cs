using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
    [Header("Vehicle Properties")]
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public Transform centerOfMass;

    private float currentMotorTorque;
    private float currentSteerAngle;
    private Rigidbody carRigidbody;

    [Header("List of Wheels (Hover for tooltips)")]
    [Tooltip(
        "Wheel objects should have the following structure:\n" +
        ">Wheel Container\n" +
        "    >Visuals\n" +
        "    >Collider\n" +
        "\"Wheel Container\" should have a rigidbody attached.\n" +
        "\"Visuals\" should have all visual representations of the wheel attached.\n" +
        "\"Collider\" should have a Wheel Collider component attached.\n" +
        "The names of each of these objects can be changed, but \"Visuals\" MUST be Child 0 of \"Wheel Container\"."
    )]
    public List<WheelObject> wheelObjects;

    [System.Serializable]
    public class WheelObject {
        [Tooltip("Child 0 of this object must be an object containing visuals - another child must contain a WheelCollider component.")]
        public GameObject wheelContainer;
        [Tooltip("Determines if torque is applied to the wheel - is it affected by the motor?")]
        public bool isMotorized;
        [Tooltip("Determines if steer angle is applied to the wheel - does this wheel steer?")]
        public bool canSteer;
        
        [HideInInspector] public WheelCollider wheelCollider;
        [HideInInspector] public GameObject wheelVisuals;
    }

    public void Start() {
        foreach(WheelObject wheel in wheelObjects) {
            wheel.wheelVisuals = wheel.wheelContainer.transform.GetChild(0).gameObject;
            wheel.wheelCollider = wheel.wheelContainer.GetComponentInChildren<WheelCollider>();
        }
        carRigidbody = wheelObjects[0].wheelCollider.attachedRigidbody;
    }

    public void Update() {
        updatePlayerInput();
    }
        
    public void FixedUpdate() {
        updateWheelProperties();
    }

    public void updatePlayerInput() {
        currentMotorTorque = maxMotorTorque * Input.GetAxis("Vertical");
        currentSteerAngle = maxSteeringAngle * Input.GetAxis("Horizontal");
    }

    public void updateWheelProperties() {
        foreach (WheelObject wheel in wheelObjects) {
            // if wheel steers
            if (wheel.canSteer) {
                // set the steer angle
                wheel.wheelCollider.steerAngle = currentSteerAngle;
            }

            // if wheel is motorized
            if (wheel.isMotorized) {
                // set the motor torque
                wheel.wheelCollider.motorTorque = currentMotorTorque;
            }
            
            // get wheel collider transforms
            Vector3 wheelColliderPosition;
            Quaternion wheelColliderRotation;
            Vector3 wheelColliderRotationEul;
            wheel.wheelCollider.GetWorldPose(out wheelColliderPosition, out wheelColliderRotation);
            wheelColliderRotationEul = wheelColliderRotation.eulerAngles;
            //TODO: set the rotation of wheel mesh to the rotation of wheel collider
            
        }
    }
}