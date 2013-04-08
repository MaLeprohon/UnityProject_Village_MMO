var speed = 6.0; // standard running speed
var walkSpeed = .8; // walk speed adjust both for your charcter so feet don't slide on ground
var jumpSpeed = 8.0;
var gravity = 20.0;
var Character : Transform;

private var moveDirection = Vector3.zero;
private var grounded : boolean = false;
// a flag to determine idle vs walking/running (so animations do not get started over and over)
private var walking : boolean = false;
// a flag to init idle animation at beginning
private var startup : boolean = true;
// a flag to determine walking vs running
private var running : boolean = true;

function FixedUpdate() {
if (grounded) {
// We are grounded, so recalculate movedirection directly from axes
moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

// if both mouse buttons are down, move forward
if (Input.GetMouseButton(0) && Input.GetMouseButton(1)) 
{ 
moveDirection.z = 1;
}

moveDirection = transform.TransformDirection(moveDirection);
if(running == true){
moveDirection *= speed;
} else {
moveDirection *= walkSpeed;
}

if (Input.GetButton ("Jump")) {
moveDirection.y = jumpSpeed;
}

// auto toggle between idle and walking animations - based on run / walk switch
if(Character){
// toggle between walk and run with <left shift> R
if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)){
if(running == true) {
running = false;
if(walking == true) Character.animation.CrossFade("walk");
} else {
running = true;
if(walking == true) Character.animation.CrossFade("run");
}
}
if(startup == true){
startup = false;
Character.animation.Play("idle");
}
if((moveDirection == Vector3.zero)&&(walking == true)){
walking = false;
Character.animation.CrossFade("idle");
} else {
if((moveDirection != Vector3.zero)&&(walking == false)){
walking = true;
if(running == true){
Character.animation.CrossFade("run");
} else {
Character.animation.CrossFade("walk");
}
}
}
}
}
// Apply gravity
moveDirection.y -= gravity * Time.deltaTime;

// Move the controller
var controller : CharacterController = GetComponent(CharacterController);
var flags = controller.Move(moveDirection * Time.deltaTime);
grounded = (flags & CollisionFlags.CollidedBelow) != 0;

}

@script RequireComponent(CharacterController)