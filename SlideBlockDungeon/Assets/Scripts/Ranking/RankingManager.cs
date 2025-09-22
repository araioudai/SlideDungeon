using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankingManager : MonoBehaviour
{
    #region �V���O���g��
    public static RankingManager Instance { get; private set; }
    #endregion

    #region private�ϐ�
    [SerializeField] private Text[] textRanking;
    private string m_name;
    private float m_time;   //�N���A�^�C���i�b�j
    private bool m_request; //�����L���O�Ď擾���N�G�X�g
    private string currentRankingName;
/*    private float lastUserDataUpdateTime = -1000f;   //�Ō�ɑ��M��������
    private const float userDataUpdateCooldown = 10f; //�N�[���^�C���i�b�j*/
    #endregion

    #region �Z�b�g�֐�
    public void SetTime(float time) { m_time = time; }
    public void SetRequest(bool req) { m_request = req; }
    public void SetStage(int stageIndex)
    {
        currentRankingName = $"Stage{stageIndex}Ranking";
        //Debug.Log($"RankingManager: {currentRankingName} �ɐ؂�ւ��܂���");
    }
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //�V�[�����[�h�C�x���g�o�^
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        currentRankingName = "Stage1Ranking";
        for (int i = 0; i < textRanking.Length; i++)
        {
            if (textRanking[i] != null)
            {
                textRanking[i].text = "";
            }
        }

        if (PlayFabLogin.IsLoggedIn)
        {
            GetRanking();
        }
        else
        {
            PlayFabLogin.LoginSucceeded += OnLoginSucceeded;
        }
    }

    void Update()
    {
        //Debug.Log($"Update m_request={m_request}, IsLoggedIn={PlayFabLogin.IsLoggedIn}");

        if (!PlayFabLogin.IsLoggedIn) return;

        m_name = InputManager.playerName;

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetUserName(m_name);
            SubmitClearTime(m_time);
        }
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (GameManager.Instance.GetGameClear())
            {
                SetUserName(m_name);
                SubmitClearTime(m_time);
            }
        }
        if (m_request)
        {
            GetRanking();
            m_request = false;
        }
        //Debug.Log(m_request);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            var parent = GameObject.Find("Canvas").transform.Find("RankingPanel/RankingTexts"); ; //�I�u�W�F�N�g���ɍ��킹��
            if (parent != null)
            {
                textRanking = parent.GetComponentsInChildren<Text>();
                UpdateRankingUI();
            }
            else
            {
                Debug.LogWarning("RankingTexts ��������܂���");
            }
        }
    }

    void OnLoginSucceeded()
    {
        PlayFabLogin.LoginSucceeded -= OnLoginSucceeded;
        GetRanking();
    }

    void SetUserName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Guest";
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => Debug.Log("�v���C���[���ύX ����"),
            error => Debug.Log("�v���C���[���ύX ���s: " + error.GenerateErrorReport()));
    }

    public void TestSend()
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate { StatisticName = "Stage1Ranking", Value = 123 }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("�e�X�g���M����"),
            error => Debug.LogError("�e�X�g���M���s: " + error.GenerateErrorReport())
        );
    }

    // �X�R�A�i�^�C���j���M
    void SubmitClearTime(float time)
    {
        if (!PlayFabLogin.IsLoggedIn)
        {
            Debug.LogWarning("���O�C�����Ă��Ȃ����߃^�C�����M�ł��܂���");
            return;
        }

        int modifiedScore = int.MaxValue - Mathf.RoundToInt(time * 100f);
        if (modifiedScore < 0) modifiedScore = 0;

        var statisticUpdate = new StatisticUpdate
        {
            StatisticName = currentRankingName,
            Value = modifiedScore
        };

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> { statisticUpdate }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log($"�^�C�����M ����: {modifiedScore}");

            // UserData�������X�V
            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "ClearTime", time.ToString("F2") },
                { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() }
            },
                Permission = UserDataPermission.Public
            };

            PlayFabClientAPI.UpdateUserData(dataRequest, _ => GetRanking(), err =>
            {
                Debug.LogWarning("UserData�ۑ����s: " + err.GenerateErrorReport());
                GetRanking();
            });

        }, error =>
        {
            Debug.LogError("�^�C�����M ���s:\n" + error.GenerateErrorReport());
        });
    }


    /*    void SubmitClearTime(float time)
        {
            int modifiedScore = int.MaxValue - Mathf.RoundToInt(time * 100f);

            var statisticUpdate = new StatisticUpdate
            {
                StatisticName = currentRankingName,
                Value = modifiedScore,
            };

            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> { statisticUpdate }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
            {
                Debug.Log("�^�C�����M ����");

                // UserData�������ɍX�V�i�Q�l�l�Ƃ��āj
                var dataRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() },
                        { "ClearTime", time.ToString("F2") }
                    },
                    Permission = UserDataPermission.Public
                };
                PlayFabClientAPI.UpdateUserData(dataRequest,
                    _ => GetRanking(),
                    err => { Debug.LogWarning("UserData�ۑ����s: " + err.GenerateErrorReport()); GetRanking(); });

            }, error =>
            {
                Debug.Log("�^�C�����M ���s: " + error.GenerateErrorReport());
            });
        }*/


    /*void SubmitClearTime(float time)
    {
        // ���[�g���~�b�g�΍�: �N�[���^�C�����o�߂Ȃ�X�L�b�v
        if (Time.time - lastUserDataUpdateTime < userDataUpdateCooldown)
        {
            Debug.LogWarning("UserData�X�V: �N�[���^�C�����̂��߃X�L�b�v");
            return;
        }
        lastUserDataUpdateTime = Time.time;

        int modifiedScore = int.MaxValue - Mathf.RoundToInt(time * 100f);
        var statisticUpdate = new StatisticUpdate
        {
            StatisticName = currentRankingName,
            Value = modifiedScore,
        };

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> { statisticUpdate }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log("�^�C�����M ����");

            // UserData�������ɍX�V�i�Q�l�l�Ƃ��āj
            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() },
                { "ClearTime", time.ToString("F2") }
            },
                Permission = UserDataPermission.Public
            };
            PlayFabClientAPI.UpdateUserData(dataRequest,
                _ => GetRanking(),
                err => { Debug.LogWarning("UserData�ۑ����s: " + err.GenerateErrorReport()); GetRanking(); });

        }, error =>
        {
            Debug.Log("�^�C�����M ���s: " + error.GenerateErrorReport());
        });
    }*/

    // �����L���O�擾
    void GetRanking()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = currentRankingName,
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, leaderboardResult =>
        {
            cachedRanking.Clear();
            foreach (var item in leaderboardResult.Leaderboard)
            {
                float clearTime = (int.MaxValue - item.StatValue) / 100f;
                cachedRanking.Add((item.DisplayName, clearTime));
                Debug.Log($"�����L���O�擾: {item.DisplayName} {clearTime:F2}�b");
            }
            UpdateRankingUI();
        }, error =>
        {
            Debug.Log("�����L���O�擾���s: " + error.GenerateErrorReport());
        });
    }

    // �����L���O�f�[�^�L���b�V���i���O�E�^�C���̂݁j
    private List<(string name, float clearTime)> cachedRanking = new List<(string, float)>();

    // UI �X�V
    void UpdateRankingUI()
    {
        for (int i = 0; i < textRanking.Length; i++)
        {
            if (textRanking[i] == null) continue; //�j������Ă�����X�L�b�v

            if (i < cachedRanking.Count)
            {
                var r = cachedRanking[i];
                textRanking[i].text = $"{r.name}�@{r.clearTime:F2}�b";
            }
            else
            {
                textRanking[i].text = "";
            }
        }
    }
}



/*using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankingManager : MonoBehaviour
{
    #region �V���O���g��
    public static RankingManager Instance { get; private set; }
    #endregion

    [SerializeField] private Text[] textRanking;
    private string m_name;
    private float m_time;   //�X�R�A�ł͂Ȃ��N���A�^�C����ێ�
    private bool m_request; //�^�C�g���}�l�[�W���[���烊�N�G�X�g���ꂽ��ranking�X�V

    public void SetTime(float time) { m_time = time; }   //�^�C�����Z�b�g
    public void SetRequest(bool req) { m_request = req; }

    *//*float GetTime() { return m_time; }                  //�^�C�����擾*//*

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  //�V�[���؂�ւ��Ŕj�����Ȃ�
    }

    void Start()
    {
        //�����L���O��UI��������
        for (int i = 0; i < textRanking.Length; i++)
        {
            if (textRanking[i] != null)
            {
                textRanking[i].text = $"{i + 1}��";
            }
        }

        //���O�C���ς݂Ȃ烉���L���O���擾
        if (PlayFabLogin.IsLoggedIn)
        {
            GetRanking();
        }
        else
        {
            //���O�C���������ɃC�x���g���t�b�N
            PlayFabLogin.LoginSucceeded += OnLoginSucceeded;
        }
    }

    void Update()
    {
        if (!PlayFabLogin.IsLoggedIn) return;

        //���͂��ꂽ�v���C���[�����擾
        m_name = InputManager.playerName;

        //�X�R�A���M��������
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetUserName(m_name);
            SubmitClearTime(m_time);
        }
        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            if (GameManager.Instance.GetGameClear())
            {
                SetUserName(m_name);
                SubmitClearTime(m_time);
            }
        }
        //Debug.Log(m_request);
        //�����L���O�Ď擾
        if (m_request)
        {
            GetRanking();
            m_request = false;
        }
    }

    void OnLoginSucceeded()
    {
        PlayFabLogin.LoginSucceeded -= OnLoginSucceeded;
        GetRanking();
    }

    //�v���C���[����PlayFab��DisplayName�ɐݒ�
    void SetUserName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Guest";
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => Debug.Log("�v���C���[���ύX ����"),
            error => Debug.Log("�v���C���[���ύX ���s: " + error.GenerateErrorReport()));
    }

    //�X�R�A�i�^�C���j���M
    void SubmitClearTime(float time)
    {
        //Debug.Log($"���M����^�C��: {time:F2}�b");

        //PlayFab�ɂ�int��������Ȃ��̂� "�������^�C�� = ������" �ɂȂ�悤�ɕϊ�
        int modifiedScore = int.MaxValue - Mathf.RoundToInt(time * 100f);

        var statisticUpdate = new StatisticUpdate
        {
            StatisticName = "Stage1Ranking",
            Value = modifiedScore,
        };

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> { statisticUpdate }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log("�^�C�����M ����");

            //�����L���O���ёւ��p�Ƀ^�C���ƃ^�C���X�^���v��UserData�ɕۑ�
            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() },
                    { "ClearTime", time.ToString("F2") } //�� ������2�ʂ܂ŕۑ�
                },
                Permission = UserDataPermission.Public //
            };
            PlayFabClientAPI.UpdateUserData(dataRequest,
                _ => GetRanking(), //�ۑ�������Ƀ����L���O�擾
                err => { Debug.LogWarning("UserData�ۑ����s: " + err.GenerateErrorReport()); GetRanking(); });

        }, error =>
        {
            Debug.Log("�^�C�����M ���s: " + error.GenerateErrorReport());
        });
    }

    //�����L���O�擾
    void GetRanking()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Stage1Ranking",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, leaderboardResult =>
        {
            cachedRanking.Clear();
            var playerIds = new List<string>();

            foreach (var item in leaderboardResult.Leaderboard)
            {
                //ClearTime �͌�� UserData ����擾����̂� "??" �����Ă���
                cachedRanking.Add((item.DisplayName, item.StatValue, long.MaxValue, item.PlayFabId, "??"));
                playerIds.Add(item.PlayFabId);
            }

            UpdateRankingUI(); //��Ɏb��\��

            int receivedCount = 0;

            //�e�v���C���[�� UserData ���� ClearTime �� Timestamp ���擾
            foreach (var pid in playerIds)
            {
                var dataRequest = new GetUserDataRequest { PlayFabId = pid };
                PlayFabClientAPI.GetUserData(dataRequest, dataResult =>
                {
                    long ts = long.MaxValue;
                    string clearTimeStr = "??";

                    if (dataResult.Data != null)
                    {
                        if (dataResult.Data.ContainsKey("ScoreTimestamp"))
                            long.TryParse(dataResult.Data["ScoreTimestamp"].Value, out ts);

                        if (dataResult.Data.ContainsKey("ClearTime"))
                            clearTimeStr = dataResult.Data["ClearTime"].Value;
                    }

                    for (int i = 0; i < cachedRanking.Count; i++)
                    {
                        if (cachedRanking[i].playFabId == pid)
                        {
                            cachedRanking[i] = (cachedRanking[i].name, cachedRanking[i].score, ts, pid, clearTimeStr);
                            break;
                        }
                    }

                    receivedCount++;
                    if (receivedCount == cachedRanking.Count) SortAndDisplay();

                }, err =>
                {
                    Debug.LogWarning("UserData�擾���s: " + err.GenerateErrorReport());
                    receivedCount++;
                    if (receivedCount == cachedRanking.Count) SortAndDisplay();
                });
            }

        }, error =>
        {
            Debug.Log("�����L���O�擾���s: " + error.GenerateErrorReport());
        });
    }

    //���בւ��i�X�R�A�D�� �� �^�C���X�^���v���j
    void SortAndDisplay()
    {
        cachedRanking.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score); //�X�R�A�i�ϊ��ςݒl�j�Ŕ�r
            if (scoreCompare != 0) return scoreCompare;
            return a.timestamp.CompareTo(b.timestamp);    //���X�R�A�Ȃ�Â����M��D��
        });
        UpdateRankingUI();
    }

    //�����L���O�f�[�^�L���b�V��
    private List<(string name, int score, long timestamp, string playFabId, string clearTime)> cachedRanking
        = new List<(string, int, long, string, string)>();

    //UI �X�V
    void UpdateRankingUI()
    {
        for (int i = 0; i < textRanking.Length; i++)
        {
            if (i < cachedRanking.Count)
            {
                var r = cachedRanking[i];
                textRanking[i].text = $"{r.name}�@{r.clearTime:F2}�b"; // ClearTime��StatValue����v�Z
            }
            else
            {
                textRanking[i].text = "";
            }
        }

        *//*        for (int i = 0; i < textRanking.Length; i++)
                {
                    if (textRanking[i] == null) continue; //�j������Ă�����X�L�b�v
                    if (i < cachedRanking.Count)
                    {
                        var r = cachedRanking[i];
                        //"xx.xx�b" �̌`���ŕ\��
                        textRanking[i].text = $"{r.name}�@{r.clearTime}�b";
                    }
                    else
                    {
                        textRanking[i].text = "";
                    }
                }*//*
    }
}*/

/*
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    #region �V���O���g��
    public static RankingManager Instance { get; private set; } //���̃X�N���v�g����Instance�ŃA�N�Z�X�ł���悤�ɂ���
    #endregion

    [SerializeField] private Text[] textRanking;
    string m_name;
    float m_time;

    public void SetTime(float time) { m_time = time; }

    float GetTime() { return m_time; }

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
        //DontDestroyOnLoad(gameObject);  //�V�[���؂�ւ��Ŕj�����Ȃ�
    }

    //Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            if (textRanking[i] != null)
            {
                textRanking[i].text = $"{i + 1}��";
            }
        }

        //���O�C���ς݂Ȃ瑦�����L���O�擾
        if (PlayFabLogin.IsLoggedIn)
        {
            GetRanking();
        }
        else
        {
            //�����_�����Ɖ������Â炢���烁�\�b�h�o�^����
            PlayFabLogin.LoginSucceeded += OnLoginSucceeded;
        }
    }

    //�e�X�g�p�̉��v���O����
    //�����L���O�̑���M���s��
    void Update()
    {
        if (!PlayFabLogin.IsLoggedIn) return; //���O�C���������ĂȂ���Ή������Ȃ�

        m_name = InputManager.playerName;

        //�L�[�{�[�h�ŁuB�v�����͂��ꂽ��X�R�A�Ƃ���m_score�𑗐M����
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetUserName(m_name);
            SubmitClearTime(GetTime());
        }

        //�L�[�{�[�h�ŁuC�v�����͂��ꂽ�烉���L���O���擾����
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetRanking();
        }
    }

    #endregion

    void OnLoginSucceeded()
    {
        PlayFabLogin.LoginSucceeded -= OnLoginSucceeded; //�o�^����
        GetRanking();
    }

    //�ύX��̃v���C���[���������ɂ���
    void SetUserName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Guest";
        }

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSetUserNameSuccess, OnSetUserNameFailure);

        void OnSetUserNameSuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("����");
            //SubmitScore(GetScore());
        }

        void OnSetUserNameFailure(PlayFabError error)
        {
            Debug.Log("���s");
            //SubmitScore(GetScore());
        }
    }


    //���M����X�R�A�������ɂ���
    *//*    void SubmitScore(int score)
        {
            //���������L���O�p�ɃX�R�A���C������
            //�~�������L���O�̏ꍇ�͂��̍�Ƃ͏ȗ�����
            //�����̓��e�Ɨ��R�͌�q
            //int modifiedScore = int.MaxValue - score;

            Debug.Log($"���M����X�R�A: {score}");
            int modifiedScore = score;

            var statisticUpdate = new StatisticUpdate
            {
                StatisticName = "Stage1Ranking",
                Value = modifiedScore,
            };
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> { statisticUpdate }
            };

            //�X�R�A���M
            PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
            {
                Debug.Log("�X�R�A�̑��M�ɐ������܂���");

                //���M������UserData�ɕۑ��i���X�R�A���̑��M������p�j
                var dataRequest = new UpdateUserDataRequest
                {
                    Data = new Dictionary<string, string>
                    {
                        { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() }
                    }
                };
                PlayFabClientAPI.UpdateUserData(dataRequest,
                    _ => GetRanking(),
                    err => { Debug.LogWarning("�����ۑ����s: " + err.GenerateErrorReport()); GetRanking(); });

            }, error =>
            {
                Debug.Log("�X�R�A�̑��M�Ɏ��s���܂���");
            });
        }*//*

    void SubmitClearTime(float time)
    {
        Debug.Log($"���M����X�R�A: {time}");

        //������2�ʂ܂ň��������̂�100�{���Đ�����
        int modifiedScore = int.MaxValue - Mathf.RoundToInt(time * 100f);

        var statisticUpdate = new StatisticUpdate
        {
            StatisticName = "Stage1Ranking",
            Value = modifiedScore,
        };
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> { statisticUpdate }
        };

        //�X�R�A���M
        PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
        {
            Debug.Log("�X�R�A�̑��M�ɐ������܂���");

            //�{����float�l��UserData�ɕۑ����Ă���
            var dataRequest = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
            {
                { "ScoreTimestamp", System.DateTime.UtcNow.Ticks.ToString() },
                { "ClearTime", time.ToString("F2") } //������2�ʂ܂ŕۑ�
            }
            };
            PlayFabClientAPI.UpdateUserData(dataRequest,
                _ => GetRanking(),
                err => { Debug.LogWarning("�����ۑ����s: " + err.GenerateErrorReport()); GetRanking(); });

        }, error =>
        {
            Debug.Log("�X�R�A�̑��M�Ɏ��s���܂���");
        });
    }


    ///<summary>
    ///�����L���O�擾���\�b�h
    ///</summary>
    void GetRanking()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Stage1Ranking",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, leaderboardResult =>
        {
            cachedRanking.Clear();
            var playerIds = new List<string>();

            foreach (var item in leaderboardResult.Leaderboard)
            {
                //�����͈�U�ő�l�i��Œu��������j
                cachedRanking.Add((item.DisplayName, item.StatValue, long.MaxValue, item.PlayFabId));
                playerIds.Add(item.PlayFabId);
            }

            //�܂��X�R�A�݂̂Ŏb��\��
            UpdateRankingUI();

            int receivedCount = 0;

            //�e�v���C���[�̑��M�������擾
            foreach (var pid in playerIds)
            {
                var dataRequest = new GetUserDataRequest { PlayFabId = pid };
                PlayFabClientAPI.GetUserData(dataRequest, dataResult =>
                {
                    long ts = long.MaxValue;
                    if (dataResult.Data != null && dataResult.Data.ContainsKey("ScoreTimestamp"))
                    {
                        long.TryParse(dataResult.Data["ScoreTimestamp"].Value, out ts);
                    }

                    for (int i = 0; i < cachedRanking.Count; i++)
                    {
                        if (cachedRanking[i].playFabId == pid)
                        {
                            cachedRanking[i] = (cachedRanking[i].name, cachedRanking[i].score, ts, pid);
                            break;
                        }
                    }

                    receivedCount++;

                    //�S������������\�[�g���ĕ\��
                    if (receivedCount == cachedRanking.Count)
                    {
                        SortAndDisplay();
                    }

                }, err =>
                {
                    Debug.LogWarning("���[�U�[�f�[�^�擾���s: " + err.GenerateErrorReport());
                    receivedCount++;

                    if (receivedCount == cachedRanking.Count)
                    {
                        SortAndDisplay();
                    }
                });
            }

        }, error =>
        {
            Debug.Log("�����L���O�̎擾�Ɏ��s���܂���");
        });
    }

    void SortAndDisplay()
    {
        cachedRanking.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score); //�X�R�A�~��
            if (scoreCompare != 0) return scoreCompare;
            return a.timestamp.CompareTo(b.timestamp);    //���M���������i����������j
        });
        UpdateRankingUI();
    }

    //���[�U�[���ӂ̏��������L���O�̃X�R�A�擾���\�b�h
    void GetRankingAroundPlayer()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Stage1Ranking",
            MaxResultsCount = 11
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, leaderboardResult =>
        {
            foreach (var item in leaderboardResult.Leaderboard)
            {
                Debug.Log($"{item.Position + 1}�ʁ@�v���C���[���F{item.DisplayName}�@�X�R�A�F{item.StatValue}");
            }
        }, error =>
        {
            Debug.Log("�����L���O�̎擾�Ɏ��s���܂���");
        });
    }

    //�����Ή��Ń����L���O�L���b�V��
    private List<(string name, int score, long timestamp, string playFabId)> cachedRanking
        = new List<(string, int, long, string)>();

    void UpdateRankingUI()
    {
        for (int i = 0; i < textRanking.Length; i++)
        {
            if (i < cachedRanking.Count) { var r = cachedRanking[i]; textRanking[i].text = $"{i + 1}�ʁ@{r.name}�@{r.score}"; }
            else
            {
                textRanking[i].text = $"{i + 1}��";
            }
        }
    }
}*/