using System.Collections;
using UnityEngine;

public class playterMove : MonoBehaviour
{
    public Camera playerCamera;
    public CharacterController characterController;

    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    private bool isRunning = false;
    private float gravity = 10f;
    private Vector3 moveDirection;
    private float actualHorizontalSpeed;
    private float actualSpeed;
    private Vector3 previousPosition;
    private bool isGrounded;
    public float jumpForce = 5f;

    public float viewSensitivity = 120f;
    float xRotation;
    private bool canMouseRotate = true;
    private bool canMove = true;
    private bool canSprint = true;
    public float rotationSpeed = 5f;
    private float targetZRotation = 0.0f;
    private float currentZRotation = 0.0f;

    private float timer = 0.0f;
    public float bobbingSpeed = 6f;
    public float sprintBobbingSpeed = 9f;
    public float bobbingAmount = 0.02f;
    public float sprintBobbingAmount = 0.035f;
    private float midpoint = 0.7f;

    // --- Nuevo sistema de pasos ---
    public AudioSource footstepAudioSource;   // Asignar en Inspector
    public AudioClip footstepClip;            // Asignar en Inspector
    public float stepInterval = 0.5f;         // Tiempo entre pasos
    private float stepTimer = 0f;

    void Start()
    {
        foreach (Transform findObj in this.transform)
        {
            if (findObj.name == "PlayerCamera")
            {
                playerCamera = findObj.GetComponent<Camera>();
            }
        }

        characterController = transform.GetComponent<CharacterController>();
        Cursor.visible = false;
        LockCursor();
    }

    void Update()
    {
        if (canMove) Move();
        if (canMouseRotate && !Input.GetKey(KeyCode.R)) View();
    }

    private void Move()
    {
        actualHorizontalSpeed = ((new Vector3(transform.position.x, 0f, transform.position.z) - new Vector3(previousPosition.x, 0f, previousPosition.z)).magnitude) / Time.deltaTime;
        actualSpeed = ((transform.position - previousPosition).magnitude) / Time.deltaTime;
        previousPosition = transform.position;

        if (characterController.isGrounded)
        {
            isGrounded = true;

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);

            if (Input.GetKey(KeyCode.LeftShift) && canSprint)
            {
                isRunning = true;
                moveDirection *= sprintSpeed;
            }
            else
            {
                isRunning = false;
                moveDirection *= walkSpeed;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                moveDirection.y = jumpForce;
                isGrounded = false;
            }

            if (Input.GetKey(KeyCode.D)) targetZRotation = -1.5f;
            else if (Input.GetKey(KeyCode.A)) targetZRotation = 1.5f;
            else targetZRotation = 0.0f;

            currentZRotation = Mathf.Lerp(currentZRotation, targetZRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, currentZRotation);

            // --- Head bobbing ---
            if (actualHorizontalSpeed > 0.1f)
            {
                timer += Time.deltaTime * (isRunning ? sprintBobbingSpeed : bobbingSpeed);
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    midpoint + Mathf.Sin(timer) * (isRunning ? sprintBobbingAmount : bobbingAmount),
                    playerCamera.transform.localPosition.z
                );
            }
            else
            {
                timer = 0;
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    midpoint,
                    playerCamera.transform.localPosition.z
                );
            }

            // --- Nuevo sistema de pasos ---
            if (actualHorizontalSpeed > 0.1f)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= stepInterval)
                {
                    PlayFootstep();
                    stepTimer = 0f;
                }
            }
            else
            {
                stepTimer = 0f;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void View()
    {
        float inputX = Input.GetAxis("Mouse X") * viewSensitivity * Time.deltaTime;
        float inputY = Input.GetAxis("Mouse Y") * viewSensitivity * Time.deltaTime;

        xRotation -= inputY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * inputX);
    }
    public void SetPlayerControl(bool state)
    {
        canMove = state;
        canMouseRotate = state;
        canSprint = state;
    }

    void PlayFootstep()
    {
        if (footstepClip != null && footstepAudioSource != null)
        {
            footstepAudioSource.PlayOneShot(footstepClip);
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
