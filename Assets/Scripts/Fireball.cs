using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector2 velocity;
    private Animator animator;
    private float lifetime = 0; // Creating a lifetime variable
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime); // Using deltatime so the fireball doesn't go insanely quick
        
        lifetime += Time.deltaTime; // Tying the lifeline to deltatime so after 12 seconds, it gets destroyed
        if (lifetime >= 12.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }
}
