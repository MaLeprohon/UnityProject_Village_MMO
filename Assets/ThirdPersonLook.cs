using UnityEngine;
using System.Collections;

// ThirdPersonLook is used on the Controller with Axes set to ControllerLook and target is the Main Camera

// To make an third person style character from the default FPS character
// Add the FPS Controller prefab to the scene
// Drag the animated character you will be using to the controller, so it is first child of controller, above Graphics and Main Camera
// Adjust the position (mostly height) so your character fits into the graphic capsule (as it shows your collider)
// Uncheck Mesh Renderer on Graphics so it is not drawn
// Add this scrip to your CameraScripts folder
// Replace the mouseLook script in the Controller and Main Camera with this one
// On the controller, drag the Main Camera to target and select Axes Controller Look
// On the Main Camera, drag the Controller to the Target and set Axes to Camera Look
// Replace the FPSWalker script on the controller with the Third Person Controller script
// Try it out
// You may need to comment the Idle / Walk animation changes in the Walker script

[AddComponentMenu("Camera-Control/Mouse Look")]
public class ThirdPersonLook : MonoBehaviour {

// select for Controller or Camera, so we only have one script
public enum RotationAxes { ControllerLook = 0, CameraLook = 1}
public RotationAxes axes = RotationAxes.ControllerLook;

// select the other so we have that transform for calculations
public Transform target; 

// xand Y mouse sensitivity
public float sensitivityX = 15F;
public float sensitivityY = 10F;

// horizontal rotation limits
public float minimumX = -360F;
public float maximumX = 360F;

// vertical rotation limits
public float minimumY = -90F;
public float maximumY = 90F;

// how close the camera is allowed to objects
public float offsetFromWall = 0.2f;

// Min and max for scroll wheel zoom
public float startDistance = 1.0f;
public float maxDistance = 20; 
public float minDistance = 0.01f; 

// how fast the camera swings back behind player when they start to move
public float CameraSwingBack = 250.0f;

// how fast the scroll wheel zoom works
public int zoomRate = 40; 

// vertical offset so the camera will circle the player's head
public float CameraYOffset = .6f;

private float desiredDistance; 
private float rotationX = 0F;
private float rotationY = 0F;

Quaternion originalRotation;

void Update ()
{
// This section is for the Controller, just to rotate the whole character
if (axes == RotationAxes.ControllerLook)
{
if (Input.GetMouseButton(1)) 
{
rotationX += Input.GetAxis("Mouse X") * sensitivityX;
rotationX = ClampAngle (rotationX, minimumX, maximumX);

Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
transform.rotation = xQuaternion;

}
}
// This section is for the camera to allow vertical and horizontal swing, plus zoom, all with collision detection of the camera
else if (axes == RotationAxes.CameraLook)
{
Vector3 vCamOffset;
RaycastHit hit = new RaycastHit();

// zoom in and out along current camera axis with scroll wheel
float Dis; 
if(Input.GetAxis ("Mouse ScrollWheel") != 0){
Dis = Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs (desiredDistance); 
// without some limit the scroll speed gets very small close in!
if(Mathf.Abs (Dis) < .1f) {
if(Dis < 0) Dis = -.1f;
else Dis = .1f;
}
desiredDistance -= Dis; 
desiredDistance = Mathf.Clamp (desiredDistance, minDistance, maxDistance); 

if (target) {
// raycast needs world values, position is world, but we need to add the controller rotation to the camera ray to know which way it really points
// if a hit is found, shorten up the camera distance to stay between character and object hit. We don't care what it was
if(Physics.Raycast(target.position,target.rotation * transform.localPosition,out hit,desiredDistance + offsetFromWall)){
desiredDistance = hit.distance - offsetFromWall;
}
}
// for simplicity, remove the camera rotation from the position to get the initial ray where z is the distance we want
Quaternion rot = Quaternion.Inverse(transform.localRotation);
vCamOffset = rot * transform.localPosition;
// set to new distance
vCamOffset.z = -desiredDistance;
// put it back in with the rotation
transform.localPosition = transform.localRotation * vCamOffset;
}

if (!target) return; 

Vector3 vTargetOffset;

// tilt camera up and down if EITHER mouse button is down (so vertical camera works while moving, but character, not camera rotates while moving
if ((Input.GetMouseButton(0))||(Input.GetMouseButton( 1))) 
{
rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
rotationY = ClampAngle (rotationY, minimumY, maximumY);

// if ONLY left mouse button is down swing camera around
if (Input.GetMouseButton(0) && !Input.GetMouseButton(1)) 
{ 
rotationX += Input.GetAxis("Mouse X") * sensitivityX;
rotationX = ClampAngle (rotationX, minimumX, maximumX);
} else {
// if right only or both buttons are down, swing the camera back behind the character
if(rotationX > 0){
rotationX -= CameraSwingBack * Time.deltaTime;
if(rotationX < 0) rotationX = 0;
} else if(rotationX < 0){
rotationX += CameraSwingBack * Time.deltaTime;
if(rotationX > 0) rotationX = 0;
}
}

Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, Vector3.left);

Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
// add rotations in the right order (around then up and down) to get the desired movement
xQuaternion *= yQuaternion;
yQuaternion = xQuaternion;

// yQuaternion now has both rotations in the correct order

vTargetOffset = new Vector3 (0, CameraYOffset, -desiredDistance);
transform.localPosition = yQuaternion * vTargetOffset;
// check to make sure camera rotation hasn't made it collide with anything
if(Physics.Raycast(target.position,target.rotation * transform.localPosition,out hit,desiredDistance + offsetFromWall)){
desiredDistance = hit.distance - offsetFromWall;
vTargetOffset = new Vector3 (0, CameraYOffset, -desiredDistance);
transform.localPosition = yQuaternion * vTargetOffset;
}
// apply it
transform.localRotation = originalRotation * yQuaternion;
}	
}
}

void Start ()
{
desiredDistance = startDistance; 

// Make the rigid body not change rotation
if (rigidbody)
rigidbody.freezeRotation = true;
originalRotation = transform.localRotation;
}

public static float ClampAngle (float angle, float min, float max)
{
if (angle < -360F)
angle += 360F;
if (angle > 360F)
angle -= 360F;
return Mathf.Clamp (angle, min, max);
}
}