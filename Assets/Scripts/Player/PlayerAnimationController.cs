using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        //"Player" 오브젝트 기준으로 자식 오브젝트인 
        //"arms_assault_rifle_01"오브젝트에 animator 컴포넌트가 있다
        anim = GetComponentInChildren<Animator>();
    }

    public void OnReload()
    {
        anim.SetTrigger("OnReload");
    }

    public float MoveSpeed
    {
        set => anim.SetFloat("movementSpeed", value);
        get => anim.GetFloat("movementSpeed");
    }

    public bool AimModeIs
    {
        set => anim.SetBool("isAimMode", value);
        get => anim.GetBool("isAimMode");
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        anim.Play(stateName, layer, normalizedTime);
    }

    public bool CurrentAnimationIs(string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}
