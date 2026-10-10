using NUnit.Framework.Constraints;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum EFirePiranhaPlantState : byte
{
    Unknown,
    Hiding,
    AnimatingUp,
    Active,
    Firing,
    FiringDelay,
    AnimatingDown
}

public enum EFirePiranhaDirection : byte
{
    Unknown,
    LeftDown,
    RightDown,
    LeftUp,
    RightUp
}
public class FirePiranhaPlant : Enemy
{
    private SpriteRenderer spriteRenderer;
    public EnemySettings settings;
    private Animator animator;
    private EFirePiranhaDirection direction = EFirePiranhaDirection.Unknown;
    private EFirePiranhaPlantState state = EFirePiranhaPlantState.Unknown;
    private Vector2 hidingLocation = Vector2.zero;
    private Vector2 activeLocation = Vector2.zero;
    private float holdTimer = 0.0f;
    private float firingTimer = 0.5f;
    private float firingDelayTimer = 1.5f;
    private float animationTimer = 0.0f;
    private bool isFiring = false;

    public EFirePiranhaPlantState State
    {
        get { return state; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the enemy type
        enemyType = EEnemyType.FirePiranhaPlant;

        animator = GetComponent<Animator>(); // setting the animator to the animator tool

        spriteRenderer = GetComponent<SpriteRenderer>(); // Creating a variable to flip the sprite
        
        // Capture the starting location (hiding) and from that calculate the active (on-screen) location
        hidingLocation = transform.position;
        activeLocation = hidingLocation + new Vector2(0.0f, settings.FirePiranhaPlantOffsetY);

        // Set the state to hiding
        SetState(EFirePiranhaPlantState.Hiding);

    }

    // Update is called once per frame
    void Update()
    {
        if (state == EFirePiranhaPlantState.Hiding)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EFirePiranhaPlantState.AnimatingUp);
            }
        }
        else if (state == EFirePiranhaPlantState.AnimatingUp)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.FirePiranhaPlantAnimationDuration);
            float locationX = Mathf.Lerp(hidingLocation.x, activeLocation.x, pct);
            float locationY = Mathf.Lerp(hidingLocation.y, activeLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EFirePiranhaPlantState.Active);
            }
        }
        else if (state == EFirePiranhaPlantState.Active)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EFirePiranhaPlantState.Firing);
            }
        }

        else if (state == EFirePiranhaPlantState.Firing)
        {
            firingTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (firingTimer <= 0.0f)
            {
                firingTimer = 0.0f;

                // Get Mario's location
                Vector2 marioFireballLocation = Game.Instance.MarioGameObject.transform.position;

                // Where to spawn the fireball
                Vector2 fireballLocation = transform.position + new Vector3(0.0f, 1.5f, 0.0f);

                // Velocity for the fireball
                Vector2 velocity = (marioFireballLocation - fireballLocation).normalized * 3.0f; // Setting it 3 pixels per frame to match the game.

                //Spawning the Fireball
                Game.Instance.SpawnFireball(fireballLocation, velocity);

                SetState(EFirePiranhaPlantState.FiringDelay);
            }
        }

        else if (state == EFirePiranhaPlantState.FiringDelay)
        {
            firingDelayTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (firingDelayTimer <= 0.0f)
            {
                firingDelayTimer = 0.0f;
                SetState(EFirePiranhaPlantState.AnimatingDown);
            }
        }
        
        else if (state == EFirePiranhaPlantState.AnimatingDown)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.FirePiranhaPlantAnimationDuration);
            float locationX = Mathf.Lerp(activeLocation.x, hidingLocation.x, pct);
            float locationY = Mathf.Lerp(activeLocation.y, hidingLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EFirePiranhaPlantState.Hiding);
            }
        }

        // Get Mario's location
        Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;

        bool isRight = marioLocation.x > transform.position.x;
        bool isUp = marioLocation.y > transform.position.y;

        if (isRight && isUp) // Looking up Right if Mario is up Right
        {
            SetDirection(EFirePiranhaDirection.RightUp);
        }
        else if (!isRight && isUp) // Looking up Left if Mario is up Left
        {
            SetDirection(EFirePiranhaDirection.LeftUp);
        }
        else if (isRight && !isUp) // Looking down Right if Mario is down right
        {
            SetDirection(EFirePiranhaDirection.RightDown);
        }
        else if (!isRight && !isUp) // Looking sown left if Mario is down left
        {
            SetDirection(EFirePiranhaDirection.LeftDown);
        }
    }

    private void SetState(EFirePiranhaPlantState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EFirePiranhaPlantState.Hiding)
            {
                transform.position = hidingLocation;
                holdTimer = UnityEngine.Random.Range(settings.FirePiranhaPlantHiddenDurationMin, settings.FirePiranhaPlantHiddenDurationMax);
            }
            else if (state == EFirePiranhaPlantState.AnimatingUp)
            {
                // Get Mario's location
                Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;

                // Check if Mario is on top of the pipe, if he is, don't spawn the piranha plant
                bool checkY = Mathf.Clamp(marioLocation.x, activeLocation.x - 1.0f, activeLocation.x + 1.0f) == marioLocation.x;
                if (checkY && Mathf.Abs(activeLocation.y - marioLocation.y) <= 0.51f)
                {
                    SetState(EFirePiranhaPlantState.Hiding);
                    return;
                }

                animationTimer = settings.FirePiranhaPlantAnimationDuration;
            }
            else if (state == EFirePiranhaPlantState.Active)
            {
                transform.position = activeLocation;
                holdTimer = settings.FirePiranhaPlantActiveDuration;
            }
            else if (state == EFirePiranhaPlantState.Firing)
            {
                isFiring = true;
                firingTimer = 0.5f;
            }
            else if (state == EFirePiranhaPlantState.FiringDelay)
            {
                isFiring = false;
                firingDelayTimer = 1.5f;
            }
            else if (state == EFirePiranhaPlantState.AnimatingDown)
            {
                animationTimer = settings.FirePiranhaPlantAnimationDuration;
            }
        }
    }
    private void SetDirection(EFirePiranhaDirection newDirection)
    {
        direction = newDirection; // A parameter that recieves calls from the enum
        UpdateAnimator();
    }
    private void UpdateAnimator()
    {
        if (isFiring)
        {
            if(direction == EFirePiranhaDirection.LeftDown || direction == EFirePiranhaDirection.RightDown)
            {
                animator.Play("FirePiranhaDown-Open");
            }
            else
            {
                animator.Play("FirePiranhaUp-Open");
            }
            return;
        }

        if (direction == EFirePiranhaDirection.LeftDown)
        {
            animator.Play("FirePiranhaDown-Closed");
            spriteRenderer.flipX = false;
        }
        else if (direction == EFirePiranhaDirection.LeftUp)
        {
            animator.Play("FirePiranhaUp-Closed");
            spriteRenderer.flipX = false;
        }
        else if (direction == EFirePiranhaDirection.RightDown)
        {
            animator.Play("FirePiranhaDown-Closed");
            spriteRenderer.flipX = true;
        }
        else if (direction == EFirePiranhaDirection.RightUp)
        {
            animator.Play("FirePiranhaUp-Closed");
            spriteRenderer.flipX = true;
        }
    }
}

