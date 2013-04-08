        public var swimming:boolean = false;

    

    private var jumpSpeed:float = 8.0;

    private var gravity:float = 20.0;

    private var runSpeed:float = 10.0;

    private var walkSpeed:float = 4.0;

    private var rotateSpeed:float = 250.0;

    private var walkBackSpeed:float = 3.0;

    private var speedforwalk:float = 0.0;

     

    private var grounded:boolean = false;

    private var moveDirection:Vector3 = Vector3.zero;

    private var isWalking:boolean = false;

    private var moveStatus:String = "idle";

    private var jumping:boolean = false;

    private var moveSpeed:float = 0.0;

    private var mouseSideButton:boolean = false;

    private var pbuffer:float = 0.0;        //Cooldownpuffer for SideButtons

    private var coolDown:float = 0.5;       //Cooldowntime for SideButtons

     

    function Update ()

    {

       // Only allow movement and jumps while grounded

       if(grounded) {

       

        moveDirection = new Vector3((Input.GetMouseButton(1) ? Input.GetAxis("Horizontal") : 0),0,Input.GetAxis("Vertical"));

        

        //pushbuffer

        if(pbuffer>0)

            pbuffer -=Time.deltaTime;

        if(pbuffer<0)pbuffer=0;

                        

        //Sidebuttonmovement

        if(Input.GetAxis("Toggle Move") && pbuffer == 0){

                pbuffer=coolDown;

                mouseSideButton = !mouseSideButton; 

        }

        //L+R MouseButton Movement

         if (Input.GetMouseButton(0) && Input.GetMouseButton(1) || mouseSideButton)

            moveDirection.z += 1;

         if(Input.GetAxis("Strafing") != 0)

            moveDirection.x -= Input.GetAxis("Strafing");

            

          // if moving forward and to the side at the same time, compensate for distance

          // TODO: may be better way to do this?

          if(Input.GetMouseButton(1) && Input.GetAxis("Horizontal") && Input.GetAxis("Vertical")) {

             moveDirection *= .7;

          }

           

          moveDirection = transform.TransformDirection(moveDirection);

        

        //Slow down back and sidemovements

        speedforwalk = ((Input.GetAxis("Vertical") < 0) || (Input.GetMouseButton(1) && Input.GetAxis("Horizontal"))) ? walkBackSpeed : walkSpeed;

                          

          moveDirection *= isWalking ? speedforwalk : runSpeed;

           

          moveDirection*= swimming ? 0.7 : 1;

            

          moveStatus = "idle";

          if(moveDirection != Vector3.zero)

             moveStatus = isWalking ? "walking" : "running";

           

          // Jump!

          if(Input.GetButton("Jump"))

             moveDirection.y = jumpSpeed;

       }

       

       // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.

       if(Input.GetMouseButton(1)) {

          transform.rotation = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0);

       } else {

          transform.Rotate(0,Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);

       }

       

       // Hold "Run" to run

       isWalking = Input.GetAxis("Run") ? false : true;

 

       //Apply gravity

       moveDirection.y -= gravity * Time.deltaTime;

       

       //Move controller

       var controller:CharacterController = GetComponent(CharacterController);

       var flags = controller.Move(moveDirection * Time.deltaTime);

       grounded = (flags & CollisionFlags.Below) != 0;

    }