using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public CharacterController controller;
    private PlayerManager _playerManager;
    public PlayerInputActions playerInputActions;
    private Vector2 movementInput;
    private Vector3 moveDirection;
    public GameObject _armature;
    public GameObject cameraHolder;
    public GameObject aimLookAt;
    public GameObject dashParticle;
    private GameObject spawnedDashParticle;
    
    [Header("Movements Speeds")]   
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float slidingSpeed = 8f;
    [SerializeField] private float wallRunSpeed = 10f;
    [SerializeField] private float jumpHeight = 2.5f;
    public float rotationSpeed = 360f;

    [Header("Player State")]
    public bool isMoving;
    public bool isCrouching = false;
    public bool isSprinting = false;
    public bool isJumping = false;
    public bool isSliding = false;
    public bool isRolling = false;
    public bool isDashing = false;
    public bool isWallRunning  = false;
    public bool isAiming = false;
    public bool isAttacking = false;
    public bool specialAttack = false;

    
    [Header("Movements Values")]
    private Quaternion targetRotation;
    private Quaternion lookRotation;
    public float currentSpeed;
    [HideInInspector] public float sprintInput;
    [SerializeField] private float gravity = -9.81f;

    [Header("Slide")]
    [SerializeField] private float slidingTime = 3f;
    [SerializeField] private float currentSlidingTime;

    [Header("Jump & Wall Jump")]
    private float verticalVelocity;
    public float jumpCounter;
    private RaycastHit forwardHit;
    public bool canWallJump = false;
    public bool needToWaitWallJump = false;

    [Header("Rolling")]    
    private Vector3 rollDestination;
    private float rollDuration = 1f;
    private float rollTimer = 0f;
    public bool hasRolled = false;
    Vector3 currentRollPosition;

    [Header("Dash")]
    private Vector3 dashDestination;
    private float dashDuration = 1f;
    private float dashTimer = 0f;
    private float currentLoadDashTime;
    private float maxLoadDashTime = 3;
    private bool canLoadDash = true;
    public bool loadDash = false;
    Vector3 currentDashPosition;
    private bool canDash;
    private float dashCoolDownTime = 0.5f;
    private float dashCoolDownTimer = 0f;

    [Header("Attack & Defense")]
    public float attackNumber;

    [Header("Wall Run")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float currentWallRunTime;
    [SerializeField] private float maxWallRunTime = 5;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    public bool wallLeft;
    public bool wallRight;
    private bool canWallRun = false;


    Vector3 rayOrigin;
    Ray ray;
    RaycastHit hit;

    private void Awake(){
        controller = GetComponent<CharacterController>();
        _playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
    
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Movement.performed += OnMovePerformed;
        playerInputActions.Player.Movement.canceled += OnMoveCanceled;

        playerInputActions.Player.Jump.performed += OnJumpPerformed;

        playerInputActions.Player.Crouch.performed += OnCrouching;
        playerInputActions.Player.Crouch.canceled += OnCrouchingCanceled;

        playerInputActions.Player.Sprint.performed += OnSprint;
        playerInputActions.Player.Sprint.canceled += OnSprintCanceled;

        playerInputActions.Player.Roll.performed += OnRoll;

        playerInputActions.Player.Aim.performed += OnAim;
        playerInputActions.Player.Aim.canceled += OnAimCanceled;

        playerInputActions.Player.Fire.performed += OnAttack;
        playerInputActions.Player.Fire.canceled += OnAttackCanceled;
        playerInputActions.Player.SpecialFire.performed += OnSpecialAttack;

        playerInputActions.Player.WeaponChange.performed += WeaponChange;

        playerInputActions.Player.MeleeAttack.performed += OnMeleeAttack;
        playerInputActions.Player.MeleeAttack.performed += OnMeleeAttackCanceled;

        playerInputActions.Enable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();

        if (!isMoving) isMoving = true;    
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {

        if (isMoving) isMoving = false;  

        movementInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (controller.isGrounded && !isJumping)
        {
            isJumping = true;
            jumpCounter ++;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);   
        }
        else if(canWallJump && !needToWaitWallJump && jumpCounter >= 1){
            verticalVelocity = Mathf.Sqrt((jumpHeight + 3)* -2f * gravity);   
            isJumping = true;
            jumpCounter ++;
            needToWaitWallJump = true;
            Invoke("ResetWaitWallJump", 0.9f);
        }
        else if(canWallRun){
            isWallRunning = !isWallRunning;
        }
    }

    void ResetWaitWallJump(){
        needToWaitWallJump = false;
        isJumping = false;
        canWallJump = false;
    }

    private void OnCrouching(InputAction.CallbackContext context){
        if(!isSprinting && !isCrouching){
            if (controller.isGrounded) isCrouching = true;
        } 
        else if (!isSliding){
            isSliding = true;  
        }
    }

    private void OnCrouchingCanceled(InputAction.CallbackContext context){
        if (isCrouching) isCrouching = false;
        if (isSliding) {
            isSliding = false;
            
        }
    }

    private void OnSprint(InputAction.CallbackContext context){
        if (controller.isGrounded && !isCrouching && !isSprinting) {
            isSprinting = true;
            sprintInput = 1;           
        }
        else if (!controller.isGrounded && !isSprinting && canDash){
            if (!loadDash && canLoadDash) loadDash = true;
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext context){
        if (isSprinting) isSprinting = false;
        sprintInput = 0;

        if (loadDash){
            loadDash = false;
            if (!isDashing && canDash) isDashing = true;
        }
    }

    void WaitForDashReset(){
        isDashing = false;  
        Destroy(spawnedDashParticle);
    }

    private void OnRoll(InputAction.CallbackContext context){
        if (!isRolling && !hasRolled)
        {
            isRolling = true;
            hasRolled = true;
            canDash = false;

            rollDestination = transform.position + _armature.transform.forward * currentSpeed;

            Invoke("WaitForRollReset", 1f);
        }
    }

    void WaitForRollReset(){
        if (hasRolled) hasRolled = false;
    }

    private void OnAim(InputAction.CallbackContext context){
        if (!isAiming) isAiming = true;
    }

    private void OnAimCanceled(InputAction.CallbackContext context){
        if (isAiming) isAiming = false;
    }

    private void OnAttack(InputAction.CallbackContext context){
        if (_playerManager.currentWeaponsPlace != Weapons.Melee){
            if (!isAttacking) isAttacking = true;
            attackNumber = Random.Range(0, 3);
        }
    }

    private void OnAttackCanceled(InputAction.CallbackContext context){
        if (isAttacking) isAttacking = false;
    }

    private void OnSpecialAttack(InputAction.CallbackContext context){
        if (!specialAttack) specialAttack = true;
    }

    private void WeaponChange(InputAction.CallbackContext context){
        StartCoroutine(DelayWeaponChange(0.2f));
    }

    IEnumerator DelayWeaponChange(float delay){
        if (_playerManager.currentWeaponsPlace == Weapons.Primary && _playerManager._secondWeapon != null){
            _playerManager.weaponChanging = true;
            yield return new WaitForSeconds(delay);
            _playerManager.currentWeapon = _playerManager.secondWeaponObject;
            _playerManager.currentWeaponsPlace = Weapons.Secondary;
            _playerManager.ChangeWeapons(false, true, false);
            StartCoroutine(_playerManager.ReBuildRig());
        }
        else if (_playerManager.currentWeaponsPlace == Weapons.Secondary && _playerManager._firstWeapon != null){
            _playerManager.weaponChanging = true;
            yield return new WaitForSeconds(delay);
            _playerManager.currentWeapon = _playerManager.firstWeaponObject;
            _playerManager.currentWeaponsPlace = Weapons.Primary;
            _playerManager.ChangeWeapons(true, false, false);
            StartCoroutine(_playerManager.ReBuildRig());
        }
        else if (_playerManager.currentWeaponsPlace == Weapons.Melee)
        {
            if (_playerManager._firstWeapon != null){
                _playerManager.weaponChanging = true;
                yield return new WaitForSeconds(delay);
                _playerManager.currentWeapon = _playerManager.firstWeaponObject;
                _playerManager.currentWeaponsPlace = Weapons.Primary;
                _playerManager.ChangeWeapons(true, false, false);
                StartCoroutine(_playerManager.ReBuildRig());
            }
            else if (_playerManager._secondWeapon != null){
                _playerManager.weaponChanging = true;
                yield return new WaitForSeconds(delay);
                _playerManager.currentWeapon = _playerManager.secondWeaponObject;
                _playerManager.currentWeaponsPlace = Weapons.Secondary;
                _playerManager.ChangeWeapons(false, true, false);
                StartCoroutine(_playerManager.ReBuildRig());
            }
        }
    }


    private void OnMeleeAttack(InputAction.CallbackContext context){
        if (!isAttacking) isAttacking = true;
        attackNumber = Random.Range(0, 3);
        if (_playerManager.currentWeapon != _playerManager.meleeWeaponObject){         
            _playerManager.weaponChanging = true;
            _playerManager.currentWeapon = _playerManager.meleeWeaponObject;
            _playerManager.currentWeaponsPlace = Weapons.Melee;
            _playerManager.ChangeWeapons(false, false, true);
            StartCoroutine(_playerManager.ReBuildRig());
        }

    }

    private void ResetAttack(){
        if (isAttacking) isAttacking = false;
    }

    private void OnMeleeAttackCanceled(InputAction.CallbackContext context){
        Invoke("ResetAttack", 0.5f);
    }

    private float CalculatedSpeed(){

        if (isMoving && !isSprinting) currentSpeed = walkSpeed;
        if (isMoving && isSprinting) currentSpeed = runSpeed;
        if (isSliding) currentSpeed = slidingSpeed;
        if (isCrouching) currentSpeed = crouchSpeed;
        if (isWallRunning) currentSpeed = wallRunSpeed;

        return currentSpeed;
    }

    private void Update()
    {   
        if (!controller.isGrounded && specialAttack) verticalVelocity = -Mathf.Sqrt(1000 * -2 * gravity);

        if (!canDash){
            dashCoolDownTimer += Time.deltaTime;
            if (dashCoolDownTimer >= dashCoolDownTime){
                canDash = true;
                dashCoolDownTimer = 0f;
            }
        }

        if (isAiming || isAttacking){

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            ray = Camera.main.ScreenPointToRay(screenCenter);

            if (Physics.Raycast(ray, out hit))
            {

                Vector3 worldAimTarget = hit.point;
                worldAimTarget.y = _armature.transform.position.y;
                targetRotation = Quaternion.LookRotation(worldAimTarget - _armature.transform.position);
            }
        }
        else{
            Vector3 direction = (cameraHolder.transform.forward * movementInput.y + cameraHolder.transform.right * movementInput.x).normalized;
            if (direction != Vector3.zero) lookRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);
        }


        _armature.transform.rotation = Quaternion.Slerp(_armature.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            float t = rollTimer / rollDuration;

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, rollDestination, ref currentRollPosition, 0.1f);
            
            if (!controller.isGrounded) smoothedPosition.y = verticalVelocity * Time.deltaTime;

            if (t >= 0.9){
                isRolling = false;
                rollTimer = 0;
            }

            controller.Move(smoothedPosition - transform.position);
        }

        if (isDashing){
            if (currentLoadDashTime != 0) currentLoadDashTime = 0;

            dashTimer += Time.deltaTime;
            float t = dashTimer / dashDuration;

            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, dashDestination, ref currentDashPosition, 0.1f);

            Invoke("WaitForDashReset", 0.4f);

            controller.Move(smoothedPosition - transform.position);
        }

        if(loadDash && currentLoadDashTime < maxLoadDashTime && !isDashing && canLoadDash) {
            currentLoadDashTime += Time.deltaTime;

            dashDestination = transform.position + cameraHolder.transform.forward * 15;

            if (spawnedDashParticle == null) spawnedDashParticle = Instantiate(dashParticle, dashDestination, transform.rotation);

            spawnedDashParticle.transform.position = dashDestination;

            if (gravity != -1f) gravity = -1f;
            if (isRolling) isRolling = false;
        }

        wallRight = Physics.Raycast(_armature.transform.position, _armature.transform.right, out rightWallHit, 1, whatIsWall);
        wallLeft = Physics.Raycast(_armature.transform.position, -_armature.transform.right, out leftWallHit, 1, whatIsWall);

        if ((wallLeft || wallRight)  && !controller.isGrounded && isSprinting && currentWallRunTime <= maxWallRunTime){
            if (!canWallRun) canWallRun = true;
            if(isJumping) isJumping = false;
        }
        else if(!(wallLeft || wallRight)){
            if (canWallRun) canWallRun = false;
        }
        else{
            if (canWallRun) canWallRun = false;
            if (isWallRunning) isWallRunning = false;       
            if (currentWallRunTime >= 0) currentWallRunTime -= Time.deltaTime;
        }

        
        if (Physics.Raycast(_armature.transform.position, _armature.transform.forward, out forwardHit, 2)){
            if (!canWallJump && !needToWaitWallJump) canWallJump = true;
        }
        else{
            if (canWallJump)  canWallJump = false;
        }

    }

    private void FixedUpdate(){
        if (controller.isGrounded && isJumping && verticalVelocity <= 0) isJumping = false;
        if (controller.isGrounded && isJumping && hasRolled) hasRolled = false;
        if (controller.isGrounded && jumpCounter > 0 && verticalVelocity <= 0) jumpCounter = 0;
        if (controller.isGrounded && specialAttack) specialAttack = false;
        if (controller.isGrounded && !isDashing && spawnedDashParticle != null) Destroy(spawnedDashParticle); 
        if (controller.isGrounded && isRolling) isRolling = false;
        if (controller.isGrounded && !canLoadDash) canLoadDash = true;
        if (controller.isGrounded && loadDash) loadDash = false;

        if (loadDash && currentLoadDashTime >= maxLoadDashTime){
            canLoadDash = false;
            loadDash = false;
            if (spawnedDashParticle != null) Destroy(spawnedDashParticle);
            if (currentLoadDashTime != 0) currentLoadDashTime -= Time.deltaTime;
        }

        if (isMoving){
            Vector3 camForward = cameraHolder.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            moveDirection = (camForward * movementInput.y + cameraHolder.transform.right * movementInput.x).normalized * CalculatedSpeed();
            moveDirection.y += verticalVelocity;

            if (!isWallRunning) controller.Move(moveDirection * Time.deltaTime);
        }
        else{
            Vector3 stationaryMovement = new Vector3(0f, verticalVelocity, 0f);
            controller.Move(stationaryMovement * Time.deltaTime);
        }

        if (isWallRunning){
            Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

            currentWallRunTime += Time.deltaTime;

            Vector3 wallForward = Vector3.Cross(wallNormal, _armature.transform.up);
            wallForward.y = 0;
            gravity = 0;

            

            if ((_armature.transform.forward - wallForward).magnitude > (_armature.transform.forward - -wallForward).magnitude){
                wallForward = -wallForward;
            }


            if ((wallLeft && playerInputActions.Player.Movement.controls[3].IsPressed()) || (wallRight && playerInputActions.Player.Movement.controls[2].IsPressed())){   
                isWallRunning = false;
            }

            controller.Move(wallForward * CalculatedSpeed() * Time.deltaTime);
        }

        if (isSliding){
            if (!controller.isGrounded && !isRolling) {
                if (gravity != -4) gravity = -4;
                if (playerInputActions.Player.Jump.IsPressed()) isSliding = false;
            }
            else{
                if (gravity != -9.81) gravity = -9.81f;
            }

            if (currentSlidingTime <= slidingTime){
                currentSlidingTime += Time.deltaTime;
            }
            else{
                if (isSliding) isSliding = false;
            }
        }
        else{
            if (gravity != -9.81) gravity = -9.81f;

            if (currentSlidingTime > 0) {
                currentSlidingTime -= Time.deltaTime;
            }
        }

        verticalVelocity += gravity * Time.deltaTime;

    }
}
