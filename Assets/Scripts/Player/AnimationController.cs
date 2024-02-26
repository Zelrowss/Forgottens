using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(RigBuilder))]
[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
 
    [Header("References")]
    private PlayerController _playerController;
    private PlayerManager _playerManager;
    private Animator anim;
    
    [Header("Rig Layers")]
    private RigBuilder _rigBuilder;
    public Rig rifleAimLayer;
    public Rig pistolAimLayer;
    public MultiRotationConstraint swordLayer;

    [Header("Animator Values")]
    public float smoothTime = 0.3f;
    private float currentLayerValue;


    private void Awake(){
        _playerController = GetComponentInParent<PlayerController>();
        _playerManager = GetComponentInParent<PlayerManager>();
        anim = GetComponent<Animator>();
        _rigBuilder = GetComponent<RigBuilder>();
    }


    private void Update(){

        if (!Mathf.Approximately(anim.GetFloat("SprintValue"), _playerController.sprintInput)){
            anim.SetFloat("SprintValue", _playerController.sprintInput, 0.1f, Time.deltaTime);
        }

        if (anim.GetFloat("wallRunDirection") != (_playerController.wallLeft ? -1 : 1)){
            anim.SetFloat("wallRunDirection", _playerController.wallLeft ? -1 : 1);
        }

        if (anim.GetFloat("attackNumber") != _playerController.attackNumber){
            anim.SetFloat("attackNumber", _playerController.attackNumber);
        }  

        if (anim.GetBool("isMoving") != _playerController.isMoving) anim.SetBool("isMoving", _playerController.isMoving);
        if (anim.GetBool("isJumping") != _playerController.isJumping) anim.SetBool("isJumping", _playerController.isJumping);
        if (anim.GetBool("isWallJumping") != (_playerController.isJumping && _playerController.canWallJump && (_playerController.jumpCounter > 1))) anim.SetBool("isWallJumping", (_playerController.isJumping && _playerController.canWallJump && (_playerController.jumpCounter > 1)));
        if (anim.GetBool("isCrouched") != _playerController.isCrouching) anim.SetBool("isCrouched", _playerController.isCrouching);
        if (anim.GetBool("isSliding") != _playerController.isSliding) anim.SetBool("isSliding", _playerController.isSliding);
        if (anim.GetBool("isRolling") != _playerController.isRolling) anim.SetBool("isRolling", _playerController.isRolling);
        if (anim.GetBool("isFalling") != (!_playerController.controller.isGrounded && !_playerController.isJumping)) anim.SetBool("isFalling", !_playerController.controller.isGrounded && !_playerController.isJumping && !_playerController.isWallRunning);
        if (anim.GetBool("isWallRuning") != _playerController.isWallRunning) anim.SetBool("isWallRuning", _playerController.isWallRunning);
        if (anim.GetBool("isAttacking") != _playerController.isAttacking) anim.SetBool("isAttacking", _playerController.isAttacking);
        if (anim.GetBool("isSpecialAttacking") != _playerController.specialAttack) anim.SetBool("isSpecialAttacking", _playerController.specialAttack);
        
        if (anim.GetBool("isBlocking") != _playerController.isAiming) anim.SetBool("isBlocking", _playerController.isAiming);

        if (_playerManager.isInMission){
            if (_playerManager.currentWeaponsPlace == Weapons.Primary){
                if (anim.GetLayerWeight(anim.GetLayerIndex("Sword Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Sword Movement"), 0f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Pistol Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Pistol Movement"), 0f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement")) != 1) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement"), 1f);

                for (int i = 1; i < _rigBuilder.layers.Count - 1; i++){
                    if (i < 3) _rigBuilder.layers[i].active = true;
                    else _rigBuilder.layers[i].active = false;
                }

                if(_playerController.isAiming || _playerController.isAttacking){
                    if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement")) != 1) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement"), 1f);   
                    rifleAimLayer.weight += Time.deltaTime / smoothTime;
                }
                else{
                    if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement"), 0f);
                    rifleAimLayer.weight -= Time.deltaTime / smoothTime;
                }
            }
            else if(_playerManager.currentWeaponsPlace == Weapons.Secondary){
                if (anim.GetLayerWeight(anim.GetLayerIndex("Sword Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Sword Movement"), 0f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Pistol Movement")) != 1) anim.SetLayerWeight(anim.GetLayerIndex("Pistol Movement"), 1f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement"), 0f); 
                if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement"), 0f);

                for (int i = 1; i < _rigBuilder.layers.Count - 1; i++){
                    if (i == 3 || i == 4) _rigBuilder.layers[i].active = true;
                    else _rigBuilder.layers[i].active = false;
                }

                if (_playerController.isAiming || _playerController.isAttacking){
                    pistolAimLayer.weight += Time.deltaTime / smoothTime;
                }  
                else{
                    pistolAimLayer.weight -= Time.deltaTime;
                }
            }
            else if (_playerManager.currentWeaponsPlace == Weapons.Melee){
                if (anim.GetLayerWeight(anim.GetLayerIndex("Sword Movement")) != 1) anim.SetLayerWeight(anim.GetLayerIndex("Sword Movement"), 1f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Pistol Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Pistol Movement"), 0f);
                if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Normal Movement"), 0f); 
                if (anim.GetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement")) != 0) anim.SetLayerWeight(anim.GetLayerIndex("Rifle Aiming Movement"), 0f);

                swordLayer.data.constrainedObject = _playerManager.meleeWeaponObject.transform;

                for (int i = 1; i < _rigBuilder.layers.Count - 1; i++){
                    if (i == 6) _rigBuilder.layers[i].active = true;
                    else _rigBuilder.layers[i].active = false;
                } 
            }

            if (!_playerController.controller.isGrounded && _playerController.specialAttack){
                AnimatorStateInfo stateInfo =  anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Sword Movement"));
                if (stateInfo.IsName("Sword@Ground Attack") && stateInfo.normalizedTime > 0.48f){
                    if (anim.speed != 0) anim.speed = 0;
                }
            }
            else{
                if (anim.speed != 1) anim.speed = 1;
            }
        }



    }

}
