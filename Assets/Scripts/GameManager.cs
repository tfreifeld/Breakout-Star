using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int numberOfLevels;
    public GameObject ballPrefab;
    [FormerlySerializedAs("pickupPrefab")] public GameObject powerUpPrefab;

    private AudioSource _audioSource;
    private AudioClip _loseClip;

    private int _paddleSize;
    private bool _hasGuns;
    private string CurrentScene { get; set; }


    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _scoreText.GetComponent<Text>().text = "Score: " + _score;
        }
    }

    public int BallsInPlay { get; set; } = 1;

    public bool IsWeakBricks { set; get; }

    public bool IsFireBall { get; set; }


    public int PaddleSize
    {
        get => _paddleSize;
        set
        {
            if (value >= 1 && value <= 6)
            {
                _paddleSize = value;
                _paddle.GetComponent<PaddleController>().ChangeSize(_paddleSize);
            }
        }
    }

    private int Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            _livesText.GetComponent<Text>().text = "Lives: " + _lives;
        }
    }

    public bool HasGuns
    {
        get => _hasGuns;
        set
        {
            _hasGuns = value;
            if (_hasGuns)
            {
                _paddle.GetComponent<PaddleController>().EnableGuns();
            }
            else
            {
                _paddle.GetComponent<PaddleController>().DisableGuns();
            }
        }
    }

    public bool GameBegun { get; private set; }

    private int _score;
    private int _lives;

    private GameObject _scoreText;
    private GameObject _livesText;
    private GameObject _gameOver;
    private GameObject _playAgainButton;
    private GameObject _quitButton;
    private GameObject _winner;
    private GameObject _finalScoreText;
    private GameObject _continueButton;
    private GameObject _instructions;
    private GameObject _paddle;

    private bool _debug;


    private void Awake()
    {
        _scoreText = GameObject.Find("Score");
        _livesText = GameObject.Find("Lives");
        _gameOver = GameObject.Find("GameOver");
        _playAgainButton = GameObject.Find("PlayAgain");
        _quitButton = GameObject.Find("Quit");
        _winner = GameObject.Find("Winner");
        _continueButton = GameObject.Find("Continue");
        _finalScoreText = GameObject.Find("FinalScore");
        _instructions = GameObject.Find("Instructions");
        _paddle = GameObject.Find("Paddle");

        _audioSource = GetComponent<AudioSource>();
        _loseClip = Resources.Load<AudioClip>("Audio/lose");


        _gameOver.SetActive(false);
        _playAgainButton.SetActive(false);
        _quitButton.SetActive(false);
        _winner.SetActive(false);

        CurrentScene = SceneManager.GetActiveScene().name;

        PaddleSize = 2;
        HasGuns = false;


        Score = PlayerPrefs.GetInt("score");
        Lives = PlayerPrefs.GetInt("lives");
    }

    private void Start()
    {
        if (CurrentScene == "Level 1")
        {
            GameBegun = false;
        }
        else
        {
            _instructions.SetActive(false);
            GameBegun = true;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !GameBegun)
        {
            _instructions.SetActive(false);
            GameBegun = true;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _debug = !_debug;
        }

        if (_debug)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                IsFireBall = !IsFireBall;
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                IsWeakBricks = !IsWeakBricks;
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                foreach (BallController controller in FindObjectsOfType<BallController>())
                {
                    controller.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.right;
                }
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                BoxCollider2D boxCollider2D = GameObject.Find("Floor").GetComponent<BoxCollider2D>();
                boxCollider2D.isTrigger = !boxCollider2D.isTrigger;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                PaddleSize++;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                PaddleSize--;
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                HasGuns = !HasGuns;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.SetInt("score", 0);
            PlayerPrefs.SetInt("lives", 3);
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void KillBall(GameObject ball)
    {
        Destroy(ball);


        if (BallsInPlay == 1)
        {
            BallsInPlay--;
            Lose();
        }
        else
        {
            BallsInPlay--;
        }
    }

    private void Lose()
    {
        Lives--;
        PaddleSize = 2;
        HasGuns = false;
        IsWeakBricks = false;
        IsFireBall = false;

        _audioSource.PlayOneShot(_loseClip);
        foreach (PowerUpController controller in FindObjectsOfType<PowerUpController>())
        {
            Destroy(controller.gameObject);
        }

        if (Lives != 0)
        {
            GameObject newBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);

            GameObject paddle = GameObject.Find("Paddle");
            newBall.transform.SetParent(paddle.transform);
            newBall.transform.position = paddle.transform.position;
            newBall.transform.Translate(new Vector3(0, 0.4f, 0));

            BallsInPlay = 1;
        }
        else
        {
            _gameOver.SetActive(true);
            _playAgainButton.SetActive(true);
            _quitButton.SetActive(true);

            int highScore = PlayerPrefs.GetInt("high score");
            if (Score > highScore)
            {
                PlayerPrefs.SetInt("high score", Score);
            }
        }
    }

    public void LevelWon()
    {
        foreach (BallController controller in FindObjectsOfType<BallController>())
        {
            Destroy(controller.gameObject);
        }

        foreach (PowerUpController powerUpController in FindObjectsOfType<PowerUpController>())
        {
            Destroy(powerUpController.gameObject);
        }

        _winner.SetActive(true);
        _finalScoreText.GetComponent<Text>().text = "Your Score: " + Score;
    }

    public void MakePowerUp(Vector3 position)
    {
        GameObject powerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);
        int kind = Random.Range(1, 11);
        PowerUpController controller = powerUp.GetComponent<PowerUpController>();
        switch (kind)
        {
            case 1:
                controller.SetKind("Balls");
                break;
            case 2:
                controller.SetKind("Fire");
                break;
            case 3:
                controller.SetKind("WeakBricks");
                break;
            case 4:
                controller.SetKind("Slow");
                break;
            case 5:
                controller.SetKind("Fast");
                break;
            case 6:
                controller.SetKind("Shrink");
                break;
            case 7:
                controller.SetKind("Expand");
                break;
            case 8:
                controller.SetKind("Guns");
                break;
            case 9:
                controller.SetKind("250");
                break;
            case 10:
                controller.SetKind("500");
                break;
        }
    }

    [UsedImplicitly]
    public void PlayAgain()
    {
        _playAgainButton.GetComponent<AudioSource>().Play();
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("lives", 3);
        SceneManager.LoadScene("Level 1");
    }

    [UsedImplicitly]
    public void Quit()
    {
        _quitButton.GetComponent<AudioSource>().Play();
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("lives", 3);
        SceneManager.LoadScene("Main Menu");
    }

    [UsedImplicitly]
    public void Continue()
    {
        _continueButton.GetComponent<AudioSource>().Play();

        PlayerPrefs.SetInt("score", Score);
        PlayerPrefs.SetInt("lives", Lives);


        int nextLevel = int.Parse(CurrentScene.Substring(CurrentScene.Length - 1)) + 1;

        if (nextLevel > numberOfLevels)
        {
            SceneManager.LoadScene("Main Menu");
        }
        else
        {
            string nextScene = "Level " + nextLevel;
            SceneManager.LoadScene(nextScene);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("lives", 3);
    }
}