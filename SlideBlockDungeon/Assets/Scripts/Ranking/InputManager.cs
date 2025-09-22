using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Text nameText;             //���O��\������e�L�X�g
    [SerializeField] private InputField nameInputField; //���O���̓t�B�[���h

    public static string playerName { get; private set; }

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
}
