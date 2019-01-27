using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    public bool isControlable;
    private Vector3 screenPoint;
    private Vector3 offset;
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 50.0f;
    public float ySpeed = 50.0f;

    public float yMinLimit = -80f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 200f;

    public float smoothTime = 2f;

    public float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;

    float velocityX = 0.0f;
    float velocityY = 0.0f;
    float moveDirection = -1;

    public void SetControllable(bool value)
    {
        isControlable = value;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = (rotationYAxis == 0) ? angles.y : rotationYAxis;
        rotationXAxis = angles.x;
    }

    void LateUpdate()
    {
        if (target)
        {

            if (isControlable)
            {
                velocityX += xSpeed * Input.GetAxis("CamX") * 0.02f;
                velocityY += ySpeed * Input.GetAxis("CamY") * 0.02f;
            }

            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;

            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;

            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

            screenPoint = cam.WorldToScreenPoint(target.transform.position);
            offset = target.transform.position - cam.ScreenToWorldPoint(new Vector3(moveDirection * Input.mousePosition.x, moveDirection * Input.mousePosition.y, screenPoint.z));
        }

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
   
}
