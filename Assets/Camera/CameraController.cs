using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject targetObject;

    [Header("Camera Properties")]
    public float minZoom = 4f;
    public float maxZoom = 10f;
    public float zoomSpeed = 0.75f;
    public float rotationSpeed = 2f;
    
    [Header("Current State")]
    public float currentZoom;
    public Vector2 currentRotation = Vector2.zero;
    public bool trackMouse = true;

    private Vector3 betweenTargetAndParent = Vector3.zero;
    private RaycastHit raycastHit;

    void Start() {
        if (targetObject == null) {
            targetObject = gameObject;
        }
        currentZoom = minZoom;
        // move to the position directly behind the target
        transform.position = targetObject.transform.position + -targetObject.transform.forward * currentZoom;

        // re-calculate the location between the target and the parent to be focused upon
        betweenTargetAndParent = (targetObject.transform.parent.position + (targetObject.transform.position - targetObject.transform.parent.position) / 2);
        // look at the space between the target object and the target's parent object
        transform.LookAt(betweenTargetAndParent);
    }

    void Update() {
        getPlayerInput();
    }

    void LateUpdate() {
        // re-calculate the location between the target and the parent to be focused upon
        betweenTargetAndParent = (targetObject.transform.parent.position + (targetObject.transform.position - targetObject.transform.parent.position) / 2);
        
        updatePosition();
    }

    void getPlayerInput() {
        if (trackMouse) {
            // updates horizontal look rotation based upon mouse rotation
            currentRotation.x += rotationSpeed * Input.GetAxis("Mouse X");
            if (currentRotation.x > 360f) {
                currentRotation.x = 0f;
            }
            else if (currentRotation.x < -360f) {
                currentRotation.x = 0f;
            }
            currentRotation.y += rotationSpeed * -Input.GetAxis("Mouse Y");
            if (currentRotation.y > 360f) {
                currentRotation.y = 0f;
            }
            else if (currentRotation.y < -360f) {
                currentRotation.y = 0f;
            }
        }

        if (Input.GetAxis("Scrollwheel") > 0f) {
            // zoom in
            currentZoom -= zoomSpeed;
            // if closer than minimum zoom, zoom out to the minimum zoom
            if (currentZoom < minZoom) {
                currentZoom = minZoom;
            }
        }
        else if (Input.GetAxis("Scrollwheel") < 0f) {
            // zoom out
            currentZoom += zoomSpeed;
            // if farther than maximum zoom, zoom in to the maximum zoom
            if (currentZoom > maxZoom) {
                currentZoom = maxZoom;
            }
        }
    }

    void updatePosition() {
        // move to the position behind the target with a distance of the currentZoom
        transform.position = targetObject.transform.position + -Vector3.forward * currentZoom;

        // reset rotation
        transform.rotation = Quaternion.identity;
        // rotate horizontally around the target to the current horizontal rotation
        transform.RotateAround(targetObject.transform.position, transform.up, currentRotation.x);
        // rotate vertically around the target to the current vertical rotation
        transform.RotateAround(targetObject.transform.position, transform.right, currentRotation.y);

        // raycast to check for collisions between the target and the camera
        if (Physics.Raycast(betweenTargetAndParent, (transform.position - betweenTargetAndParent).normalized, out raycastHit, currentZoom)) {
            transform.position = raycastHit.point;
        }
        
        // look at the position between the target object and the target's parent object
        transform.LookAt(betweenTargetAndParent);
    }
}