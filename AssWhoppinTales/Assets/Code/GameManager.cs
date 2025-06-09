// Nguồn file: Code/GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isGameOver = false;

    // === PHẦN ĐÃ SỬA: Tách riêng 2 panel ===
    [Header("UI Panels")]
    public GameObject victoryPanel; // Panel hiển thị khi thắng
    public GameObject gameOverPanel; // Panel hiển thị khi thua

    [Header("In-Game UI")]
    public TMP_Text scoreText;

    [Header("Victory Scoreboard")]
    public TMP_Text finalScoreText_Victory;
    public TMP_Text highScoreText_Victory;
    public TMP_Text enemiesDefeatedText_Victory;
    public GameObject newRecordTextObject;

    [Header("Game Over Scoreboard")]
    public TMP_Text finalScoreText_GameOver;

    // === Biến theo dõi chỉ số ===
    private int score = 0;
    private int enemyCount;
    private int enemiesDefeatedCount = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Ẩn cả 2 panel khi bắt đầu
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (newRecordTextObject != null) newRecordTextObject.SetActive(false);

        // Đếm kẻ địch
        enemyCount = GameObject.FindGameObjectsWithTag("Enemies").Length;
        Debug.Log("[GameManager] Số lượng kẻ địch ban đầu: " + enemyCount);
        
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // Được gọi khi người chơi bị hạ gục
    public void PlayerDied()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            Debug.Log("[GameManager] GAME OVER!");
            ShowGameOverScreen();
        }
    }
    
    // Được gọi khi tiêu diệt hết kẻ địch
    public void EnemyDefeated()
    {
        enemyCount--;
        enemiesDefeatedCount++; // Tăng số lượng kẻ địch đã hạ gục để hiển thị
        Debug.Log("[GameManager] Kẻ địch bị hạ! Còn lại: " + enemyCount);

        if (enemyCount <= 0 && !isGameOver)
        {
            isGameOver = true;
            Debug.Log("[GameManager] Đã tiêu diệt hết kẻ địch! CHIẾN THẮNG!");
            ShowVictoryScreen();
        }
    }

    // === PHẦN MỚI: Hàm hiển thị màn hình chiến thắng ===
    private void ShowVictoryScreen()
    {
        Time.timeScale = 0f;
        if (victoryPanel != null) victoryPanel.SetActive(true);

        // Cập nhật text trên bảng chiến thắng
        if (finalScoreText_Victory != null) finalScoreText_Victory.text = "Score: " + score;
        if (enemiesDefeatedText_Victory != null) enemiesDefeatedText_Victory.text = "Enemies Defeated: " + enemiesDefeatedCount;

        // Kiểm tra và hiển thị điểm cao
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            if (newRecordTextObject != null) newRecordTextObject.SetActive(true);
        }
        if (highScoreText_Victory != null) highScoreText_Victory.text = "High Score: " + highScore;
    }

    // === PHẦN MỚI: Hàm hiển thị màn hình thua cuộc ===
    private void ShowGameOverScreen()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        
        // Cập nhật text trên bảng thua cuộc
        if (finalScoreText_GameOver != null) finalScoreText_GameOver.text = "Final Score: " + score;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Thay "MainMenu" bằng tên Scene menu chính của bạn
    }
}