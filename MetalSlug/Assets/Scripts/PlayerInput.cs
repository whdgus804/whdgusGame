using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerInputAction action;

    /// <summary>
    /// 플레이어 대기 애니메이션이 재생후 다음 대기 애니메이션이 나오기까지의 시간 해당 시간에 playWaitAnimationTime을 더한다
    /// </summary>
    public float nextWaitAnimTime = 7.0f;
    /// <summary>
    /// 플레이어의 통상 이동속도
    /// </summary>
    public float idleMoveSpeed;
    /// <summary>
    /// 앉은 상태에서의 이동속도
    /// </summary>
    public float sitMoveSpeed;
    /// <summary>
    /// 플레이어의 점프높이를 조절하는 변수
    /// </summary>
    public float jumpPower;
    /// <summary>
    /// 플레이어가 떨어지는 도중 좌우로 이동할때 이동속도
    /// </summary>
    public float fallingMoveSpeed;
    /// <summary>
    /// 저장된 초만큼 기다리면 대기애니메이션 재생
    /// </summary>
    public float playWaitAnimationTime = 7.0f;
    /// <summary>
    /// 플레이어의 상반신 애니메이터
    /// </summary>
    Animator upperanimator;
    /// <summary>
    /// 플레이어의 전신, 하반신 애니메이터
    /// </summary>
    Animator lowerAnimator;
    Rigidbody2D rigid;

    SpriteRenderer upperBodySprite;
    /// <summary>
    /// 플레이어 이동방향
    /// </summary>
    Vector3 inputDirection = Vector3.zero;
    public Transform attackRange;
    /// <summary>
    /// 플레이어가 사격시 1씩 증가
    /// </summary>
    int hGAmmo = 0;
    /// <summary>
    /// 근접 공격 모션 선택을 위한 변수
    /// </summary>
    int attackCount = 0;
    /// <summary>
    /// 플레이어 지상이동 애니메이션
    /// </summary>
    readonly int move_To_Hash = Animator.StringToHash("Move");
    /// <summary>
    /// 점프애니메이션
    /// </summary>
    readonly int jump_To_Hash = Animator.StringToHash("Jump");
    /// <summary>
    /// 앉기 애니메이션
    /// </summary>
    readonly int Sit_To_Hash = Animator.StringToHash("Sit");
    /// <summary>
    /// 총을 발사하는 애니메이션
    /// </summary>
    readonly int fire_To_Hash = Animator.StringToHash("Fire");
    /// <summary>
    /// 전신 애니메이션 재생중 상체 애니메이션이 나와야할때 전신애니메이션을 바로 끄게 해주는 변수
    /// </summary>
    readonly int exitAnim_To_Hash = Animator.StringToHash("Exit");
    /// <summary>
    /// 재장선 애니메이션
    /// </summary>
    readonly int reloadAnim_To_Hash = Animator.StringToHash("Reload");
    /// <summary>
    /// 플레이어 대기 애니메이션
    /// </summary>
    readonly int waitAnim_To_Hash = Animator.StringToHash("Wait");
    /// <summary>
    /// 플레이어근접공격 애니메이션
    /// </summary>
    readonly int attack_To_Hash = Animator.StringToHash("Attack");
    readonly int attackTrigger_To_Hash = Animator.StringToHash("AttackTrigger");
    /// <summary>
    /// 플레이어가 위를 쳐다보는 애니메이션
    /// </summary>
    readonly int lookUp_To_Hash = Animator.StringToHash("LookUp");
    /// <summary>
    /// 현제 플레이어에게 적용된 이동속도
    /// </summary>
    [HideInInspector]
    public float nowMoveSpeed=0.0f;
    /// <summary>
    /// 대기 애니메이션이 실행되는 조건변수
    /// </summary>
    public float waitTime = 0.0f;
    /// <summary>
    /// 이동입력을 받으면 저장될 변수
    /// </summary>
    float sight=0.0f;
    /// <summary>
    /// 현재 플레이어가 땅위에 있는지 나타내는 변수 true면 땅위
    /// </summary>
    bool isGrouded=false;
    /// <summary>
    /// 플레이어가 앉은 신호를 받았는지 나타내는 변수 false면 받지않음
    /// </summary>
    bool isSitting = false;
    /// <summary>
    /// 플레이어가 space를 눌렀는 지 확인하는 변수 땅에 닿으면 false 플레이어가 점프버튼을 누르지 않고 떨어지는 곳에서 낙하할때 애니메이션 재생을 위함
    /// </summary>
    bool jumpInput = false;
    /// <summary>
    /// 플레이어가 위를 쳐다보고 있는지 나타내는 변수 true 면 위를 쳐다보고 있는중 
    /// </summary>
    bool lookUp = false;

    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position, attackRange.position);
    //}

    private void Awake()
    {
        action=new PlayerInputAction();
        nowMoveSpeed = idleMoveSpeed;
        upperanimator=transform.GetChild(0).GetComponent<Animator>();
        upperBodySprite=transform.GetChild(0).GetComponent<SpriteRenderer>();
        lowerAnimator=transform.GetChild(1).GetComponent<Animator>();
        rigid=GetComponent<Rigidbody2D>();
        waitTime = playWaitAnimationTime;
        
    }

    private void OnEnable()
    {
        action.Enable();
        action.Player.Move.performed += OnMove;
        action.Player.Move.canceled += OnMove;
        action.Player.Jump.performed += OnJump;
        action.Player.Down.performed += GetDown;
        action.Player.Down.canceled += GetUp;
        action.Player.Fire.performed += OnFire;
        action.Player.LookUp.performed += LookUp;
        action.Player.LookUp.canceled += LookUp;
    }


    private void OnDisable()
    {
        action.Player.LookUp.canceled -= LookUp;
        action.Player.LookUp.performed -= LookUp;
        action.Player.Fire.performed -= OnFire;
        action.Player.Down.canceled -= GetUp;
        action.Player.Down.performed -= GetDown;
        action.Player.Jump.performed -= OnJump;
        action.Player.Move.canceled -= OnMove;
        action.Player.Move.performed -= OnMove;
        action.Disable();
    }
    private void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime < 0)
        {
            OnWaitAnim();
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(inputDirection*nowMoveSpeed * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //플레이어가 지면에 착지 할 때
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpInput = false;
            isGrouded = true;
            nowMoveSpeed = idleMoveSpeed;
            upperanimator.SetBool(jump_To_Hash, false);
            lowerAnimator.SetBool(jump_To_Hash, false);
            Sight();
            MoveAnim();
            //플레이어가 아레 버튼을 누르고 있을 시 숙이기 재생
            if (isSitting)
                GetDown();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrouded = false;
            lowerAnimator.SetBool(Sit_To_Hash, false);
            Fly();
            LowerBodyAnimExit();
        }
    }
    //----------------------------------------------------------------------------------------이동관련
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input= context.ReadValue<Vector2>();
        sight = input.x;
        inputDirection = (Vector3)input;
        Sight();
        MoveAnim();
    }
    /// <summary>
    /// 플레이어가 이동했을 때 바라보는 방향을 교체하는 함수
    /// </summary>
    private void Sight()
    {
        if (isGrouded)
        {
            if (sight < 0.0f)
            {
                transform.localScale=new Vector3(-1,1,1);
                
                
            }else if(sight > 0.0f)
            {
                transform.localScale=new Vector3(1,1,1);
            }
        }
    }
    /// <summary>
    /// 플레이어의 지상이동 애니메이션
    /// </summary>
    private void MoveAnim()
    {
        float num = sight;
        if(num<0.0f)
            num = num * -1;
        upperanimator.SetFloat(move_To_Hash, num);
        lowerAnimator.SetFloat(move_To_Hash, num);

        //대기 시간 초기화
        OnWaitAnim();
    }
    /// <summary>
    /// 플레이어의 대기 애니메이션을 실행시키는 함수 혹은 특정 행동을 하여 대기시간을 초기화시킬때 사용
    /// </summary>
    private void OnWaitAnim()
    {
        if(waitTime < 0)
        {
            upperanimator.SetBool(waitAnim_To_Hash, true);
            waitTime = nextWaitAnimTime;
            StartCoroutine(WaitAnimationTime());
        }
        else
        {
            upperanimator.SetBool(waitAnim_To_Hash, false);
            waitTime = nextWaitAnimTime;
        }
    }
    IEnumerator WaitAnimationTime()
    {
        yield return new WaitForSeconds(0.5f);
        upperanimator.SetBool(waitAnim_To_Hash, false);
    }
    //---------------------------------------------------------------------------------------점프관련
    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrouded)
        {
            jumpInput = true;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
    private void Fly()
    {
        //플레이어가 떨어질때
        if (!jumpInput || sight < 0.1f)
        {
            nowMoveSpeed=fallingMoveSpeed;
        }
        if (isSitting)
        {
            upperanimator.SetBool(jump_To_Hash, true);
            lowerAnimator.SetBool(jump_To_Hash, true);
            LookDown();
        }
        else
        {
            upperanimator.SetBool(jump_To_Hash, true);
            lowerAnimator.SetBool(jump_To_Hash, true);
        }
    }

    //-------------------------------------------------------------------------------------하단 관련
    private void GetDown(InputAction.CallbackContext context)
    {
        GetDown();
        isSitting = true;
        LookDown();
    }
    private void GetDown()
    {
        if (isGrouded)
        {
            StartCoroutine(Sitting());
            lowerAnimator.SetBool(Sit_To_Hash, true);
        }

    }
    private void GetUp(InputAction.CallbackContext context)
    {
        isSitting = false;
        
        nowMoveSpeed = idleMoveSpeed;
        LookDown();
        if(!upperBodySprite.enabled)
            LowerBodyAnimExit();
        //upperBodySprite.enabled=true;
        lowerAnimator.SetBool(Sit_To_Hash, isSitting);
    }
    /// <summary>
    /// 플레이어가 공중에서 하단을 보거나 취소할때 실행될함수
    /// </summary>
    private void LookDown()
    {
        upperanimator.SetBool(Sit_To_Hash, isSitting);
    }
    IEnumerator Sitting()
    {
        nowMoveSpeed = 0.0f;
        upperBodySprite.enabled = false;
        yield return new WaitForSeconds(0.25f);
        nowMoveSpeed=sitMoveSpeed;
    }
    //-----------------------------------------------------------------------상단 바라보기
    private void LookUp(InputAction.CallbackContext context)
    {
        lookUp=!lookUp;

        upperanimator.SetBool(lookUp_To_Hash, lookUp);  
        //통상-> 위 바라보기
        //앉은상태->서있는상태->위 바라보기
        //공중-> 위바라보기
    }

    //------------------------------------------------------------------------발사
    private void OnFire(InputAction.CallbackContext context)
    {
        if (!upperBodySprite.enabled)
            upperBodySprite.enabled = true;
        Fire();
        OnWaitAnim();
    }
    private void Fire()
    {
        //만약 가까이에 적이 있다면
        if (LayShoot())
        {
            if (isSitting)
            {

            }
            else
            {
                upperanimator.SetTrigger(attackTrigger_To_Hash);
                upperanimator.SetInteger(attack_To_Hash, attackCount);
                attackCount++;
                if (attackCount>1)
                    attackCount = 0;
            }
        }
        else
        {
            //적이 가까이 없고 앉아 있는 상태면
            if (isSitting)
            {
                //공중일때 
                if (!isGrouded)
                {
                    upperanimator.SetTrigger(fire_To_Hash);
                }
                else//지상일때 앉아서 발사
                {
                    upperBodySprite.enabled = false;
                    lowerAnimator.SetTrigger(fire_To_Hash);
                    StopCoroutine(WaitSitShoot());
                    StartCoroutine(WaitSitShoot());
                }
            }
            else//일어서있는 상태일때
            {
                LowerBodyAnimExit();    //하체 애니메이션을 기본으로 변경
                upperanimator.SetTrigger(fire_To_Hash); //상체 애니메이션 발사재생
            }
            hGAmmo++;           //총알 사용 수 증가
            //StartCoroutine(ExitTrigger());
            if (hGAmmo > 6 && upperBodySprite.enabled)
            {
                //플레이어가 뒤돌면 중지

                upperanimator.SetTrigger(reloadAnim_To_Hash);
                hGAmmo = 0;
            }
        }
    }
    /// <summary>
    /// 앞에 짧은 범위의 레이를 쏴 적이 있는지 확한한다 true면 적이 있다
    /// </summary>
    /// <returns></returns>
    bool LayShoot()
    {
        bool result = false;
        Vector2 direction = attackRange.position - transform.position;

        int enemyLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D raycastHit2D=Physics2D.Raycast(transform.position,direction,1,enemyLayer);
        Debug.DrawRay(transform.position, direction);
        if (raycastHit2D.collider != null)
        {
            result= true;
        }

        return result;
    }
    
    /// <summary>
    /// 하체 애니메이션 중 EXit 트리거를 작동시키는 함수
    /// </summary>
    private void LowerBodyAnimExit()
    {
        lowerAnimator.SetTrigger(exitAnim_To_Hash);
        if(!upperBodySprite.enabled)
            upperBodySprite.enabled = true;
        StartCoroutine(ExitTrigger());
    }

    IEnumerator ExitTrigger()
    {
        yield return new WaitForSeconds(0.1f);
        lowerAnimator.ResetTrigger(exitAnim_To_Hash);
    }

    IEnumerator WaitSitShoot()
    {
        nowMoveSpeed= 0.0f;
        yield return new WaitForSeconds(0.6f);
        nowMoveSpeed = sitMoveSpeed;
    }


}
