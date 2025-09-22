using UnityEngine;

public class Player : MonoBehaviour
{
    #region player��Ԃ̍\����
    enum OperationType
    {
        UP, 
        DOWN, 
        LEFT, 
        RIGHT,
        TAP,
        NONE
    }
    #endregion

    #region �萔

    private const int MOVE = 0;

    #endregion

    #region private�ϐ�
    [Header("�ǂƂ̓����蔻��p")]
    [SerializeField] private LayerMask wallCheckLayer;    //�Ǔ����蔻��p�̃��C���[
    [Header("�|�[�^��1�Ƃ̓����蔻��p")]
    [SerializeField] private LayerMask portalFirstLayer;  //�|�[�^��1�����蔻��p�̃��C���[
    [Header("�|�[�^��2�Ƃ̓����蔻��p")]
    [SerializeField] private LayerMask portalSecondLayer; //�|�[�^��2�����蔻��p�̃��C���[
    [Header("�S�[���Ƃ̓����蔻��p")]
    [SerializeField] private LayerMask goalLayer;         //�S�[�������蔻��p�̃��C���[
    [Header("���Ƃ����Ƃ̓����蔻��p")]
    [SerializeField] private LayerMask holeLayer;         //���Ƃ��������蔻��p�̃��C���[
    [Header("�v���C���[�̈ړ��X�s�[�h:float")]
    [SerializeField] private float speed;                 //�v���C���[�̃X�s�[�h
    [Header("��s���͈ړ��^�C�}�[�Z�b�g:float")]
    [SerializeField] private float bufferTime;            //��s���͈ړ��^�C�}�[�Z�b�g�p

    private Animator m_player;     //�v���C���[�̃A�j���[�V�����p
    private Vector3 tapStartPos;   //�X���C�v����ŏ���������
    private Vector3 tapEndPos;     //�X���C�v����Ō㗣������
    private Vector3 moveDirection; //�G�̐i�ތ����ɕς���p
    private float moveBufferTimer; //��s���͈ړ��^�C�}�[(�m�ێ���)
    private bool moveBuffered;     //��s���͈ړ��p�t���O
    private bool isMove;           //�ړ��X�^�[�g�t���O
    private bool movable;          //�ړ��\���̃t���O
    private bool isSound;          //�T�E���h��񂾂��Đ��p
    private bool isJump;           //�W�����v�����̃t���O
    //private bool canJump;
    private bool expansion;        //�g�傷�邩�ǂ���
    private float inputLockTimer;  //���͋֎~����
    private const float INPUT_LOCK_DURATION = 0.2f; //�V�[���J�n����0.2�b�͓��͖���


    #endregion

    #region Unity�C�x���g�֐�
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputLockTimer > 0f)
        {
            inputLockTimer -= Time.deltaTime;
            return; // ���͂��󂯕t���Ȃ�
        }

        //�^�b�v���Ă邩
        if (Input.GetMouseButtonDown(0))
        {
            tapStartPos = Input.mousePosition;
        }

        //�w�𗣂�����
        if (Input.GetMouseButtonUp(0))
        {
            tapEndPos = Input.mousePosition;
            TapCheck();
        }
        //�ړ��J�n���Ă��Ĉړ��\��Ԃ�������ړ�
        Move();
        //�ړ����ŃW�����v�\��Ԃ�������W�����v
        Jump();
        //Debug.Log(isJump);

        GameManager.Instance.SetHitCheck(IsHitPortalFirst(), IsHitPortalSecond());
        //GameManager.Instance.SetPlayerMove(isMove);
        UpdateBuffer();
        PlayerAnima();
        GameClear();
        GameOver();
    }
    #endregion

    #region Start�Ăяo���֐�
    void Init()
    {
        inputLockTimer = INPUT_LOCK_DURATION; //�J�n���Ƀ��b�N
        m_player = GetComponent<Animator>();
        //bufferTime = 0.2f;
        moveBufferTimer = 0;
        moveBuffered = false;
        isMove = false;
        movable = true;
        isSound = false;
        isJump = false;
        //canJump = false;
        expansion = false;
        moveDirection = Vector3.zero;
        tapStartPos = Vector3.zero;     
        tapEndPos = Vector3.zero;       
    }
    #endregion

    #region Update�Ăяo���֐�

    #region Player�ړ�
    void Move()
    {
        //�ǂɓ��������瑦��~���� return
        if (IsHitWall())
        {
            isMove = false;
            movable = true;
            isSound = false;
            //canJump = false;
            GameManager.Instance.SetPlayerSwiped(false); //�X���C�v���ĂȂ���Ԃɂ���

            if (moveBuffered) //�o�b�t�@����Ă��玟�ֈڍs
            {
                isMove = true;
                moveBuffered = false;
                //canJump = true;
            }

            return;
        }

        if (isMove)
        {
            if (!isSound)
            {
                //SoundManager.Instance.SePlay(MOVE);
                isSound = true;
            }
            //�ꉞ������
            Vector3 velocity = Vector3.zero;
            ////�X���C�v���������Ɉړ�
            velocity = moveDirection * speed;
            transform.position += velocity * Time.deltaTime;
        }
    }
    #endregion

    #region �o�b�t�@�^�C���̍X�V
    void UpdateBuffer()
    {
        //�o�b�t�@���Ԃ̍X�V
        if (moveBuffered)
        {
            //�o�b�t�@�m�ێ��Ԍ��炷
            moveBufferTimer -= Time.deltaTime;
            if (moveBufferTimer <= 0f)
            {
                moveBuffered = false; //���Ԑ؂�Ŗ�����
            }
        }
    }
    #endregion

    #region Player�ړ�
    void Jump()
    {
        if (isJump)
        {
            if (!expansion)
            {
                //�g�咆
                transform.localScale += Vector3.one * Time.deltaTime;
                if (transform.localScale.x >= 1.7f)
                {
                    expansion = true;
                }
            }
            else
            {
                //�k����
                transform.localScale -= Vector3.one * Time.deltaTime;
                if (transform.localScale.x <= 1f)
                {
                    transform.localScale = Vector3.one;
                    isJump = false;
                    expansion = false; //����W�����v�̂��߂Ƀ��Z�b�g
                }
            }
        }
    }
    #endregion

    #region �X���C�v�����擾

    OperationType GetDirection()
    {
        OperationType ret = OperationType.NONE;
        var directionX = tapEndPos.x - tapStartPos.x;
        var directionY = tapEndPos.y - tapStartPos.y;

        if (movable)
        {
            //���Əc�ǂ����ɃX���C�v������
            if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
            {
                //�������ɃX���C�v������
                if (directionX > 0)
                {
                    ret = OperationType.RIGHT;
                    movable = false;
                }
                else if (directionX < 0)
                {
                    ret = OperationType.LEFT;
                    movable = false;
                }
                else
                {
                    //�^�b�v����
                    ret = OperationType.TAP;
                }

            }
            else if (Mathf.Abs(directionY) > Mathf.Abs(directionX))
            {
                //�c�����ɃX���C�v������
                if (directionY > 0)
                {
                    ret = OperationType.UP;
                    movable = false;

                }
                else if (directionY < 0)
                {
                    ret = OperationType.DOWN;
                    movable = false;
                }
                else
                {
                    //�^�b�v����
                    ret = OperationType.TAP;
                }
            }
            else
            {
                //��L�ӊO�̓^�b�v����
                ret = OperationType.TAP;
            }
        }
        return ret;
    }

    #endregion

    #region �ړ������ƈړ��J�n

    void TapCheck()
    {
        if (GameManager.Instance.GetPause() || CameraManager.Instance.GetSlide()) { return; }
        moveBuffered = true;
        moveBufferTimer = bufferTime;
        var direction = GetDirection();
        //m_player.SetFloat("direction", (float)direction);
        //m_player.SetInteger("directions", (int)direction);
        switch (direction)
        {
            case OperationType.UP:
                //��Ƀ��C���΂�
                moveDirection = Vector3.up;
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //�X���C�v�������Ƃ�ʒm
                m_player.SetTrigger("moveUp");
                break;
            case OperationType.DOWN:
                //���Ƀ��C���΂�
                moveDirection = Vector3.down;
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //�X���C�v�������Ƃ�ʒm
                m_player.SetTrigger("moveDown");
                break;
            case OperationType.RIGHT:
                //�E�Ƀ��C���΂�
                moveDirection = Vector3.right;
                //�E�ɃX���C�v���ꂽ��E������
                transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //�X���C�v�������Ƃ�ʒm
                m_player.SetTrigger("moveSide");
                break;
            case OperationType.LEFT:
                //���Ƀ��C���΂�
                moveDirection = Vector3.left;
                //���ɃX���C�v���ꂽ�獶������
                transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //�X���C�v�������Ƃ�ʒm
                m_player.SetTrigger("moveSide");
                break;
            case OperationType.TAP:
                moveBuffered = false; //TAP�Ȃ疳��
                //�������u�ړ����v�̂݃W�����v�ł���悤��
                if (!isJump && isMove)
                {
                    isJump = true;
                }
                break;
        }

        //�X���C�h�񐔂����Z����i1�񂾂��j
        if (direction != OperationType.TAP && direction != OperationType.NONE)
        {
            GameManager.Instance.SlideCount(1);
        }

        //����������Ȃ�J�nTAP�������ꍇ��isMove�����������Ȃ��悤�ɂ���
        if (direction != OperationType.TAP && movable)
        {
            isMove = true;
            moveBuffered = false; //�����ς�
        }
    }

    #endregion

    #region ���[�v�A�j���[�V�����p

    void PlayerAnima()
    {
        m_player.SetBool("isMove", isMove);
    }

    #endregion

    #region �Q�[���N���A�A�Q�[���I�[�o�[�Ď��p

    void GameClear()
    {
        if (IsHitGoal())
        {
            GameManager.Instance.SetGameClear(IsHitGoal());
        }
    }

    void GameOver()
    {
        if (IsHitHole())
        {
            GameManager.Instance.SetGameOver(IsHitHole());
        }
    }

    #endregion

    #region Ray�����蔻��
    //�i�ޕ�����Ray���΂��ĕǂƂ̓����蔻����s��
    private bool IsHitWall()
    {
        float rayLength = 0.55f;             //Ray�̋���
        Vector2 origin = transform.position; //Ray�̎n�_

        //�i�ޕ�����Raycast�iwallChecklayer�ɓ���������hit�j
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, wallCheckLayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //wall�ɓ���������true
    }
    #endregion

    #region �|�[�^���Ƃ̓����蔻��

    //�i�ޕ�����Ray���΂��ă|�[�^���Ƃ̓����蔻����s��
    private bool IsHitPortalFirst()
    {
        float rayLength = 0.01f;              //Ray�̋���
        Vector2 origin = transform.position; //Ray�̎n�_

        //�i�ޕ�����Raycast�iportalFirstLayer�ɓ���������hit�j
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, portalFirstLayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //portal�ɓ���������true
    }

    //�i�ޕ�����Ray���΂��ă|�[�^���Ƃ̓����蔻����s��
    private bool IsHitPortalSecond()
    {
        float rayLength = 0.01f;              //Ray�̋���
        Vector2 origin = transform.position; //Ray�̎n�_

        //�i�ޕ�����Raycast�iportalSecondLayer�ɓ���������hit�j
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, portalSecondLayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //portal�ɓ���������true
    }

    #endregion

    #region �S�[���Ƃ̓����蔻��

    //�i�ޕ�����Ray���΂��ăS�[���Ƃ̓����蔻����s��
    private bool IsHitGoal()
    {
        float rayLength = 0.01f;              //Ray�̋���
        Vector2 origin = transform.position; //Ray�̎n�_

        //�i�ޕ�����Raycast�igoalLayer�ɓ���������hit�j
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, goalLayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //goal�ɓ���������true
    }

    #endregion

    #region ���Ƃ����Ƃ̓����蔻��

    //�i�ޕ�����Ray���΂��ė��Ƃ����Ƃ̓����蔻����s��
    private bool IsHitHole()
    {
        float rayLength = 0.1f;              //Ray�̋���
        Vector2 origin = transform.position; //Ray�̎n�_

        //�i�ޕ�����Raycast�iholeLayer�ɓ���������hit�j
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, holeLayer);

        //�f�o�b�O�p��Ray��\��
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //hole�ɓ���������true
    }

    #endregion

    #endregion
}
