using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    #region private�֐�
    [SerializeField] private Text nameText;             //���O��\������e�L�X�g
    [SerializeField] private InputField nameInputField; //���O���̓t�B�[���h
    #endregion

    #region ���͂��ꂽ�v���C���[���ǂݎ��p
    public static string playerName { get; private set; }
    #endregion

    #region ���O�̓��͂����������ۂɌĂԊ֐�
    //���O�̓��͂����������ۂɌĂԊ֐�
    public void NameInputComplete()
    {
        string input = nameInputField.text.Trim();

        //���̓`�F�b�N�i�󗓂̏ꍇ�̓f�t�H���g���j
        if (string.IsNullOrEmpty(input))
        {
            playerName = "Guest";
        }
        else
        {
            playerName = input;
        }

        nameText.text = playerName;
        Time.timeScale = 1f;
    }
    #endregion
}
