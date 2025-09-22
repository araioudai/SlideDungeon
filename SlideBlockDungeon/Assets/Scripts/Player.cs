using UnityEngine;

public class Player : MonoBehaviour
{
    #region player状態の構造体
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

    #region 定数

    private const int MOVE = 0;

    #endregion

    #region private変数
    [Header("壁との当たり判定用")]
    [SerializeField] private LayerMask wallCheckLayer;    //壁当たり判定用のレイヤー
    [Header("ポータル1との当たり判定用")]
    [SerializeField] private LayerMask portalFirstLayer;  //ポータル1当たり判定用のレイヤー
    [Header("ポータル2との当たり判定用")]
    [SerializeField] private LayerMask portalSecondLayer; //ポータル2当たり判定用のレイヤー
    [Header("ゴールとの当たり判定用")]
    [SerializeField] private LayerMask goalLayer;         //ゴール当たり判定用のレイヤー
    [Header("落とし穴との当たり判定用")]
    [SerializeField] private LayerMask holeLayer;         //落とし穴当たり判定用のレイヤー
    [Header("プレイヤーの移動スピード:float")]
    [SerializeField] private float speed;                 //プレイヤーのスピード
    [Header("先行入力移動タイマーセット:float")]
    [SerializeField] private float bufferTime;            //先行入力移動タイマーセット用

    private Animator m_player;     //プレイヤーのアニメーション用
    private Vector3 tapStartPos;   //スワイプ判定最初押した時
    private Vector3 tapEndPos;     //スワイプ判定最後離した時
    private Vector3 moveDirection; //敵の進む向きに変える用
    private float moveBufferTimer; //先行入力移動タイマー(確保時間)
    private bool moveBuffered;     //先行入力移動用フラグ
    private bool isMove;           //移動スタートフラグ
    private bool movable;          //移動可能かのフラグ
    private bool isSound;          //サウンド一回だけ再生用
    private bool isJump;           //ジャンプ中かのフラグ
    //private bool canJump;
    private bool expansion;        //拡大するかどうか
    private float inputLockTimer;  //入力禁止時間
    private const float INPUT_LOCK_DURATION = 0.2f; //シーン開始から0.2秒は入力無効


    #endregion

    #region Unityイベント関数
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
            return; // 入力を受け付けない
        }

        //タップしてるか
        if (Input.GetMouseButtonDown(0))
        {
            tapStartPos = Input.mousePosition;
        }

        //指を離したか
        if (Input.GetMouseButtonUp(0))
        {
            tapEndPos = Input.mousePosition;
            TapCheck();
        }
        //移動開始していて移動可能状態だったら移動
        Move();
        //移動中でジャンプ可能状態だったらジャンプ
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

    #region Start呼び出し関数
    void Init()
    {
        inputLockTimer = INPUT_LOCK_DURATION; //開始時にロック
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

    #region Update呼び出し関数

    #region Player移動
    void Move()
    {
        //壁に当たったら即停止して return
        if (IsHitWall())
        {
            isMove = false;
            movable = true;
            isSound = false;
            //canJump = false;
            GameManager.Instance.SetPlayerSwiped(false); //スワイプしてない状態にする

            if (moveBuffered) //バッファされてたら次へ移行
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
            //一応初期化
            Vector3 velocity = Vector3.zero;
            ////スワイプした方向に移動
            velocity = moveDirection * speed;
            transform.position += velocity * Time.deltaTime;
        }
    }
    #endregion

    #region バッファタイムの更新
    void UpdateBuffer()
    {
        //バッファ時間の更新
        if (moveBuffered)
        {
            //バッファ確保時間減らす
            moveBufferTimer -= Time.deltaTime;
            if (moveBufferTimer <= 0f)
            {
                moveBuffered = false; //時間切れで無効化
            }
        }
    }
    #endregion

    #region Player移動
    void Jump()
    {
        if (isJump)
        {
            if (!expansion)
            {
                //拡大中
                transform.localScale += Vector3.one * Time.deltaTime;
                if (transform.localScale.x >= 1.7f)
                {
                    expansion = true;
                }
            }
            else
            {
                //縮小中
                transform.localScale -= Vector3.one * Time.deltaTime;
                if (transform.localScale.x <= 1f)
                {
                    transform.localScale = Vector3.one;
                    isJump = false;
                    expansion = false; //次回ジャンプのためにリセット
                }
            }
        }
    }
    #endregion

    #region スワイプ方向取得

    OperationType GetDirection()
    {
        OperationType ret = OperationType.NONE;
        var directionX = tapEndPos.x - tapStartPos.x;
        var directionY = tapEndPos.y - tapStartPos.y;

        if (movable)
        {
            //横と縦どっちにスワイプしたか
            if (Mathf.Abs(directionY) < Mathf.Abs(directionX))
            {
                //横方向にスワイプした時
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
                    //タップ判定
                    ret = OperationType.TAP;
                }

            }
            else if (Mathf.Abs(directionY) > Mathf.Abs(directionX))
            {
                //縦方向にスワイプした時
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
                    //タップ判定
                    ret = OperationType.TAP;
                }
            }
            else
            {
                //上記意外はタップ扱い
                ret = OperationType.TAP;
            }
        }
        return ret;
    }

    #endregion

    #region 移動方向と移動開始

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
                //上にレイを飛ばす
                moveDirection = Vector3.up;
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //スワイプしたことを通知
                m_player.SetTrigger("moveUp");
                break;
            case OperationType.DOWN:
                //下にレイを飛ばす
                moveDirection = Vector3.down;
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //スワイプしたことを通知
                m_player.SetTrigger("moveDown");
                break;
            case OperationType.RIGHT:
                //右にレイを飛ばす
                moveDirection = Vector3.right;
                //右にスワイプされたら右を向く
                transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //スワイプしたことを通知
                m_player.SetTrigger("moveSide");
                break;
            case OperationType.LEFT:
                //左にレイを飛ばす
                moveDirection = Vector3.left;
                //左にスワイプされたら左を向く
                transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
                isMove = true;
                //canJump = true;
                GameManager.Instance.SetPlayerSwiped(true); //スワイプしたことを通知
                m_player.SetTrigger("moveSide");
                break;
            case OperationType.TAP:
                moveBuffered = false; //TAPなら無効
                //ここを「移動中」のみジャンプできるように
                if (!isJump && isMove)
                {
                    isJump = true;
                }
                break;
        }

        //スライド回数を加算する（1回だけ）
        if (direction != OperationType.TAP && direction != OperationType.NONE)
        {
            GameManager.Instance.SlideCount(1);
        }

        //すぐ動けるなら開始TAPだった場合はisMoveを書き換えないようにする
        if (direction != OperationType.TAP && movable)
        {
            isMove = true;
            moveBuffered = false; //消化済み
        }
    }

    #endregion

    #region ループアニメーション用

    void PlayerAnima()
    {
        m_player.SetBool("isMove", isMove);
    }

    #endregion

    #region ゲームクリア、ゲームオーバー監視用

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

    #region Ray当たり判定
    //進む方向にRayを飛ばして壁との当たり判定を行う
    private bool IsHitWall()
    {
        float rayLength = 0.55f;             //Rayの距離
        Vector2 origin = transform.position; //Rayの始点

        //進む方向にRaycast（wallChecklayerに当たったらhit）
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, wallCheckLayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //wallに当たったらtrue
    }
    #endregion

    #region ポータルとの当たり判定

    //進む方向にRayを飛ばしてポータルとの当たり判定を行う
    private bool IsHitPortalFirst()
    {
        float rayLength = 0.01f;              //Rayの距離
        Vector2 origin = transform.position; //Rayの始点

        //進む方向にRaycast（portalFirstLayerに当たったらhit）
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, portalFirstLayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //portalに当たったらtrue
    }

    //進む方向にRayを飛ばしてポータルとの当たり判定を行う
    private bool IsHitPortalSecond()
    {
        float rayLength = 0.01f;              //Rayの距離
        Vector2 origin = transform.position; //Rayの始点

        //進む方向にRaycast（portalSecondLayerに当たったらhit）
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, portalSecondLayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //portalに当たったらtrue
    }

    #endregion

    #region ゴールとの当たり判定

    //進む方向にRayを飛ばしてゴールとの当たり判定を行う
    private bool IsHitGoal()
    {
        float rayLength = 0.01f;              //Rayの距離
        Vector2 origin = transform.position; //Rayの始点

        //進む方向にRaycast（goalLayerに当たったらhit）
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, goalLayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //goalに当たったらtrue
    }

    #endregion

    #region 落とし穴との当たり判定

    //進む方向にRayを飛ばして落とし穴との当たり判定を行う
    private bool IsHitHole()
    {
        float rayLength = 0.1f;              //Rayの距離
        Vector2 origin = transform.position; //Rayの始点

        //進む方向にRaycast（holeLayerに当たったらhit）
        RaycastHit2D hit = Physics2D.Raycast(origin, moveDirection, rayLength, holeLayer);

        //デバッグ用にRayを表示
        Debug.DrawRay(origin, moveDirection * rayLength, Color.green);

        return hit.collider != null;         //holeに当たったらtrue
    }

    #endregion

    #endregion
}
