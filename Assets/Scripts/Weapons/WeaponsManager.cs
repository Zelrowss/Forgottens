using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum WeaponsType{
    FlameThrower,
    Automatic,
    Sniper,
    ShotGun,
    Burst,
    Laser,
    Sword,
}

public enum Element {
Nothing,
Ice,
Fire,
Air,
Earth,
// Fire + Ice
Steam,
// Ice + Earth
Mud,
// Air + Ice
Wind,
// Air + Fire
Blast,
// Fire + Earth
Lava,
// Air + Earth
Corrosive,
// Fire + Ice + Earth
IceLava,
// Air + Ice + Earth
Haze,
// Air + Fire + Ice
Electricity,
// Air + Fire + Earth + Ice
Viral,

}

public class WeaponsManager : MonoBehaviour
{
    class Bullet {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }

    [Header("References")]
    private PlayerController _playerController;
    private WeaponHolder _weaponHolder;
    public Transform rightHandPos;
    public Transform leftHandPos;
    [SerializeField] private GameObject damagePopupGO;
    
    [Header("Particles")]
    public ParticleSystem hitEffect;
    public TrailRenderer bulletTracer;
    public LineRenderer laserTracer;
    public ParticleSystem muzzleFlash;
    public ParticleSystem flameThrowerParticle;

    [Header("Weapons")]
    public WeaponsType currentWeaponType;
    public Element currentElement;
    public Transform raycastOrigin;
    [HideInInspector] public RaycastHit hit;
    private List<Bullet> bullets = new List<Bullet>();

    [Header("Values")]
    public float healthDamage;
    public float shieldDamage;
    public float criticalChance;
    public float elementalChance;
    public float elementalStrength;
    public float elementalDuration = 10;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public float maxLifeTime = 3.0f;
    public float fireRate = 10;

    [Header("Value (bis)")]
    private float criticalMultiplier;
    [HideInInspector] public float finalDamage;
    private bool hasShoot = false;
    private bool isElementaryShot;
    [HideInInspector] public Color elementaryColor;

    [Header("Automatic Values")]  
    private float lastTimeAttack;

    [Header("Burst Values")]
    public float burstCooldown = 1.0f;
    public float bulletPerBurst = 3;
    public float timeBetweenBullets = 0.1f;
    private float bulletsShotInBurst;
    private float lastTimeShot;
    private float lastTimeBurstShot;

    [Header("ShotGun Values")]
    public int shotgunBulletCount = 8;
    private float shotgunSpreadAngle = 10f;

    [Header("Laser Values")]
    private LineRenderer spawnedLaser;
    private float laserCurrentTime;
    public float laserMaxTime;

    
    void Awake(){
        _playerController = GetComponentInParent<PlayerController>();
        _weaponHolder = GetComponentInParent<WeaponHolder>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void Start(){
        hitEffect = _weaponHolder.GetComponentInChildren<ParticleSystem>();
    }

    Vector3 GetPosition(Bullet bullet){
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity){
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(bulletTracer, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);

        return bullet;
    }

    private void UpdateBullet(float deltaTime){
        SimulateBullets(deltaTime);
        DestroyBullet();
    }

    private void SimulateBullets(float deltaTime){
        bullets.ForEach(bullet => {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullet(){
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }

    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet){
        Vector3 direction = end - start;
        float distance = (end - start).magnitude;

        if (Physics.Raycast(start, direction, out hit, distance)){

            hitEffect.transform.position = hit.point;
            hitEffect.transform.forward = hit.normal;
            hitEffect.Emit(1);

            GameObject hitObject = hit.transform.gameObject;       

            if (hitObject.CompareTag("Enemy") || hitObject.CompareTag("DestroyableObject") || hitObject.CompareTag("SabotageObjectif")){
                MakeDamage(hitObject);
            }
            else{
                if (hitObject.CompareTag("Unmovable")) {return;}
                Rigidbody rbObject = hitObject.GetComponent<Rigidbody>();

                if (rbObject != null) rbObject.AddForce(-hit.normal * 5, ForceMode.Impulse);
            }
            
            if (bullet.tracer != null){
                bullet.tracer.transform.position = hit.point;
                bullet.time = maxLifeTime;
            }

        }
        else{
            if (bullet.tracer != null) bullet.tracer.transform.position = end;
        }
        
    }

    private void MakeDamage(GameObject enemy){
        if (enemy.CompareTag("Enemy")){
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            Vector3 popupPos = hit.point + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Quaternion popupRot = Quaternion.LookRotation(-hit.normal);
            Animation procAnimation = enemy.GetComponent<Animation>();
            Material materialAnimationColor = enemy.GetComponentInChildren<SkinnedMeshRenderer>().material;

            if (isElementaryShot && currentElement != Element.Nothing){
                ProcElement proc = new ProcElement();
                ProcElement existingProc = enemyComponent.currentProcs.Find(proc => proc.procElement == currentElement);

                materialAnimationColor.SetColor("_GlowColor", elementaryColor);
                procAnimation.Play();

                if (currentElement == Element.Air){
                    Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

                    if (elementalStrength <= 100) enemyRb.AddForce(-hit.normal * (1.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 200) enemyRb.AddForce(-hit.normal * (2f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 300) enemyRb.AddForce(-hit.normal * (2.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 400) enemyRb.AddForce(-hit.normal * (3f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                }
                else if (currentElement == Element.Wind){
                    Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

                    if (elementalStrength <= 100) enemyRb.AddForce(-hit.normal * (3f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 200) enemyRb.AddForce(-hit.normal * (3.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 300) enemyRb.AddForce(-hit.normal * (4f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                    else if (elementalStrength <= 400) enemyRb.AddForce(-hit.normal * (4.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1))), ForceMode.Impulse);
                }
                else if (currentElement == Element.Corrosive){
                    if (elementalStrength <= 100) shieldDamage *= 1.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 200) shieldDamage *= 2f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 300) shieldDamage *= 2.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 400) shieldDamage *= 3f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                }
                else if (currentElement == Element.Viral){
                    if (elementalStrength <= 100) healthDamage *= 1.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 200) healthDamage *= 2f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 300) healthDamage *= 2.5f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                    else if (elementalStrength <= 400) healthDamage *= 3f + (0.1f * (existingProc != null ? existingProc.procElementsNumbers : 1));
                }
                else if (currentElement == Element.Blast){
                    enemyComponent.shield /= 1.5f;
                }


                for (int i = 0; i < enemyComponent.ElementImages.Length; i++){

                    if (enemyComponent.ElementImages[i].sprite == null){
                        enemyComponent.ElementImages[i].enabled = true;
                        for (int j = 0; j < enemyComponent.ElementsSprite.Length; j++){
                            if (enemyComponent.ElementsSprite[j].name == currentElement.ToString()){
                                enemyComponent.ElementImages[i].sprite = enemyComponent.ElementsSprite[j];
                                break;
                            }
                        }
                        break;
                    }
                    else{
                        if (enemyComponent.ElementImages[i].sprite.name == currentElement.ToString()){
                            break;
                        }
                    }
                }

                if (existingProc == null){
                    proc.procElement = currentElement;
                    proc.procElementsNumbers += 1;
                    proc.lastProcTime = Time.time;
                    enemyComponent.currentProcs.Add(proc);
                }
                else{
                    if (existingProc.procElementsNumbers < 10) existingProc.procElementsNumbers += 1;
                    existingProc.lastProcTime = Time.time;
                }
         

            }

            if (enemyComponent.shield < 0){
                if (criticalMultiplier != 0) finalDamage = healthDamage * criticalMultiplier * (currentElement == enemyComponent.weakness ? 2 : 1);
                else finalDamage = healthDamage * (currentElement == enemyComponent.weakness ? 2 : 1);

                enemyComponent.health -= finalDamage;
                DamagePopup.Create(damagePopupGO, popupPos, popupRot, finalDamage, criticalMultiplier);
            }
            else{   
                if (criticalMultiplier != 0) finalDamage = shieldDamage * criticalMultiplier * (currentElement == enemyComponent.weakness ? 2 : 1);
                else finalDamage = shieldDamage * (currentElement == enemyComponent.weakness ? 2 : 1);

                enemyComponent.shield -= finalDamage;
                DamagePopup.Create(damagePopupGO, popupPos, popupRot, finalDamage, criticalMultiplier);
            }
        }
        else if(enemy.CompareTag("DestroyableObject") || enemy.CompareTag("SabotageObjectif")) {
            Vector3 popupPos = hit.point + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            Quaternion popupRot = Quaternion.LookRotation(-hit.normal);
            DestroyableObjects destroyableObject = enemy.GetComponent<DestroyableObjects>();

            if (destroyableObject.health > 0) {
                finalDamage = criticalMultiplier > 0 ? healthDamage * criticalMultiplier : healthDamage;

                destroyableObject.health -= finalDamage;
                DamagePopup.Create(damagePopupGO, popupPos, popupRot, finalDamage, criticalMultiplier);
            }
        }

    }

    IEnumerator ResetElentaryDamage(){
        if (isElementaryShot){
            yield return new WaitForSeconds(0.1f);
            isElementaryShot = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ResetElentaryDamage());

        if (_playerController.isAiming && _weaponHolder.currentWeaponPos != null || _playerController.isAttacking && _weaponHolder.currentWeaponPos != null){
            Vector3 targetRotation = new Vector3(_playerController.cameraHolder.transform.rotation.eulerAngles.x, transform.rotation.y, _weaponHolder.currentWeaponPos.transform.rotation.z);
            _weaponHolder.currentWeaponPos.transform.localRotation = Quaternion.Euler(targetRotation);
        }

        UpdateBullet(Time.deltaTime);
           
        if (_playerController.isAttacking){

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            RaycastHit hit;

            

            if (criticalChance <= 100){
                if (Random.Range(0, 101) < criticalChance){
                    criticalMultiplier = 1.5f;
                }
                else{
                    criticalMultiplier = 0f;
                }
            }
            else if (criticalChance <= 200){
                if (Random.Range(0, 101) < criticalChance - 100){
                    criticalMultiplier = 3;
                }
                else{
                    criticalMultiplier = 1.5f;
                }
            }
            else if (criticalChance <= 300){
                if (Random.Range(0, 101) < criticalChance - 200){
                    criticalMultiplier = 6;
                }
                else {
                    criticalMultiplier = 3f;
                }
            }
            else if (criticalChance <= 400){
                if (Random.Range(0, 101) < criticalChance - 300){
                    criticalMultiplier = 12f;
                }
                else{
                    criticalMultiplier = 6f;
                }
            }

            if (elementalChance <= 100){
                if (Random.Range(0, 101) < elementalChance){
                    if (Random.Range(0, 101) < 75) isElementaryShot = true;
                }
            }
            else if (elementalChance <= 200){
                if (Random.Range(0, 101) < elementalChance - 100) {
                    if (Random.Range(0, 101) < 50) isElementaryShot = true;
                }
            }
            else if(elementalChance <= 300){
                if (Random.Range(0, 101) < elementalChance - 200){
                    if (Random.Range(0, 101) < 25) isElementaryShot = true;
                }
            }
            else if (elementalChance <= 400){
                if (Random.Range(0, 101) < elementalChance - 300){
                    isElementaryShot = true;
                }
            }


            if (currentWeaponType == WeaponsType.Automatic){
                if (Time.time - lastTimeAttack >= 1f / fireRate){

                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 velocity = (hit.point - raycastOrigin.position).normalized * bulletSpeed;
                        var bullet = CreateBullet(raycastOrigin.position, velocity);
                        bullets.Add(bullet);

                        muzzleFlash.Emit(1);

                        lastTimeAttack = Time.time;
                    }
                }
            }
            else if (currentWeaponType == WeaponsType.Burst){
                if (Time.time >= lastTimeBurstShot + burstCooldown){
                    if (bulletsShotInBurst == 0 || Time.time - lastTimeShot >= timeBetweenBullets){
                        if (bulletsShotInBurst == 0){
                            bulletsShotInBurst = bulletPerBurst;
                        }

                        if (Physics.Raycast(ray, out hit))
                        {
                            Vector3 velocity = (hit.point - raycastOrigin.position).normalized * bulletSpeed;
                            var bullet = CreateBullet(raycastOrigin.position, velocity);
                            bullets.Add(bullet);

                            muzzleFlash.Emit(1);

                            lastTimeShot = Time.time;
                            bulletsShotInBurst--;

                            if (bulletsShotInBurst == 0)
                            {
                                lastTimeBurstShot = Time.time;
                            }
                        }

                        
                    }
                }
            }
            else if (currentWeaponType == WeaponsType.ShotGun){
                if (Time.time - lastTimeAttack >= 1f / fireRate && !hasShoot){
                    for (int i = 0; i < shotgunBulletCount; i++){


                        if (Physics.Raycast(ray, out hit))
                        {
                            Vector3 spreadDirection = GetSreadDirection(hit.point - raycastOrigin.position, shotgunSpreadAngle);
                            Vector3 velocity = spreadDirection.normalized * bulletSpeed;
                            var bullet = CreateBullet(raycastOrigin.position, velocity);
                            bullets.Add(bullet);
                        }
                            
                    }

                    hasShoot = true;

                    muzzleFlash.Emit(1);

                    lastTimeAttack = Time.time;
                }
            }
            else if (currentWeaponType == WeaponsType.Sniper){
                if (Time.time - lastTimeAttack >= 1f && !hasShoot){


                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 velocity = (hit.point - raycastOrigin.position).normalized * bulletSpeed;
                        var bullet = CreateBullet(raycastOrigin.position, velocity);
                        bullets.Add(bullet);

                        muzzleFlash.Emit(1);

                        hasShoot = true;

                        lastTimeAttack = Time.time;
                    }

                }
            }
            else if (currentWeaponType == WeaponsType.FlameThrower){
                if (Time.time - lastTimeAttack >= 1f / fireRate){

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Physics.Raycast(raycastOrigin.position, hit.point - raycastOrigin.position, out hit, maxLifeTime * 2))
                        {
                            MakeDamage(hit.transform.gameObject);
                        }

                        flameThrowerParticle.startLifetime = maxLifeTime / 4;
                        flameThrowerParticle.Emit(1);

                        lastTimeAttack = Time.time;
                    }
                }
            }
            else if (currentWeaponType == WeaponsType.Laser){
                if (_playerController.isAiming && laserCurrentTime <= laserMaxTime && !hasShoot){
                    laserCurrentTime += Time.deltaTime;
                    if (spawnedLaser == null){
                        spawnedLaser = Instantiate(laserTracer, raycastOrigin.position, Quaternion.identity);
                    }

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (Physics.Raycast(raycastOrigin.position, hit.point - raycastOrigin.position, out hit, 999))
                        {
                            MakeDamage(hit.transform.gameObject);
                        }

                        spawnedLaser.SetPosition(0, raycastOrigin.position);
                        spawnedLaser.SetPosition(1, hit.point);
                    }
                }
                else{
                    if (spawnedLaser!= null) Destroy(spawnedLaser);
                    if (!hasShoot) hasShoot = true;
                    StartCoroutine(WaitForShootAgain());
                }
            }


        }
        else{
            if (hasShoot) hasShoot = false;
            if (spawnedLaser!= null) Destroy(spawnedLaser);
        }

    }

    IEnumerator WaitForShootAgain(){
        yield return new WaitForSeconds(1f);
        laserCurrentTime = 0;
    }

    private Vector3 GetSreadDirection(Vector3 direction, float angle){
        Vector3 spreadDirection = Quaternion.Euler(Random.Range(-angle, angle), Random.Range(-angle, angle), 0) * direction;
        return spreadDirection;
    }

}
