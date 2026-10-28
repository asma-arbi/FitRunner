using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RunnerController : MonoBehaviour
{
    [Header("Vitesse")]
    public float baseSpeed = 8f;
    public float minSpeed = 3f;
    public float maxSpeed = 20f;
    public float sprintMultiplier = 1.5f;
    public float sprintDuration = 1.5f;
    public float sprintCooldown = 4f;

    [Header("Voies et saut")]
    public float laneDistance = 2f;
    public float jumpVelocity = 7f;

    Rigidbody rb;
    int lane = 1; // 0 gauche, 1 centre, 2 droite
    float currentSpeed;
    float sprintTimeLeft;
    float sprintReadyAt;

    public float Distance => transform.position.z;
    public float Speed => currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        currentSpeed = baseSpeed;
    }

    void OnEnable()
    {
        GameEvents.OnLaneChange += ChangeLane;
        GameEvents.OnJump += Jump;
        GameEvents.OnSprint += Sprint;
        GameEvents.OnEnduranceBoost += EnduranceBoost;
    }

    void OnDisable()
    {
        GameEvents.OnLaneChange -= ChangeLane;
        GameEvents.OnJump -= Jump;
        GameEvents.OnSprint -= Sprint;
        GameEvents.OnEnduranceBoost -= EnduranceBoost;
    }

    // Touches de test : elles émettent les mêmes événements que les capteurs
    void Update()
    {
        // Téléphone : tiers gauche = voie gauche, tiers droit = voie droite, milieu = saut
        var ts = Touchscreen.current;
        if (ts != null && ts.primaryTouch.press.wasPressedThisFrame)
        {
            float x = ts.primaryTouch.position.ReadValue().x / Screen.width;
            if (x < 0.33f) GameEvents.LaneChange(-1);
            else if (x > 0.66f) GameEvents.LaneChange(1);
            else GameEvents.Jump(1f);
        }

        // Éditeur : clavier
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.leftArrowKey.wasPressedThisFrame) GameEvents.LaneChange(-1);
        if (kb.rightArrowKey.wasPressedThisFrame) GameEvents.LaneChange(1);
        if (kb.spaceKey.wasPressedThisFrame) GameEvents.Jump(1f);
        if (kb.leftShiftKey.wasPressedThisFrame) GameEvents.Sprint();
    }

    void FixedUpdate()
    {
        if (sprintTimeLeft > 0f) sprintTimeLeft -= Time.fixedDeltaTime;
        float speed = currentSpeed * (sprintTimeLeft > 0f ? sprintMultiplier : 1f);
        float targetX = (lane - 1) * laneDistance;
        float vx = (targetX - rb.position.x) * 10f;
        rb.linearVelocity = new Vector3(vx, rb.linearVelocity.y, speed);
    }

    public void AddSpeed(float delta)
    {
        currentSpeed = Mathf.Clamp(currentSpeed + delta, minSpeed, maxSpeed);
    }

    void ChangeLane(int dir) => lane = Mathf.Clamp(lane + dir, 0, 2);

    bool IsGrounded() =>
        Physics.Raycast(transform.position, Vector3.down, 1.1f, ~0, QueryTriggerInteraction.Ignore);

    void Jump(float power)
    {
        if (!IsGrounded()) return;
        power = Mathf.Clamp01(power);
        float v = Mathf.Lerp(jumpVelocity * 0.6f, jumpVelocity, power);
        var vel = rb.linearVelocity;
        vel.y = v;
        rb.linearVelocity = vel;
    }

    void Sprint()
    {
        if (Time.time < sprintReadyAt) return;
        sprintTimeLeft = sprintDuration;
        sprintReadyAt = Time.time + sprintCooldown;
    }

    void EnduranceBoost(float amount) => AddSpeed(amount * 2f);
}