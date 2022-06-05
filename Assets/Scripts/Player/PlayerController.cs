using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCode")]
    [SerializeField] KeyCode keyCodeRun = KeyCode.LeftShift;    //달리기 키
    [SerializeField] KeyCode keyCodeJump = KeyCode.Space;    //점프 키
    [SerializeField] KeyCode keyCodeReload = KeyCode.R;

    [Header("Audio Clips")]
    [SerializeField] AudioClip walkClip;
    [SerializeField] AudioClip runClip;

    [Header("GameOver")]
    [SerializeField] GameObject GameOverPanel;

    [Header("TimeOver")]
    [SerializeField] GameObject TimeOverPanel;

    RotateToMouse rotMouse;
    PlayerMovement playerMovement;
    PlayerStatus playerStatus;
    PlayerAnimationController animator;
    AudioSource audioSource;
    CharacterController characterController;
    WeaponAssaultlife weapon;

    [SerializeField] Text txt_Timer;

    bool isCursor = false;
    float time = 181;
    float selectCountdown;

    private void Awake()
    {
        //마우스 커서 보이지 않게 설정, 현재 위치 고정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        selectCountdown = time;

        rotMouse = GetComponent<RotateToMouse>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<PlayerStatus>();
        animator = GetComponent<PlayerAnimationController>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        weapon = GetComponentInChildren<WeaponAssaultlife>();
    }

    private void Update()
    {
        
        if(isCursor == false)
        {
            UpdateWeaponAction();
            UpdateRotate();
            UpdateMove();
            UpdateJump();

            Timer();

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("TitleScene");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    void Timer()
    {
         if(isCursor == false)
        {
            selectCountdown -= Time.deltaTime;
            txt_Timer.text = Mathf.Floor(selectCountdown).ToString();
            if(selectCountdown <= 1)
            {
                TimeOver();
            }
        }
    }

    void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotMouse.UpdateRotate(mouseX, mouseY);
    }

    void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if(x != 0 || z != 0)
        {
            bool isRun = false;

            //옆이나 뒤로 이동할 떄는 달릴수 없다.
            if (z > 0) isRun = Input.GetKey(keyCodeRun);    //옆이나 뒤로 이동할때 쉬프트키를 누르면 isRun이 true가된다

            //isRun이 false WlakSpeed , true RunSpeed
            playerMovement.MoveSpeed = isRun == true ? playerStatus.RunSpeed : playerStatus.WalkSpeed;
            animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = isRun == true ? runClip : walkClip;

            //재생중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if(audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }

            if(!characterController.isGrounded)
            {
                audioSource.Stop();
            }
            
        }
        else
        {
            //제자리에 멈춰있을 때
            playerMovement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            if(audioSource.isPlaying ==true)
            {
                audioSource.Stop();
            }
        }

        playerMovement.MoveTo(new Vector3(x, 0, z));
    }

    void UpdateJump()
    {
        if(Input.GetKeyDown(keyCodeJump))
        {
            playerMovement.Jump();
        }
    }

    void UpdateWeaponAction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            weapon.StartWeaponAction();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            weapon.StopWeaponAction();
        }

        if(Input.GetMouseButton(1))
        {
            weapon.StartWeaponAction(1);
        }
        else if(Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
        }

        if(Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }

        
    }

    public void TakeDamage(int damage)
    {
        bool isDie = playerStatus.DecreaseHP(damage);

        if(isDie == true)
        {
            Debug.Log("GameOver");
            GameOver();
        }
    }

    void GameOver()
    {
        isCursor = true;
        GameOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void TimeOver()
    {
        isCursor = true;
        TimeOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void ReplayButton()
    {
        SceneManager.LoadScene("IngameScene");
    }
}
