using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using UnityEngine;
using System.Text;

/// <summary>
/// PlayFab�ň�ӂ�customID�̃��O�C���������s���N���X
/// </summary>
public class PlayFabLogin : MonoBehaviour
{
    // �A�J�E���g��V�K�쐬���邩�ǂ���
    bool shouldCreateAccount;

    // customID�������Ă����ϐ�
    string customID;

    public static bool IsLoggedIn { get; private set; }

    public static event System.Action LoginSucceeded;

    // �f�o�C�X��customID��ۑ�����ꍇ�Ɏg���L�[
    static readonly string CUSTOM_ID_SAVE_KEY = "TEST_RANKING_SAVE_KEY";

    // customID�Ɏg�p���镶���ꗗ
    static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    void Start()
    {
        if (DebugMode.Instance.GetDebugMode())
        {
            //���O�C�������̎��s
            Login();
        }
    }

    void Login()
    {
        // customID��ǂݍ���
        customID = LoadCustomID();

        //Debug.Log($"CustomID: {customID}, CreateAccount: {shouldCreateAccount}");

        // ���O�C�����̑��
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customID,
            CreateAccount = shouldCreateAccount // �V�K�Ȃ�true, �����Ȃ�false
        };

        // ���O�C������
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        if (result.NewlyCreated)
        {
            Debug.Log("�V�K�A�J�E���g���쐬����܂���");
            // �O�̂��ߕۑ��i����̂݁j
            PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, customID);
            PlayerPrefs.Save();
        }

        IsLoggedIn = true;
        Debug.Log("���O�C���ɐ������܂���");
        Debug.Log($"CustomID : {customID}");

        // �C�x���g�ʒm
        LoginSucceeded?.Invoke();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        Debug.Log("���O�C���Ɏ��s���܂���");

        // AccountNotFound ���Ԃ��Ă����ꍇ�́u�K�� CreateAccount = true�v�ōă��O�C��
        if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            Debug.Log("���[�U�[�����݂��Ȃ��̂ŐV�K�쐬���܂�");
            shouldCreateAccount = true;
            StartCoroutine(RetryLoginAfterDelay(0.5f));
        }
    }

    IEnumerator RetryLoginAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Login();
    }

    /// <summary>
    /// customID�̎擾
    /// </summary>
    string LoadCustomID()
    {
        // PlayerPrefs���g����customID���擾�i�ۑ�����Ă��Ȃ���΋󕶎��j
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        if (string.IsNullOrEmpty(id))
        {
            shouldCreateAccount = true;
            id = GenerateAndSaveCustomID(); // �V�K�쐬���ĕۑ�
        }
        else
        {
            shouldCreateAccount = false;
        }

        return id;
    }

    /// <summary>
    /// customID�𐶐����ĕۑ�����
    /// </summary>
    string GenerateAndSaveCustomID()
    {
        int idLength = 32;
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        string newId = stringBuilder.ToString();

        // ��������ID��ۑ�
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, newId);
        PlayerPrefs.Save();

        Debug.Log($"�V����CustomID�𐶐����ĕۑ����܂���: {newId}");

        return newId;
    }
}





/*using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

// <summary>
// PlayFab�ň�ӂ�customID�̃��O�C���������s���N���X
// </summary>

public class PlayFabLogin : MonoBehaviour
{
    //�A�J�E���g��V�K�쐬���邩�ǂ���
    bool shouIdCreateAccount;

    //customID�������Ă����ϐ�
    string customID;

    public static bool IsLoggedIn { get; private set; }

    public static event System.Action LoginSucceeded;

    /// <summary>
    /// ���O�C������
    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //���O�C�������̎��s
        Login();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Login()
    {
        Debug.Log($"CustomID: {customID}, CreateAccount: {shouIdCreateAccount}");

        // TitleID �𖾎��I�ɐݒ�
        //PlayFabSettings.staticSettings.TitleId = "90775";

        //customID��ǂݍ���
        customID = LoadCustomID();

        //���O�C�����̑��
        var request = new LoginWithCustomIDRequest { CustomId = customID, CreateAccount = shouIdCreateAccount };

        //���O�C������
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //���O�C�������̏���
    void OnLoginSuccess(LoginResult result)
    {
        if (shouIdCreateAccount && !result.NewlyCreated)
        {
            Login();
            return;
        }

        if (result.NewlyCreated)
        {
            SaveCustomID();
        }

        IsLoggedIn = true;
        Debug.Log("���O�C���ɐ������܂���");
        Debug.Log($"CustomID :{customID}");

        // �C�x���g�ʒm
        LoginSucceeded?.Invoke();
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        Debug.Log("���O�C���Ɏ��s���܂���");
    }

    /// <summary>
    /// customID�̎擾
    /// </summary>

    // �f�o�C�X��customID��ۑ�����ꍇ�Ɏg���L�[�̐ݒ�
    // PlayerPrefs���g���ĕۑ���customID�̕ۑ����s���̂ŁA���̃L�[�̐ݒ�ł�
    // �ڂ����́u�I�t���C�������L���O����������v�́uPlayerPrefs���ĉ��H�v��������������
    static readonly string CUSTOM_ID_SAVE_KEY = "TEST_RANKING_SAVE_KEY";

    //������ID���擾���郁�\�b�h
    string LoadCustomID()
    {
        //PlayerPrefs���g���āAcustomID���擾����
        //�����ۑ�����Ă��Ȃ��ꍇ�͋󕶎���Ԃ�
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //����id���󕶎���������shouldCreateAccount��true�����A�����łȂ��Ȃ�false��������
        shouIdCreateAccount = string.IsNullOrEmpty(id);

        //shouldCreateAccount��true�Ȃ�customID��V�K�쐬�Afalse�Ȃ�ۑ�����Ă���customID��Ԃ�
        return shouIdCreateAccount ? GenerateCustomID() : id;
    }

    //customID���f�o�C�X�ɕۑ����郁�]�b�g
    void SaveCustomID()
    {
        //PlayerPrefs���g���āAcustomID��ۑ�����
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, customID);
    }

    /// <summary>
    /// customID�̐���
    /// </summary>
    //customID�Ɏg�p���镶���ꗗ
    static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    //customID�𐶐����郁�\�b�h
    string GenerateCustomID()
    {
        //customID�̒���
        int idLength = 32;

        //��������customID��������ϐ��̏�����
        StringBuilder stringBuilder = new StringBuilder(idLength);

        //customID�������_���ŏo�͂��邽�߂ɗ������g��
        var random = new System.Random();

        //customID�̐���
        for (int i = 0; i < idLength; i++)
        {
            //�������g���ă����_���ɕ������������
            //������ꂽ�������customID�ɂ���
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        //��������customID��Ԃ�
        return stringBuilder.ToString();
    }
}*/
