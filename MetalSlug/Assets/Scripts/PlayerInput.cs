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
    /// �÷��̾� ��� �ִϸ��̼��� ����� ���� ��� �ִϸ��̼��� ����������� �ð� �ش� �ð��� playWaitAnimationTime�� ���Ѵ�
    /// </summary>
    public float nextWaitAnimTime = 7.0f;
    /// <summary>
    /// �÷��̾��� ��� �̵��ӵ�
    /// </summary>
    public float idleMoveSpeed;
    /// <summary>
    /// ���� ���¿����� �̵��ӵ�
    /// </summary>
    public float sitMoveSpeed;
    /// <summary>
    /// �÷��̾��� �������̸� �����ϴ� ����
    /// </summary>
    public float jumpPower;
    /// <summary>
    /// �÷��̾ �������� ���� �¿�� �̵��Ҷ� �̵��ӵ�
    /// </summary>
    public float fallingMoveSpeed;
    /// <summary>
    /// ����� �ʸ�ŭ ��ٸ��� ���ִϸ��̼� ���
    /// </summary>
    public float playWaitAnimationTime = 7.0f;
    /// <summary>
    /// �÷��̾��� ��ݽ� �ִϸ�����
    /// </summary>
    Animator upperanimator;
    /// <summary>
    /// �÷��̾��� ����, �Ϲݽ� �ִϸ�����
    /// </summary>
    Animator lowerAnimator;
    Rigidbody2D rigid;

    SpriteRenderer upperBodySprite;
    /// <summary>
    /// �÷��̾� �̵�����
    /// </summary>
    Vector3 inputDirection = Vector3.zero;
    public Transform attackRange;
    /// <summary>
    /// �÷��̾ ��ݽ� 1�� ����
    /// </summary>
    int hGAmmo = 0;
    /// <summary>
    /// ���� ���� ��� ������ ���� ����
    /// </summary>
    int attackCount = 0;
    /// <summary>
    /// �÷��̾� �����̵� �ִϸ��̼�
    /// </summary>
    readonly int move_To_Hash = Animator.StringToHash("Move");
    /// <summary>
    /// �����ִϸ��̼�
    /// </summary>
    readonly int jump_To_Hash = Animator.StringToHash("Jump");
    /// <summary>
    /// �ɱ� �ִϸ��̼�
    /// </summary>
    readonly int Sit_To_Hash = Animator.StringToHash("Sit");
    /// <summary>
    /// ���� �߻��ϴ� �ִϸ��̼�
    /// </summary>
    readonly int fire_To_Hash = Animator.StringToHash("Fire");
    /// <summary>
    /// ���� �ִϸ��̼� ����� ��ü �ִϸ��̼��� ���;��Ҷ� ���žִϸ��̼��� �ٷ� ���� ���ִ� ����
    /// </summary>
    readonly int exitAnim_To_Hash = Animator.StringToHash("Exit");
    /// <summary>
    /// ���弱 �ִϸ��̼�
    /// </summary>
    readonly int reloadAnim_To_Hash = Animator.StringToHash("Reload");
    /// <summary>
    /// �÷��̾� ��� �ִϸ��̼�
    /// </summary>
    readonly int waitAnim_To_Hash = Animator.StringToHash("Wait");
    /// <summary>
    /// �÷��̾�������� �ִϸ��̼�
    /// </summary>
    readonly int attack_To_Hash = Animator.StringToHash("Attack");
    readonly int attackTrigger_To_Hash = Animator.StringToHash("AttackTrigger");
    /// <summary>
    /// �÷��̾ ���� �Ĵٺ��� �ִϸ��̼�
    /// </summary>
    readonly int lookUp_To_Hash = Animator.StringToHash("LookUp");
    /// <summary>
    /// ���� �÷��̾�� ����� �̵��ӵ�
    /// </summary>
    [HideInInspector]
    public float nowMoveSpeed=0.0f;
    /// <summary>
    /// ��� �ִϸ��̼��� ����Ǵ� ���Ǻ���
    /// </summary>
    public float waitTime = 0.0f;
    /// <summary>
    /// �̵��Է��� ������ ����� ����
    /// </summary>
    float sight=0.0f;
    /// <summary>
    /// ���� �÷��̾ ������ �ִ��� ��Ÿ���� ���� true�� ����
    /// </summary>
    bool isGrouded=false;
    /// <summary>
    /// �÷��̾ ���� ��ȣ�� �޾Ҵ��� ��Ÿ���� ���� false�� ��������
    /// </summary>
    bool isSitting = false;
    /// <summary>
    /// �÷��̾ space�� ������ �� Ȯ���ϴ� ���� ���� ������ false �÷��̾ ������ư�� ������ �ʰ� �������� ������ �����Ҷ� �ִϸ��̼� ����� ����
    /// </summary>
    bool jumpInput = false;
    /// <summary>
    /// �÷��̾ ���� �Ĵٺ��� �ִ��� ��Ÿ���� ���� true �� ���� �Ĵٺ��� �ִ��� 
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
        //�÷��̾ ���鿡 ���� �� ��
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpInput = false;
            isGrouded = true;
            nowMoveSpeed = idleMoveSpeed;
            upperanimator.SetBool(jump_To_Hash, false);
            lowerAnimator.SetBool(jump_To_Hash, false);
            Sight();
            MoveAnim();
            //�÷��̾ �Ʒ� ��ư�� ������ ���� �� ���̱� ���
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
    //----------------------------------------------------------------------------------------�̵�����
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input= context.ReadValue<Vector2>();
        sight = input.x;
        inputDirection = (Vector3)input;
        Sight();
        MoveAnim();
    }
    /// <summary>
    /// �÷��̾ �̵����� �� �ٶ󺸴� ������ ��ü�ϴ� �Լ�
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
    /// �÷��̾��� �����̵� �ִϸ��̼�
    /// </summary>
    private void MoveAnim()
    {
        float num = sight;
        if(num<0.0f)
            num = num * -1;
        upperanimator.SetFloat(move_To_Hash, num);
        lowerAnimator.SetFloat(move_To_Hash, num);

        //��� �ð� �ʱ�ȭ
        OnWaitAnim();
    }
    /// <summary>
    /// �÷��̾��� ��� �ִϸ��̼��� �����Ű�� �Լ� Ȥ�� Ư�� �ൿ�� �Ͽ� ���ð��� �ʱ�ȭ��ų�� ���
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
    //---------------------------------------------------------------------------------------��������
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
        //�÷��̾ ��������
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

    //-------------------------------------------------------------------------------------�ϴ� ����
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
    /// �÷��̾ ���߿��� �ϴ��� ���ų� ����Ҷ� ������Լ�
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
    //-----------------------------------------------------------------------��� �ٶ󺸱�
    private void LookUp(InputAction.CallbackContext context)
    {
        lookUp=!lookUp;

        upperanimator.SetBool(lookUp_To_Hash, lookUp);  
        //���-> �� �ٶ󺸱�
        //��������->���ִ»���->�� �ٶ󺸱�
        //����-> ���ٶ󺸱�
    }

    //------------------------------------------------------------------------�߻�
    private void OnFire(InputAction.CallbackContext context)
    {
        if (!upperBodySprite.enabled)
            upperBodySprite.enabled = true;
        Fire();
        OnWaitAnim();
    }
    private void Fire()
    {
        //���� �����̿� ���� �ִٸ�
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
            //���� ������ ���� �ɾ� �ִ� ���¸�
            if (isSitting)
            {
                //�����϶� 
                if (!isGrouded)
                {
                    upperanimator.SetTrigger(fire_To_Hash);
                }
                else//�����϶� �ɾƼ� �߻�
                {
                    upperBodySprite.enabled = false;
                    lowerAnimator.SetTrigger(fire_To_Hash);
                    StopCoroutine(WaitSitShoot());
                    StartCoroutine(WaitSitShoot());
                }
            }
            else//�Ͼ�ִ� �����϶�
            {
                LowerBodyAnimExit();    //��ü �ִϸ��̼��� �⺻���� ����
                upperanimator.SetTrigger(fire_To_Hash); //��ü �ִϸ��̼� �߻����
            }
            hGAmmo++;           //�Ѿ� ��� �� ����
            //StartCoroutine(ExitTrigger());
            if (hGAmmo > 6 && upperBodySprite.enabled)
            {
                //�÷��̾ �ڵ��� ����

                upperanimator.SetTrigger(reloadAnim_To_Hash);
                hGAmmo = 0;
            }
        }
    }
    /// <summary>
    /// �տ� ª�� ������ ���̸� �� ���� �ִ��� Ȯ���Ѵ� true�� ���� �ִ�
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
    /// ��ü �ִϸ��̼� �� EXit Ʈ���Ÿ� �۵���Ű�� �Լ�
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
