using UnityEngine;

public enum KinematicState { Seek, Flee, Wandering}

public class Movement : MonoBehaviour
{
    public Transform target;

    public float maxTimeToRotate = 5f;

    private float currentTimeToROtate = 0f;

    public float maxRotation = 50f;

    public float maxSpeed = 2f;

    public float radius = 1f;

    public float timeToTarget = 1f;

    public KinematicState kinematicState = KinematicState.Seek;

    private class KinematicOutput
    {
        public Vector3 velocity { get; set; }
        public float rotation { get; set; }

        public void Reset()
        {
            velocity = Vector3.zero;
            rotation = 0f;
        }
    }

    private KinematicOutput kinematicOutput = new KinematicOutput { velocity = Vector3.zero, rotation = 0f };

    private Rigidbody2D character;

    private void Awake()
    {
        character = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(kinematicState == KinematicState.Wandering)
        {
            currentTimeToROtate += Time.fixedDeltaTime;

            getWanderingOutput();
        }
        else
        {
            getKinematicOutput();
        }

        character.transform.position += kinematicOutput.velocity * Time.fixedDeltaTime;

        character.transform.rotation = Quaternion.AngleAxis(kinematicOutput.rotation, Vector3.forward);
    }
    
    private void getWanderingOutput()
    {
        // Move in direction of current orientation
        kinematicOutput.velocity = character.transform.right * maxSpeed;
        // Modify current orientation
        if(currentTimeToROtate >= maxTimeToRotate)
        {
            kinematicOutput.rotation = Random.Range(-1f, 1f) * maxRotation;

            currentTimeToROtate = 0;
        }
    }

    private void getKinematicOutput()
    {
        // Get Direction of movement
        kinematicOutput.velocity = kinematicState == KinematicState.Seek ?  target.position - character.transform.position : character.transform.position - target.position  ;

        if(kinematicOutput.velocity.magnitude < radius)
        {
            kinematicOutput.Reset();
            return;
        }

        kinematicOutput.velocity /= timeToTarget;

        if(kinematicOutput.velocity.magnitude > maxSpeed)
        {
            // Normalize vector 
            kinematicOutput.velocity = kinematicOutput.velocity.normalized;
            // Max speed 
            kinematicOutput.velocity *= maxSpeed;
        }

        // Calculate angle of rotation
        kinematicOutput.rotation = CacluateOrientation(kinematicOutput.velocity);
    }
    
    private float CacluateOrientation(Vector3 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

}
