using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [Header("References")]
    public GameObject currentCamera;
    public GameObject _normalCamera;
    public GameObject _aimCamera;
    private PlayerController _playerController;
    private PlayerManager _playerManager;
    private CameraInput _cameraInputActions;
    private CinemachineVirtualCamera _virtualCamera;

    [Header("Values")]
    private float currentFOV;
    private float currentVerticalAngle = 0f;
    private float currentHorizontalAngle = 0f;
    public float normalSensitivity = 100;
    public float aimSensitivity = 100;
    private float targetFOV, baseFOV;

    private void Awake(){
        _playerController = GetComponentInParent<PlayerController>();
        _playerManager = GetComponentInParent<PlayerManager>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentCamera = _normalCamera;

        _virtualCamera = currentCamera.GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        baseFOV  = _virtualCamera.m_Lens.FieldOfView;
        targetFOV = baseFOV;

        _cameraInputActions = new CameraInput();
        _cameraInputActions.Player.Look.performed += OnLook;
        _cameraInputActions.Enable();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mouseInput = context.ReadValue<Vector2>();
        
        currentHorizontalAngle += mouseInput.x * Time.deltaTime * (_playerController.isAiming ? aimSensitivity : normalSensitivity);
        //<<<<<<< HEAD:Assets/Scripts/Player/CameraController.cs
        currentVerticalAngle -= mouseInput.y * Time.deltaTime * (_playerController.isAiming ? aimSensitivity : normalSensitivity);

        currentVerticalAngle += -mouseInput.y * Time.deltaTime * (_playerController.isAiming ? aimSensitivity : normalSensitivity);
        //>>>>>>> enemy-IA:Assets/Scripts/CameraController.cs
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -80, 80);

        transform.localRotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        CameraChange();
        ChangeFOV();
    }

    private void ChangeFOV(){

        if (_playerController.isMoving && _playerController.isSprinting && !_playerController.isJumping && !_playerController.isSliding && !_playerController.isWallRunning && !_playerController.isDashing && !_playerController.isRolling && !_playerController.isCrouching && _playerController.controller.isGrounded) targetFOV = baseFOV + 10;
        else if (_playerController.isJumping && !_playerController.isDashing && !_playerController.isSprinting) targetFOV = baseFOV + 5;
        else if (_playerController.isJumping && !_playerController.isDashing && _playerController.isSprinting) targetFOV = baseFOV + 15;
        else if (_playerController.isSliding && _playerController.controller.isGrounded) targetFOV = baseFOV + 15;
        else if (_playerController.isSliding && !_playerController.controller.isGrounded) targetFOV = baseFOV + 20;
        else if (_playerController.isWallRunning) targetFOV = baseFOV + 25;
        else if (_playerController.isDashing) targetFOV = baseFOV + 20;
        else if (_playerController.isRolling && _playerController.controller.isGrounded) targetFOV = baseFOV + 5;
        else if (_playerController.isRolling && !_playerController.controller.isGrounded) targetFOV = baseFOV + 20;
        else if (_playerController.isCrouching) targetFOV = baseFOV - 5;
        else targetFOV = baseFOV;

        if (!Mathf.Approximately(_virtualCamera.m_Lens.FieldOfView, targetFOV)){
            _virtualCamera.m_Lens.FieldOfView = Mathf.SmoothDamp(_virtualCamera.m_Lens.FieldOfView, targetFOV, ref currentFOV, 0.1f);
        }
    }

    private void CameraChange(){
        if (_playerController.isAiming){
            if (currentCamera != _aimCamera) currentCamera = _aimCamera;
            if (!_aimCamera.activeSelf) _aimCamera.SetActive(true);
            if (_normalCamera.activeSelf) _normalCamera.SetActive(false);
        }
        else{
            if (currentCamera != _normalCamera) currentCamera = _normalCamera;
            if (_aimCamera.activeSelf) _aimCamera.SetActive(false);
            if (!_normalCamera.activeSelf) _normalCamera.SetActive(true);
        }   
    }

}

