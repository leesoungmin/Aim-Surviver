using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public class WeaponAssaultlife : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent magazineEvent = new MagazineEvent();

    [Header("Fire Effect")]
    [SerializeField] GameObject muzzleFlashEffect;

    [Header("Spawn Points")]
    [SerializeField] Transform casingSpawnPoint;
    [SerializeField] Transform bullletSpawnPoint;

    [Header("Audio Clips")]
    [SerializeField] AudioClip audioClipTakeOutWeapon;
    [SerializeField] AudioClip audioClipFire;
    [SerializeField] AudioClip audioReload;

    [Header("Weapon Setting")]
    [SerializeField] WeaponSetting weaponSetting;

    [Header("Aim UI")]
    [SerializeField] Image imageAim;

    float lastAttackTime;
    bool isReload = false;
    bool isAttack = false;                  //공격 여부 체크용
    bool isModeChange = false;        //모드 전환 여부용
    float defaultModeFOV = 60;        //기본 모드에서의 카메라 fov
    float aimModeFOV = 30;            //aim모드에서의 카메라 fov

    AudioSource audioSource;
    PlayerAnimationController animator;
    CasingMemoryPool casingMemoryPool;
    ImpactMemoryPool impactMemoryPool;
    Camera mainCamera;                                  //광선 발사

    public WeaponName weaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimationController>();
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        weaponSetting.currentMagazine = weaponSetting.maxMagazine;

        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon);
        muzzleFlashEffect.SetActive(false);
        magazineEvent.Invoke(weaponSetting.currentMagazine);
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVarialbes();
    }

    void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StartWeaponAction(int type = 0)
    {
        if (isReload == true) return;

        if (isModeChange == true) return;

        if (type == 0)
        {
            if (weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                StartCoroutine("OnAttackLoop");
            }
            else
            {
                OnAttack();
            }
        }
        else if(type == 1)
        {
            if (isAttack == true) return;

            StartCoroutine("OnModeChange");
        }
    }

    public void StopWeaponAction(int type = 0)
    {
        if(type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    public void StartReload()
    {
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        //무기 액션 도중에 'r'키를 눌러 재장전을 시도하면 무기 액션 종료후 실행
        StopWeaponAction();

        StartCoroutine("OnReload");
    }
    IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();

            yield return null;
        }
    }

    IEnumerator OnReload()
    {
        isReload = true;

        animator.OnReload();
        PlaySound(audioReload);

        while (true)
        {
            if(audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                //현재 탄창 수를 1감소 시키고. 바뀐 탄창 정보를 text ui에 업데이트
                weaponSetting.currentMagazine--;
                magazineEvent.Invoke(weaponSetting.currentMagazine);

                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            //뛰고 있을때는 공격 할수 없다
            if(animator.MoveSpeed > 0.5f)
            {
                return;
            }
            //공격주기가 되어야 공격 할수 있도록 하기 위해서 현재 시간 저장
            lastAttackTime = Time.time;

            if(weaponSetting.currentAmmo <= 0)
            {
                return; 
            }

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            if (animator.AimModeIs == false)
                StartCoroutine("OnMuzzleFlashEffect");

            StartCoroutine("OnMuzzleFlashEffect");

            PlaySound(audioClipFire);
            //탄피 생성
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            TwoStepRaycast();
        }
    }

    void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        //화면 중앙 좌표
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);

        //공격 사거리(attackdistance)안에 부딪히는 오브젝트가 있으면 tartgetpoint는 광선에 부딪힌 위치
        if(Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetpoint는 최대 사거리
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        Vector3 attackDirection = (targetPoint - bullletSpawnPoint.position).normalized;
        if(Physics.Raycast(bullletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);
            if(hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if(hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bullletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }

    void ResetVarialbes()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;   
    }

    public void IncreaseMagazine(int magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

        magazineEvent.Invoke(CurrentMagazine);
    }

    IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !animator.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? aimModeFOV : defaultModeFOV;

        isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            //mode에 따라 카메라의 시야각을 변경
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }

    IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }
}
