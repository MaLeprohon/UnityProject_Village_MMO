var speed = 6.0;

var runSpeed = 9.0;

var rotateSpeed = 90;

 

var gravity = 20.0;

 

private var moveDirection = Vector3.zero;

private var grounded : boolean = false;

 

function FixedUpdate() {

    if (grounded) {

        // We are grounded, so recalculate movedirection directly from axes

        moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical")); //Determine the player's forward speed based upon the input.

        

        moveDirection = transform.TransformDirection(moveDirection); //make the direction relative to the player.

        if(Input.GetButton("Jump")) {

            moveDirection *= runSpeed;

        }

        else {

            moveDirection *= speed;

        }

    }

 

    // Apply gravity

    moveDirection.y -= gravity * Time.deltaTime;

    

    // Move the controller

    var controller : CharacterController = GetComponent(CharacterController);

    var flags = controller.Move(moveDirection * Time.deltaTime);

    transform.Rotate(0, rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), 0);

    

    grounded = (flags & CollisionFlags.CollidedBelow) != 0;

}

 

@script RequireComponent(CharacterController)