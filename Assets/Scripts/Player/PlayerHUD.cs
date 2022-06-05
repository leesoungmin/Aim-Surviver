using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] WeaponAssaultlife weapon;
    [SerializeField] PlayerStatus status;

    [Header("Weapon Base")]
    [SerializeField] TextMeshProUGUI textWeaponName;
    [SerializeField] Image imageWeaponIcon;
    [SerializeField] Sprite[] spriteWeaponIcons;

    [Header("Ammo")]
    [SerializeField] TextMeshProUGUI textAmmo;

    [Header("Magazine")]
    [SerializeField] GameObject magazineUIPrefab;   //탄창 ui프리팹
    [SerializeField] Transform magazineParent;          //탄창 ui배치되는 panel

    [Header("HP & BloodScreen UI")]
    [SerializeField] TextMeshProUGUI textHP;
    [SerializeField] Image imageBloodScreen;
    [SerializeField] AnimationCurve curveBloodScreen;

     List<GameObject> magazineList;                         //탄창 ui 리스트

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();
        
        //메소드가 등록되어 있는 이벤트 클래스의
        //invoke() 메소드가 호출될 때 등록된 메소드(매게변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.magazineEvent.AddListener(UpdateMagazineUHD);
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    void SetupWeapon()
    {
        textWeaponName.text = weapon.weaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.weaponName];
    }

    void SetupMagazine()
    {

        //weapon에 등록되어있는 최대 탄창 개수만큼 image icon을 생성
        magazineList = new List<GameObject>();
        for(int i =0; i< weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }

        // weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
        for(int i =0; i< weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    void UpdateAmmoHUD(int curAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{curAmmo}/</size>{maxAmmo}";
    }

    void UpdateMagazineUHD(int currentMagazine)
    {
        //전부 비활성화 하고 currentMagazine 개수 만큼 활성화
        for(int i = 0; i<magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        for(int i =0; i<currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;

        //체력이 증가했을때는 화면에 빨간색 이미지를 출력하지 않도록 return
        if (previous <= current) return;

        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
