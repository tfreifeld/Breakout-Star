using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private AudioSource _audioSource;
    public GameObject highScoreText;


    private int _highScore;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _highScore =  PlayerPrefs.GetInt("high score");
        highScoreText.GetComponent<Text>().text = "High Score: " + _highScore;
    }

    [UsedImplicitly]
    public void OnStartButtonClicked()
    {
        _audioSource.Play();
        SceneManager.LoadScene("Level 1");
    }

    [UsedImplicitly]
    public void OnQuitButtonClicked()
    {
        _audioSource.Play();
        Application.Quit(0);
    }

    [UsedImplicitly]
    public void OnLevelSelectButtonClicked()
    {
        _audioSource.Play();
        SceneManager.LoadScene("Level Select");
    }
}