using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float currentSpeed = 5f;
    public AnimationCurve turnSpeedOverVelocity = AnimationCurve.Linear(0, 1, 60, 0.2f); // Set this in the inspector
    public float maxTurnSpeed = 180f; // Degrees per second at 0 speed

    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
    void OnDrawGizmos()
    {
        //AnimationCurveDebugVisualizer.Visualise(turnSpeedOverVelocity, currentSpeed, transform.position + Vector3.up * 2f);
    }

    void Update()
    {
        // Get world point under mouse
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetDirection = (hit.point - transform.position);
            targetDirection.y = 0; // Ignore vertical axis

            if (targetDirection.sqrMagnitude > 0.001f)
            {
                float speedFactor = turnSpeedOverVelocity.Evaluate(currentSpeed);
                float turnSpeed = maxTurnSpeed * speedFactor;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    turnSpeed * Time.deltaTime
                );
            }
        }
    }

    void FixedUpdate()
    {
        // Compute this frame's forward velocity
        Vector3 forwardVelocity = transform.forward * currentSpeed;

        // Preserve existing velocity in other directions
        Vector3 newVelocity = forwardVelocity + Vector3.ProjectOnPlane(rb.linearVelocity, transform.forward);

        rb.linearVelocity = newVelocity;
    }
}