using UnityEngine;

public class ShotController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Launch()
    {
        _rigidbody2D.AddForce(Vector2.up * 100f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        if (collision.gameObject.name == "Tilemap")
        {
            BrickController controller = collision.gameObject.GetComponent<BrickController>();


            controller.HandleBrickCollision(collision, _rigidbody2D.velocity.magnitude);
        }
    }
}