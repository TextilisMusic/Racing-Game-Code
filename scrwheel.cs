using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrwheel : MonoBehaviour
{
    public Rigidbody rb;
    public Transform wheelTransform; // Transform for the wheel

    [Header("Suspension")]
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;
    public float motorForce;
    public float frictionCoefficient; // Add a friction coefficient

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;
    private float frictionForce; // Variable for friction force

    private Vector3 suspensionForce;
    private Vector3 frictionForceVector; // Vector for friction force

    [Header("Wheel")]
    public float wheelRadius;
    public float wheelRotationSpeed;
    public float rotationMultiplier;

    

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
       
    }

    void FixedUpdate()
    {
        float direction = Mathf.Sign(Vector3.Dot(rb.velocity, transform.forward));

        // Calculate wheel rotation speed based on direction
        wheelRotationSpeed = direction * rb.velocity.magnitude * rotationMultiplier;

        if (rb.velocity.magnitude < .5f)
        {
            wheelRotationSpeed = 0f;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength - wheelRadius))
        {
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;
            rb.AddForceAtPosition(suspensionForce, hit.point);

            // Calculate and apply friction force
            frictionForce = frictionCoefficient * suspensionForce.magnitude;
            frictionForceVector = -rb.velocity.normalized * frictionForce;
            rb.AddForce(frictionForceVector);

            // Update wheel transform position
            wheelTransform.position = hit.point + (transform.up * wheelRadius);
        }

        // Rotate wheel for movement
        wheelTransform.Rotate(Vector3.right, wheelRotationSpeed * Time.fixedDeltaTime);


        

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * maxLength);

        Gizmos.color = Color.green;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength - wheelRadius))
        {
            Vector3 suspensionPos = hit.point;
            Gizmos.DrawWireSphere(suspensionPos, 0.1f);
        }
    }
}