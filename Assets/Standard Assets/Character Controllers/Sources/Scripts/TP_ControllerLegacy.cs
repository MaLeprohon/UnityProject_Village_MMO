using UnityEngine;
using System.Collections;

public class TP_ControllerLegacy : MonoBehaviour
{
    public static CharacterController CharacterController;
    public static TP_ControllerLegacy Instance;
    //public ConnectionListener Listener;

    void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        Instance = this;
        //Listener = GameObject.Find("ConnectionListener").GetComponent<ConnectionListener>();

        /*if (!Listener.InCharacterCreation)
        {
            TP_Camera.UseExistingOrCreateNewMainCamera();
        }*/
    }

    void Update()
    {
        if (Camera.mainCamera == null)
            return;

        TP_MotorLegacy.Instance.ResetMotor();

        if (TP_AnimatorLegacy.Instance.State != TP_AnimatorLegacy.CharacterState.Using /*&& !Listener.InCharacterCreation*/)
        {
            GetLocomotionInput();
            HandleActionInput();
        }

        TP_MotorLegacy.Instance.UpdateMotor();
    }

    void GetLocomotionInput()
    {
        var deadZone = 0.1f;

        if (Input.GetAxis("Vertical") > deadZone || Input.GetAxis("Vertical") < -deadZone)
            TP_MotorLegacy.Instance.MoveVector += new Vector3(0, 0, Input.GetAxis("Vertical"));

        if (Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone)
            TP_MotorLegacy.Instance.MoveVector += new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        TP_AnimatorLegacy.Instance.DetermineCurrentMoveDirection();
    }

    void HandleActionInput()
    {
        if (Input.GetButton("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }
    }

    public void Jump()
    {
        TP_MotorLegacy.Instance.Jump();
        TP_AnimatorLegacy.Instance.Jump();
    }

    public void Use()
    {
        TP_AnimatorLegacy.Instance.Use();
    }
}