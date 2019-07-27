using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private GameManager _gameManager;
    private AudioSource _audioSource;

    private AudioClip _spawnedClip;
    private string _kind;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        _spawnedClip = Resources.Load<AudioClip>("Audio/powerUpSpawned");
        
    }

    private void Start()
    {
        SetArt();
        ApplyForce();
        _audioSource.PlayOneShot(_spawnedClip);
    }

    private void SetArt()
    {
        var newSprite = Resources.Load<Sprite>("Sprites/" + _kind);
        GetComponent<SpriteRenderer>().sprite = newSprite;
        GetComponent<Animator>().runtimeAnimatorController =
            Resources.Load<RuntimeAnimatorController>("Animators/" + _kind + "PowerUpController");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Paddle")
        {
            Destroy(gameObject);
            _gameManager.Score += 500;
            ActivatePowerUp();
        }
        else if (other.gameObject.name == "Floor")
        {
            Destroy(gameObject);
        }
    }

    private void ActivatePowerUp()
    {
        switch (_kind)
        {
            case "Balls":
                ActivateExtraBalls();
                break;
            case "Fire":
                ActivateFireBall();
                break;
            case "WeakBricks":
                ActivateWeakBricks();
                break;
            case "Slow":
                ActivateSpeedChange(false);
                break;
            case "Fast":
                ActivateSpeedChange(true);
                break;
            case "Shrink":
                ActivateShrinkPaddle();
                break;
            case "Expand":
                ActivateExpandPaddle();
                break;
            case "Guns":
                ActivateGuns();
                break;
            case "250":
                _gameManager.Score += 250;
                break;
            case "500":
                _gameManager.Score += 500;
                break;
        }
    }

    private void ActivateGuns()
    {
        _gameManager.HasGuns = true;
    }

    private void ActivateShrinkPaddle()
    {
        _gameManager.PaddleSize--;
    }

    private void ActivateExpandPaddle()
    {
        _gameManager.PaddleSize++;
    }

    private void ActivateFireBall()
    {
        Sprite newSprite = Resources.Load<Sprite>("Sprites/fireBall");
        foreach (BallController controller in FindObjectsOfType<BallController>())
        {
            controller.gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        }

        _gameManager.IsWeakBricks = false;
        _gameManager.IsFireBall = true;
    }

    private void ActivateWeakBricks()
    {
        Sprite newSprite = Resources.Load<Sprite>("Sprites/weakBricksBall");
        foreach (BallController controller in FindObjectsOfType<BallController>())
        {
            controller.gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        }

        _gameManager.IsFireBall = false;
        _gameManager.IsWeakBricks = true;
    }

    private void ActivateSpeedChange(bool isFast)
    {
        foreach (BallController controller in FindObjectsOfType<BallController>())
        {
            float newSpeed = isFast ? 12f : 4f;
            Rigidbody2D rb = controller.gameObject.GetComponent<Rigidbody2D>();
            Vector2 normalizedVelocity = rb.velocity.normalized;
            Vector2 newVelocity = normalizedVelocity * newSpeed;
            rb.velocity = newVelocity;
        }
    }

    private void ActivateExtraBalls()
    {
        if (_gameManager.BallsInPlay < 16)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/ball");
            ;
            if (_gameManager.IsFireBall)
            {
                sprite = Resources.Load<Sprite>("Sprites/fireBall");
            }
            else if (_gameManager.IsWeakBricks)
            {
                sprite = Resources.Load<Sprite>("Sprites/weakBricksBall");
            }

            foreach (BallController controller in FindObjectsOfType<BallController>())
            {
                Rigidbody2D rb = controller.gameObject.GetComponent<Rigidbody2D>();
                Vector2 position = rb.position;
                float speed = rb.velocity.magnitude;

                for (int i = 0; i < 3; i++)
                {
                    GameObject newBall = Instantiate(_gameManager.ballPrefab, position, Quaternion.identity);

                    newBall.GetComponent<SpriteRenderer>().sprite = sprite;

                    float x = Random.Range(0f, 1f);
                    float y = Random.Range(0f, 1f);
                    Vector2 velocity = new Vector2(x, y).normalized * speed;

                    Rigidbody2D newRb = newBall.GetComponent<Rigidbody2D>();
                    newRb.velocity = velocity;
                    newRb.simulated = true;

                    newBall.GetComponent<BallController>().BallFreed = true;
                }

                _gameManager.BallsInPlay += 3;
            }
        }
    }

    private void ApplyForce()
    {
        Rigidbody2D pickupRb = GetComponent<Rigidbody2D>();
        float xForce = Random.Range(100f, 300f) *
                       (Random.Range(0f, 1f) > 0.5 ? 1 : -1);

        float yForce = Random.Range(100f, 300f);

        pickupRb.AddForce(new Vector2(xForce, yForce));
    }

    public void SetKind(string kind)
    {
        _kind = kind;
    }
}