using UnityEngine;
using System.Collections;

public class TP_MotorLegacy : MonoBehaviour
{
    public static TP_MotorLegacy Instance;

    public float ForwardSpeed = 10f;
    public float BackwardSpeed = 2f;
    public float StrafingSpeed = 5f;
    public float SlideSpeed = 10f;
    public float JumpSpeed = 6f;
    public float Gravity = 21f;
    public float TerminalVelocity = 20f;
    public float SlideThreshold = 0.6f;
    public float MaxControllableSlideMagnitude = 0.4f;
    public float FatalFallHeight = 7f;

    private Vector3 slideDirection;
    private bool isFalling = false;

    public bool IsFalling
    {
        get { return isFalling; }
        set
        {
            isFalling = value;
        }
    }

    public Vector3 MoveVector { get; set; }
    public float VerticalVelocity { get; set; }
    public bool IsSliding { get; set; }

    void Awake()
    {
        Instance = this;
    }


    public void UpdateMotor()
    {
        SnapAlignCharacterWithCamera();
        ProcessMotion();
    }

    public void ResetMotor()
    {
        VerticalVelocity = MoveVector.y;
        MoveVector = Vector3.zero;
    }

    void ProcessMotion()
    {
        // Transform MoveVector to World Space
        MoveVector = transform.TransformDirection(MoveVector);

        // Normalize MoveVector if Magnitude > 1
        if (MoveVector.magnitude > 1)
            MoveVector = Vector3.Normalize(MoveVector);

        // Apply slideing if applicable
        ApplySlide();

        // Multiply MoveVector by MoveSpeed
        MoveVector *= MoveSpeed();

        // Reapply VerticleVelocity MoveVector.y
        MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);

        // Apply gravity
        ApplyGravity();

        // Move the Character in World Space
        TP_ControllerLegacy.CharacterController.Move(MoveVector * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (MoveVector.y > -TerminalVelocity)
            MoveVector = new Vector3(MoveVector.x, MoveVector.y - Gravity * Time.deltaTime, MoveVector.z);

        if (TP_ControllerLegacy.CharacterController.isGrounded && MoveVector.y < -1)
            MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
    }

    void ApplySlide()
    {
        if (!TP_ControllerLegacy.CharacterController.isGrounded)
            return;

        slideDirection = Vector3.zero;

        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
        {
            if (hitInfo.normal.y < SlideThreshold)
            {
                slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
                if (!IsSliding)
                {
                    TP_AnimatorLegacy.Instance.Slide();
                }
                IsSliding = true;
            }
            else
            {
                IsSliding = false;
            }
        }

        if (slideDirection.magnitude < MaxControllableSlideMagnitude)
            MoveVector += slideDirection;
        else
        {
            MoveVector = slideDirection;
        }
    }

    public void Jump()
    {
        if (TP_ControllerLegacy.CharacterController.isGrounded)
            VerticalVelocity = JumpSpeed;
    }

    void SnapAlignCharacterWithCamera()
    {
        if (MoveVector.x != 0 || MoveVector.z != 0)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                    Camera.mainCamera.transform.eulerAngles.y,
                                                    transform.eulerAngles.z);
        }
    }

    float MoveSpeed()
    {
        var moveSpeed = 0f;

        switch (TP_AnimatorLegacy.Instance.MoveDirection)
        {
            case TP_AnimatorLegacy.Direction.Stationary:
                moveSpeed = 0;
                break;
            case TP_AnimatorLegacy.Direction.Forward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_AnimatorLegacy.Direction.Backward:
                moveSpeed = BackwardSpeed;
                break;
            case TP_AnimatorLegacy.Direction.Left:
                moveSpeed = StrafingSpeed;
                break;
            case TP_AnimatorLegacy.Direction.Right:
                moveSpeed = StrafingSpeed;
                break;
            case TP_AnimatorLegacy.Direction.LeftForward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_AnimatorLegacy.Direction.RightForward:
                moveSpeed = ForwardSpeed;
                break;
            case TP_AnimatorLegacy.Direction.LeftBackward:
                moveSpeed = BackwardSpeed;
                break;
            case TP_AnimatorLegacy.Direction.RightBackward:
                moveSpeed = BackwardSpeed;
                break;
        }

        if (IsSliding)
            moveSpeed = SlideSpeed;

        return moveSpeed;
    }
}