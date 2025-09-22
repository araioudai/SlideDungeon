using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpManager : MonoBehaviour
{
    #region private�ϐ�
    [Header("�|�[�^�����Z�b�g")]
    [SerializeField] private Transform warpPointFirst;
    [SerializeField] private Transform warpPointSecond;
    [Header("�v���C���[���Z�b�g")]
    [SerializeField] private Transform playerPos;
    [Header("�N�[���^�C���b��")]
    [SerializeField] private float warpCooldown = 1f;

    private bool isWarp;
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
        //���[�v����N�[���^�C�����͏������Ȃ�
        if (isWarp) return;

        //�|�[�^���ɓ���������R���[�`���J�n
        if (GameManager.Instance.GetHitCheckFirst())
        {
            StartCoroutine(PlayerWarp(warpPointSecond));
        }
        else if (GameManager.Instance.GetHitCheckSecond())
        {
            StartCoroutine(PlayerWarp(warpPointFirst));
        }
        //Debug.Log(GameManager.Instance.GetPlayerSwiped());
    }
    #endregion

    #region Start�Ăяo���֐�

    void Init()
    {
        isWarp = false;
    }

    #endregion

    #region Update�Ăяo���֐�

    #region ���[�v�N�[���_�E���i���x�����[�v���Ȃ��悤�Ɂj
    private IEnumerator PlayerWarp(Transform targetPoint)
    {
        isWarp = true;

        //���[�v����
        playerPos.position = targetPoint.position;
        //GameManager.Instance.SetPlayerMove(false);

        //���[�v����͕K���u�܂��X���C�v���ĂȂ���ԁv�Ƀ��Z�b�g
        GameManager.Instance.SetPlayerSwiped(false);

        /* yield return new WaitUntil(() => ������);
       �u�������� true �ɂȂ�܂ő҂v*/
        //�v���C���[���Ăѓ����o���܂ő҂�
        yield return new WaitUntil(() => GameManager.Instance.GetPlayerSwiped());

        //�N�[���^�C���ҋ@���邱�Ƃł����Ƀ��[�v��h��
        yield return new WaitForSeconds(warpCooldown);

        //�X���C�v�ς݃t���O�����Z�b�g�i����j
        GameManager.Instance.SetPlayerSwiped(false);

        isWarp = false;
    }
    #endregion

    #endregion
}




/*�\�[�X�R�[�h
 * �����̃��[�v�����ꍇ����Ȋ����Ŏ擾�ōs���邩��

            // �����Ώۂ̃��C���[�����w��i��: "Player"�j
            string targetLayerName = "Player";
int targetLayer = LayerMask.NameToLayer(targetLayerName);

// �S�Ă�GameObject���������� (��: GameObject.FindObjectsOfType)
GameObject[] allObjects = FindObjectsOfType<GameObject>();

// ����̃��C���[������GameObject���i�[���郊�X�g���쐬
List<GameObject> objectsOnLayer = new List<GameObject>();
foreach (GameObject obj in allObjects)
{
    // �I�u�W�F�N�g�̃��C���[�ƈ�v���邩�m�F
    if (obj.layer == targetLayer)
    {
        objectsOnLayer.Add(obj);
    }
}
// ���X�g��z��ɕϊ�
GameObject[] resultArray = objectsOnLayer.ToArray();*/
