using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public int numberOfLevels;
    public GameObject levelText;
    public GameObject image;
    
    private AudioSource _audioSource;
    private Text _levelText;
    private Image _image;

    private int _level;


    private int Level
    {
        get => _level;
        set
        {
            if (value <= numberOfLevels && value >= 1)
            {
                _level = value;
                _levelText.text = "Level " + _level;
                _image.sprite = Resources.Load<Sprite>("Pictures/level" + value);
            }
        }
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _image = image.GetComponent<Image>();
    }

    private void Start()
    {
        _levelText = levelText.GetComponent<Text>();
        Level = 1;
    }

    [UsedImplicitly]
    public void OnNextButtonClicked()
    {
        _audioSource.Play();
        Level++;
    }

    [UsedImplicitly]
    public void OnPreviousButtonClicked()
    {
        _audioSource.Play();
        Level--;
    }

    [UsedImplicitly]
    public void OnBackButtonClicked()
    {
        _audioSource.Play();
        SceneManager.LoadScene("Main Menu");
    }

    [UsedImplicitly]
    public void OnGoButtonClicked()
    {
        _audioSource.Play();
        SceneManager.LoadScene(_levelText.text);
    }
}