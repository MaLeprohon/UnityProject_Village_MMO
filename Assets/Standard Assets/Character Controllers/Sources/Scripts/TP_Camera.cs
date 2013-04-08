using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TP_Camera : MonoBehaviour
{
    public static TP_Camera Instance;

    public Transform TargetLookAt;

    public float Distance = 5f;
    public float DistanceMin = 3f;
    public float DistanceMax = 10f;
    public float DistanceSmooth = 0.05f;
    public float DistanceResumeSmooth = 1f;
    public float X_MouseSensitivity = 5f;
    public float Y_MouseSensitivity = 5f;
    public float MouseWheelSensitivity = 5f;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    public float Y_MinLimit = -40f;
    public float Y_MaxLimit = 80f;
    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float velX = 0f;
    private float velY = 0f;
    private float velZ = 0f;
    private float velDistance = 0f;
    private float startDistance = 0f;
    private float desiredDistance = 0f;
    private Vector3 position = Vector3.zero;
    private Vector3 desiredPosition = Vector3.zero;
    private float distanceSmooth = 0f;
    private float preOccludedDistance = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startDistance = Distance;
        Reset();
    }

    void LateUpdate()
    {
        if (TargetLookAt == null)
        {
            return;
        }

        HandlePlayerInput();

        var count = 0;
        do
        {
            CalculateDesiredPosition();
            count++;
        } while (CheckIfOccluded(count));
        
        UpdatePosition();
    }

    void HandlePlayerInput()
    {
        var deadZone = 0.01f;

        if (Input.GetMouseButton(1))
        {
            // The RMB is down, get mouse axis input
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
        }

        // limit mouseY 
        mouseY = Helper.ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel")*MouseWheelSensitivity,
                                          DistanceMin, DistanceMax);

            preOccludedDistance = desiredDistance;
            distanceSmooth = DistanceSmooth;
        }
    }

    void CalculateDesiredPosition()
    {
        ResetDesiredDistance();

        // Evaluate distance
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velDistance, distanceSmooth);

        // Calculate desired position
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }

    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);

        return TargetLookAt.position + rotation*direction;
    }

    bool CheckIfOccluded(int count)
    {
        var isOccluded = false;

        var nearestDistance = CheckCameraPoints(TargetLookAt.position, desiredPosition);

        if (nearestDistance != -1)
        {
            if (count < MaxOcclusionChecks)
            {
                isOccluded = true;
                Distance -= OcclusionDistanceStep;

                if (Distance < 0.25f)
                    Distance = 0.25f;
            }
            else
            {
                Distance = nearestDistance - Camera.mainCamera.nearClipPlane;
            }

            desiredDistance = Distance;
            distanceSmooth = DistanceResumeSmooth;
        }

        return isOccluded;
    }

    float CheckCameraPoints(Vector3 from, Vector3 to)
    {
        var nearestDistance = -1f;

        RaycastHit hitInfo;

        Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

        // Draw lines in the editor to make it easier to visualize
        Debug.DrawLine(from, to + transform.forward * -camera.nearClipPlane, Color.red);
        Debug.DrawLine(from, clipPlanePoints.UpperLeft);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft);
        Debug.DrawLine(from, clipPlanePoints.UpperRight);
        Debug.DrawLine(from, clipPlanePoints.LowerRight);

        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

        if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
            nearestDistance = hitInfo.distance;

        if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
            {
                nearestDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
            {
                nearestDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
            {
                nearestDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < nearestDistance || nearestDistance == -1)
            {
                nearestDistance = hitInfo.distance;
            }
        }

        return nearestDistance;
    }

    void ResetDesiredDistance()
    {
        if (desiredDistance < preOccludedDistance)
        {
            var pos = CalculatePosition(mouseY, mouseX, preOccludedDistance);

            var nearestDistance = CheckCameraPoints(TargetLookAt.position, pos);

            if (nearestDistance == -1 || nearestDistance > preOccludedDistance)
            {
                desiredDistance = preOccludedDistance;
            }
        }
    }

    void UpdatePosition()
    {
        var positionX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        var positionY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        var positionZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);

        position = new Vector3(positionX, positionY, positionZ);

        transform.position = position;

        transform.LookAt(TargetLookAt);
    }

    public void Reset()
    {
        mouseX = 0;
        mouseY = 10;
        Distance = startDistance;
        desiredDistance = Distance;
        preOccludedDistance = Distance;
    }

    public static void UseExistingOrCreateNewMainCamera()
    {
        GameObject tempCamera;
        GameObject targetLookAt;
        TP_Camera myCamera;

        if (Camera.mainCamera != null)
        {
            tempCamera = Camera.mainCamera.gameObject;
        }
        else
        {
            tempCamera = new GameObject("Main Camera");
            tempCamera.AddComponent<Camera>();
            tempCamera.tag = "MainCamera";
        }

        if (!tempCamera.GetComponent<TP_Camera>())
        tempCamera.AddComponent<TP_Camera>();
        
        myCamera = tempCamera.GetComponent<TP_Camera>();

        targetLookAt = GameObject.Find("targetLookAt") as GameObject;

        if (targetLookAt == null)
        {
            targetLookAt = new GameObject("targetLookAt");
            targetLookAt.transform.position = Vector3.zero;
        }

        myCamera.TargetLookAt = targetLookAt.transform;
    }
}
