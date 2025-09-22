using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Text nameText;             //名前を表示するテキスト
    [SerializeField] private InputField nameInputField; //名前入力フィールド

    public static string playerName { get; private set; }

    //名前の入力が完了した際に呼ぶ関数
    public void NameInputComplete()
    {
        string input = nameInputField.text.Trim();

        //入力チェック（空欄の場合はデフォルト名）
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
