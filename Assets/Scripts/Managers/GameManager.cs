using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score Info.")]
    [SerializeField] private int totalScore = 0;
    public event System.Action<int> OnScoreChanged;

    public bool IsPaused { get; private set; } = false;
    public event System.Action<bool> OnPauseChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        SpawnManager.Instance.Spawn(1);
    }

    #region ����
    public void ResetScore()
    {
        Debug.Log("���� �ʱ�ȭ");

        totalScore = 0;
        OnScoreChanged?.Invoke(totalScore);
    }

    public void AddScore(int _score)
    {
        Debug.Log("���� ȹ�� : " + _score);

        totalScore += _score;
        OnScoreChanged?.Invoke(totalScore);
    }
    #endregion

    #region ����
    public void Pause()
    {
        if (IsPaused) return;

        Debug.Log("���� �Ͻ�����");

        IsPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        OnPauseChanged?.Invoke(true);
    }

    public void Resume()
    {
        if (!IsPaused) return;

        Debug.Log("���� �簳");

        IsPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        OnPauseChanged?.Invoke(false);
    }

    public void Replay()
    {
        Resume();
        
        Debug.Log("���� �ٽ��ϱ�");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Debug.Log("���� ����");

        Time.timeScale = 1f;
        AudioListener.pause = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    #endregion

    #region GET
    public int GetTotalScore() => totalScore;
    #endregion
}
