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

public enum EFirePiranhaPlantDir : byte
{
    LeftDown,
    RightDown,
    LeftUp,
    RightUp
}
public class FirePiranhaPlant : Enemy
{
    public EnemySettings settings;
    private Animator animator;

    private EFirePiranhaPlantState state = EFirePiranhaPlantState.Unknown;
    private Vector2 hidingLocation = Vector2.zero;
    private Vector2 activeLocation = Vector2.zero;
    private float holdTimer = 0.0f;
    private float animationTimer = 0.0f;

    public EFirePiranhaPlantState State
    {
        get { return state; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the enemy type
        enemyType = EEnemyType.FirePiranhaPlant;

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
                SetState(EFirePiranhaPlantState.AnimatingDown);
            }
        }
       
        else if (state == EFirePiranhaPlantState.Firing)
        {
        }

        else if (state == EFirePiranhaPlantState.FiringDelay)
        {
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

            }
            else if (state == EFirePiranhaPlantState.FiringDelay)
            {

            }
            else if (state == EFirePiranhaPlantState.AnimatingDown)
            {
                animationTimer = settings.FirePiranhaPlantAnimationDuration;
            }
        }
    }
}
