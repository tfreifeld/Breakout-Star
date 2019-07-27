using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 100f;
    public float lowerSpeedLimit = 4f;
    public float upperSpeedLimit = 24f;
    public float horizontalTimeLimit;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;

    private AudioClip _paddleHitClip;
    private AudioClip _wallHitClip;

    public bool BallFreed { get;  set; }
    private float _horizontalTimer;


    // Start is called before the first frame update
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        _paddleHitClip = Resources.Load<AudioClip>("Audio/paddleHit");
        _wallHitClip = Resources.Load<AudioClip>("Audio/wallHit");

        _horizontalTimer = horizontalTimeLimit;
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Fire1") && !BallFreed && _gameManager.GameBegun)
        {
            transform.parent = null;
            BallFreed = true;
            _rigidbody2D.simulated = true;
            _rigidbody2D.velocity = Vector2.up * initialSpeed;
        }
        else if (BallFreed)
        {
            Vector2 velocity = _rigidbody2D.velocity;

            if (velocity.magnitude < lowerSpeedLimit || velocity.magnitude > upperSpeedLimit)
            {
                float newMagnitude =
                    (velocity.magnitude < lowerSpeedLimit) ? lowerSpeedLimit : upperSpeedLimit;

                velocity.Normalize();
                velocity *= newMagnitude;
            }

            if (Math.Abs(velocity.y) < 0.1f)
            {
                if (_horizontalTimer <= 0f)
                {
                    _horizontalTimer = horizontalTimeLimit;
                    velocity.y -= 0.25f;
                    Debug.Log("Ball horizontal fixation fixed");
                }
                else
                {
                    _horizontalTimer -= Time.deltaTime;
                }
            }
            else if (!Mathf.Approximately(_horizontalTimer, horizontalTimeLimit))
            {
                _horizontalTimer = horizontalTimeLimit;
            }

            _rigidbody2D.velocity = velocity;
//            Debug.Log(velocity.magnitude);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Paddle")
        {
            // (ball position - paddle position) / paddle width
            float x = (_rigidbody2D.position.x - collision.rigidbody.position.x) / collision.collider.bounds.size.x;

            // Calculate direction, set length to 1
            Vector2 dir = new Vector2(x, 1).normalized;

            // Set Velocity with dir * speed
            _rigidbody2D.velocity = dir * _rigidbody2D.velocity.magnitude;

            _audioSource.PlayOneShot(_paddleHitClip);
        }
        else if (collision.gameObject.name == "Tilemap")
        {
            BrickController controller = collision.gameObject.GetComponent<BrickController>();

            controller.HandleBrickCollision(collision, _rigidbody2D.velocity.magnitude);
        }
        else // walls and ceiling
        {
            _audioSource.PlayOneShot(_wallHitClip);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Floor")
        {
            _gameManager.KillBall(gameObject);
        }
    }
}