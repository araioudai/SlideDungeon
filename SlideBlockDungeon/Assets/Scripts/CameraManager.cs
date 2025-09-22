using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    #region �V���O���g��
    public static CameraManager Instance { get; private set; } //���̃X�N���v�g����Instance�ŃA�N�Z�X�ł���悤�ɂ���
    #endregion

    #region private�ϐ�

    [Header("�X���C�_�[���Z�b�g")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform sliderRange;
    [Header("�v���C���[���Z�b�g")]
    [SerializeField] private Transform target;                         //�Ǐ]�Ώہi�v���C���[��Transform�j
    [Header("�f�b�h�]�[����X�EY�͈�")]
    [SerializeField] private Vector2 deadZone = new Vector2(2f, 1.5f); //�f�b�h�]�[����X�EY�͈́i���S����̋����j
    [Header("�Ǐ]���x���Z�b�g")]
    [SerializeField] private float followSpeed = 5f;                   //�Ǐ]���x�iLerp�̌W���j

    private bool isSlider;

    #endregion
    
    #region Get�֐�

    public bool GetSlide() { return isSlider; }

    #endregion

    #region Unity�C�x���g�֐�
    void Awake()
    {
        //�V���O���g���Ǘ�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //����Instance������Ύ�����j��
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //onValueChanged�C�x���g�Ƀ��\�b�h��o�^����
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isSlider);
        TapPosCheck();
        CameraPos();
    }

    void LateUpdate()
    {
        if (target == null) return;                          //�Ǐ]�Ώۂ��Z�b�g����Ă��Ȃ���Ώ����I��
        if (!GameManager.Instance.GetIsFollow()) { return; } //�Ǐ]����K�v���Ȃ��X�e�[�W�Ȃ珈���I��
        if (isSlider) { return; }

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;

        // ------ X ���̏��� ------
        float dx = tpos.x - pos.x;
        if (Mathf.Abs(dx) > deadZone.x)
        {
            // �f�b�h�]�[���O�ɏo�Ă���ꍇ�̂�Lerp�Ŋ��炩�ɒǏ]
            pos.x = Mathf.Lerp(pos.x,
                               tpos.x - Mathf.Sign(dx) * deadZone.x,
                               followSpeed * Time.deltaTime);
        }

        // ------ Y ���̏����i�K�v�Ȃ�j ------
        /*float dy = tpos.y - pos.y;
        if (Mathf.Abs(dy) > deadZone.y)
        {
            pos.y = Mathf.Lerp(pos.y,
                               tpos.y - Mathf.Sign(dy) * deadZone.y,
                               followSpeed * Time.deltaTime);
        }*/
        // �J�����ʒu���X�V�iZ���͈ێ��j
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    #endregion


    #region Start�Ăяo���֐�


    #endregion

    #region Update�Ăяo���֐�

    #region �J�����Ǐ]
    void CameraPos()
    {
        if (isSlider)
        {
            //�X���C�_�[�̒l(0�`1)�� 0�`5 �̃��[���h���W�ɕϊ�
            float x = Mathf.Lerp(0f, 5f, slider.value);

            //�J�����̈ʒu���X�V
            transform.position = new Vector3(x, 0, -10);
        }
        else
        {
            if (GameManager.Instance.GetIsFollow())
            {
                //transform.position = new Vector3(player.transform.position.x, transform.position.y, -10);
            }
        }
    }
    #endregion

    #endregion

    #region Slider�l���ύX���ꂽ�Ƃ��ɌĂяo����郁�\�b�h

    //Slider�̒l���ύX���ꂽ�Ƃ��ɌĂяo����郁�\�b�h
    public void OnSliderValueChanged(float value)
    {
        Debug.Log("Slider�̒l���ύX����܂���: " + value);

        isSlider = true;
    }

    #endregion

    #region �^�b�v�ꏊ����

    void TapPosCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //�^�b�v�����ʒu
            Vector2 tapPos = Input.mousePosition;

            //�X���C�_�[�͈͂Ɋ܂܂�Ă��邩
            bool inSlider = RectTransformUtility.RectangleContainsScreenPoint(
                sliderRange, //�ΏۂƂȂ�UI�v�f
                tapPos,      //�^�b�v/�N���b�N���ꂽ�X�N���[�����W
                Camera.main  // UI��`�悵�Ă���J����
            );

            if (!inSlider)
            {
                //�X���C�_�[�ȊO�̏ꏊ���^�b�v����false
                isSlider = false;
            }
            else
            {
                //�ꉞ�X���C�_�[������������ł����Ă���
                isSlider = true;
            }
        }
    }

    #endregion
}
