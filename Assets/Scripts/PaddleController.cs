using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 3f;
    public GameObject projectile;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;

    private AudioClip _powerUpClip;

    private float _timeLastShoot;

    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _powerUpClip = Resources.Load<AudioClip>("Audio/powerUpCaught");
        _timeLastShoot = 0;
    }

    private void FixedUpdate()
    {
        float input = Input.GetAxis("Horizontal");
        
        
        Vector2 position = _rigidbody2D.position;
        Vector2 move = new Vector2(input, 0);

        position += speed * Time.deltaTime * move;

        _rigidbody2D.MovePosition(position);

        if (_gameManager.HasGuns)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (_timeLastShoot <= 0)
                {
                    _timeLastShoot = 0.25f;
                    Shoot();
                }
            }

            _timeLastShoot -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        Transform rightTransform = transform.GetChild(0);
        Transform leftTransform = transform.GetChild(1);

        GameObject rightProjectile =
            Instantiate(projectile, rightTransform.position, Quaternion.identity);
        GameObject leftProjectile =
            Instantiate(projectile, leftTransform.position, Quaternion.identity);

        ShotController leftController = rightProjectile.GetComponent<ShotController>();
        ShotController rightController = leftProjectile.GetComponent<ShotController>();

        leftController.Launch();
        rightController.Launch();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "PowerUp(Clone)")
        {
            _audioSource.PlayOneShot(_powerUpClip);
        }
    }


    public void ChangeSize(int size)
    {
        Vector3 scale = transform.localScale;

        switch (size)
        {
            case 1:
                scale.x = 0.5f;
                break;
            case 2:
                scale.x = 0.8f;
                break;
            case 3:
                scale.x = 1.1f;
                break;
            case 4:
                scale.x = 1.4f;
                break;
            case 5:
                scale.x = 1.7f;
                break;
            case 6:
                scale.x = 2f;
                break;
        }

        transform.localScale = scale;
    }

    public void EnableGuns()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Animator animator = GetComponent<Animator>();

        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/PaddleGuns");
        animator.runtimeAnimatorController =
            Resources.Load<RuntimeAnimatorController>("Animators/PaddleGunsController");
    }

    public void DisableGuns()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Animator animator = GetComponent<Animator>();

        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Paddle");
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/PaddleController");
    }
}