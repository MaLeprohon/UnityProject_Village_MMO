using UnityEngine;
using System.Collections;

public class TP_AnimatorLegacy : MonoBehaviour
{
    public enum Direction
    {
        Stationary, Forward, Backward, Left, Right,
        LeftForward, RightForward, LeftBackward, RightBackward
    }

    public enum CharacterState
    {
        Idle, Running, WalkBackwards, StrafingLeft, StrafingRight, Jumping,
        Falling, Landing, Sliding, Using, ActionLocked
    }

    public static TP_AnimatorLegacy Instance;

    //public GameInitializer GameInitializer;
    //public ConnectionListener Listener;

    private CharacterState lastState;

    public Direction MoveDirection { get; set; }
    public CharacterState State { get; set; }

    #region Animation Names
        // Animation names list. TODO: REPLACE YOUR OWN ANIMATION CLIP NAMES HERE

        public string IdleAnimationName = "Idle";
        public string WalkAnimationName = "Walk";
        public string RunAnimationName = "Run";
        public string WalkBackwardsAnimationName = "Walk";
        public string StrafeLeftAnimationName = "Run";
        public string StrafeRightAnimationName = "Run";
        public string JumpAnimationName = "Jump";
        public string FallAnimationName = "Fall";
        public string WalkLandAnimationName = "Land";
        public string RunLandAnimationName = "Land";
        public string UsingAnimationName = "Shuffle";
        public string SlidingAnimationName = "Shuffle";

    #endregion

    void Awake()
    {
        Instance = this;
        //Listener = GameObject.Find("ConnectionListener").GetComponent<ConnectionListener>();
    }

    /*void Start()
    {
        if (!Listener.InCharacterCreation)
            GameInitializer = GameObject.Find("GameInitializer").GetComponent<GameInitializer>();
    }*/

    void Update()
    {
        DetermineCurrentState();
        ProcessCurrentState();
    }

    public void DetermineCurrentMoveDirection()
    {
        var forward = false;
        var backward = false;
        var left = false;
        var right = false;

        if (TP_MotorLegacy.Instance.MoveVector.z > 0)
            forward = true;
        if (TP_MotorLegacy.Instance.MoveVector.z < 0)
            backward = true;
        if (TP_MotorLegacy.Instance.MoveVector.x > 0)
            right = true;
        if (TP_MotorLegacy.Instance.MoveVector.x < 0)
            left = true;

        if (forward)
        {
            if (left)
                MoveDirection = Direction.LeftForward;
            else if (right)
                MoveDirection = Direction.RightForward;
            else
                MoveDirection = Direction.Forward;
        }
        else if (backward)
        {
            if (left)
                MoveDirection = Direction.LeftBackward;
            else if (right)
                MoveDirection = Direction.RightBackward;
            else
                MoveDirection = Direction.Backward;
        }
        else if (left)
        {
            MoveDirection = Direction.Left;
        }
        else if (right)
        {
            MoveDirection = Direction.Right;
        }
        else
        {
            MoveDirection = Direction.Stationary;
        }
    }

    void DetermineCurrentState()
    {
        if (!TP_ControllerLegacy.CharacterController.isGrounded)
        {
            if (State != CharacterState.Falling &&
                State != CharacterState.Jumping &&
                State != CharacterState.Landing)
            {
                // We should be falling
                Fall();
            }
        }

        if (State != CharacterState.Falling &&
            State != CharacterState.Jumping &&
            State != CharacterState.Landing &&
            State != CharacterState.Using &&
            State != CharacterState.Sliding)
        {
            switch (MoveDirection)
            {
                case Direction.Stationary:
                    State = CharacterState.Idle;
                    break;
                case Direction.Forward:
                    State = CharacterState.Running;
                    break;
                case Direction.Backward:
                    State = CharacterState.WalkBackwards;
                    break;
                case Direction.Left:
                    State = CharacterState.StrafingLeft;
                    break;
                case Direction.Right:
                    State = CharacterState.StrafingRight;
                    break;
                case Direction.LeftForward:
                    State = CharacterState.Running;
                    break;
                case Direction.RightForward:
                    State = CharacterState.Running;
                    break;
                case Direction.LeftBackward:
                    State = CharacterState.WalkBackwards;
                    break;
                case Direction.RightBackward:
                    State = CharacterState.WalkBackwards;
                    break;
            }
        }
    }

    void ProcessCurrentState()
    {
        switch (State)
        {
            case CharacterState.Idle:
                Idle();
                break;
            case CharacterState.Running:
                Running();
                break;
            case CharacterState.WalkBackwards:
                WalkBackwards();
                break;
            case CharacterState.StrafingLeft:
                StrafingLeft();
                break;
            case CharacterState.StrafingRight:
                StrafingRight();
                break;
            case CharacterState.Jumping:
                Jumping();
                break;
            case CharacterState.Falling:
                Falling();
                break;
            case CharacterState.Landing:
                Landing();
                break;
            case CharacterState.Sliding:
                Sliding();
                break;
            case CharacterState.Using:
                Using();
                break;
            case CharacterState.ActionLocked:
                break;
        }
    }

    #region Character State Methods

    void Idle()
    {
        animation.CrossFade(IdleAnimationName);
    }

    void Running()
    {
        animation.CrossFade(RunAnimationName);
    }

    void WalkBackwards()
    {
        animation.CrossFade(WalkBackwardsAnimationName);
    }

    void StrafingLeft()
    {
        animation.CrossFade(StrafeLeftAnimationName);
    }

    void StrafingRight()
    {
        animation.CrossFade(StrafeRightAnimationName);
    }

    void Using()
    {
        if (!animation.isPlaying)
        {
            State = CharacterState.Idle;
            animation.CrossFade(IdleAnimationName);
            //GameInitializer.CurrentKeystate = KeyState.Still;
        }
    }

    void Jumping()
    {
        if ((!animation.isPlaying && TP_ControllerLegacy.CharacterController.isGrounded) ||
            TP_ControllerLegacy.CharacterController.isGrounded)
        {
            if (lastState == CharacterState.Running)
            {
                animation.CrossFade(RunLandAnimationName);
            }
            else
            {
                animation.CrossFade(WalkLandAnimationName);
            }
            State = CharacterState.Landing;
        }
        else if (!animation.IsPlaying(JumpAnimationName))
        {
            State = CharacterState.Falling;
            animation.CrossFade(FallAnimationName);
            TP_MotorLegacy.Instance.IsFalling = true;
        }
        else
        {
            State = CharacterState.Jumping;
            // Help determine if we fell too far
        }
    }

    void Falling()
    {
        if (TP_ControllerLegacy.CharacterController.isGrounded)
        {
            if (lastState == CharacterState.Running)
            {
                animation.CrossFade(RunLandAnimationName);
                //GameInitializer.CurrentKeystate = KeyState.Landing;
            }
            else
            {
                animation.CrossFade(WalkLandAnimationName);
                //GameInitializer.CurrentKeystate = KeyState.Landing;
            }
            State = CharacterState.Landing;
        }
    }

    void Landing()
    {
        if (lastState == CharacterState.Running)
        {
            if (!animation.IsPlaying(RunLandAnimationName))
            {
                State = CharacterState.Running;
                animation.Play(RunAnimationName);
                //GameInitializer.CurrentKeystate = KeyState.Still;
            }
        }
        else
        {
            if (!animation.IsPlaying(WalkLandAnimationName))
            {
                State = CharacterState.Idle;
                animation.Play(IdleAnimationName);
                //GameInitializer.CurrentKeystate = KeyState.Still;
            }
        }

        TP_MotorLegacy.Instance.IsFalling = false;
    }

    void Sliding()
    {
        if (!TP_MotorLegacy.Instance.IsSliding)
        {
            State = CharacterState.Idle;
            animation.CrossFade(IdleAnimationName);
            //GameInitializer.CurrentKeystate = KeyState.Still;
        }
    }

    #endregion

    #region Start Action Methods

    public void Use()
    {
        State = CharacterState.Using;
        animation.CrossFade(UsingAnimationName);
        //GameInitializer.CurrentKeystate = KeyState.Use;
    }

    public void Jump()
    {
        if (!TP_ControllerLegacy.CharacterController.isGrounded || State == CharacterState.Jumping)
            return;

        lastState = State;
        State = CharacterState.Jumping;
        animation.CrossFade(JumpAnimationName);
        //GameInitializer.CurrentKeystate = KeyState.Jump;
    }

    public void Fall()
    {
        if (TP_MotorLegacy.Instance.VerticalVelocity > -5)
            return;

        lastState = State;
        State = CharacterState.Falling;
        TP_MotorLegacy.Instance.IsFalling = true;
        animation.CrossFade(FallAnimationName);
        //GameInitializer.CurrentKeystate = KeyState.Falling;
    }

    public void Slide()
    {
        State = CharacterState.Sliding;
        animation.CrossFade(SlidingAnimationName);
        //GameInitializer.CurrentKeystate = KeyState.Sliding;
    }

    #endregion
}