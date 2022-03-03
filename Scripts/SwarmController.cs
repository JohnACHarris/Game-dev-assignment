using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{

    public float maxSpeed = 50f;
    public float minSpeed = 25f;
    public float separationStrength = 50f;
    public float cohesionStrengh = 2f;
    public float alignmentStrength = 10f;
    public float targetStrength = 1f;
    public float repulsionRadius = 6f;
    public float neighbourhoodRadius = 20f;

    public float turnTime = 0.01f;
    public float turnSpeed = 50.0f;

    float sqrRepulsionRadius, sqrMaxSpeed, sqrMinSpeed;

    Vector3 velocityInput;
    Vector3 velocityCurrent;
    Vector3 centre = Vector3.zero;
    Vector3 heading = Vector3.forward;
    Vector3 repulsion = Vector3.zero;
    public Collider myCollider;
    public Transform target;
    private Vector3 velRef = Vector3.zero;


    List<Transform> neighbours;

    int layerMask = 1 << 6;

    public bool showGizmos = false;

    // Start is called before the first frame update
    void Start()
    {
        //square these values for use later
        sqrRepulsionRadius = repulsionRadius * repulsionRadius;
        sqrMaxSpeed = maxSpeed * maxSpeed;
        sqrMinSpeed = minSpeed * minSpeed;

        //set current and input vel starting values
        velocityCurrent = transform.forward * minSpeed;
        velocityCurrent = transform.forward * (minSpeed + 5f);
    }

    // Update is called once per frame
    void Update()
    {

        //check situation
        neighbours = GetNeighbours();
        (centre, heading, repulsion)= GetSituation(neighbours);
        //apply rules
        //update velocity 
        velocityUpdate();

    }

    void FixedUpdate()
    {
        Move(velocityCurrent);
    }

    void velocityUpdate()
    {
        if(neighbours.Count != 0)
        {
            //apply cohesion
            velocityInput += ((centre - transform.position) * cohesionStrengh * Time.deltaTime);

            //apply alignment
            velocityInput += (transform.InverseTransformDirection(heading) * alignmentStrength * Time.deltaTime);

            //apply repulsion
            velocityInput += (transform.InverseTransformDirection(repulsion) * separationStrength * Time.deltaTime);

            
        }
        
        //apply target attraction
        velocityInput += ((target.position - transform.position) * targetStrength * Time.deltaTime);
        velocityInput = Vector3.ClampMagnitude(velocityInput, maxSpeed);

        velocityCurrent = Vector3.SmoothDamp(velocityCurrent, velocityInput, ref velRef, turnTime, turnSpeed);
        
        
        //cap max and min speed
        if(velocityCurrent.sqrMagnitude > sqrMaxSpeed)
        {
            velocityCurrent = velocityCurrent.normalized * maxSpeed;
        }
        else if (velocityCurrent.sqrMagnitude < sqrMinSpeed)
        {
            velocityCurrent = velocityCurrent.normalized * minSpeed;
        }

        

    }


    void Move(Vector3 velocity)
    {
        //change heading
        transform.forward = velocity;
        //translate
        transform.position += velocity * Time.deltaTime;
    }

    List<Transform> GetNeighbours()
    {
        List<Transform> neighbours = new List<Transform>();
        Collider[] nColliders = Physics.OverlapSphere(transform.position, neighbourhoodRadius, layerMask);
        foreach(Collider x in nColliders)
        {
            if(x != myCollider)
            {
                neighbours.Add(x.transform);
            }
        }
        return neighbours;
    }

    (Vector3, Vector3, Vector3) GetSituation(List<Transform> neighbours) 
    {

        Vector3 sumT = Vector3.zero;
        Vector3 sumH = Vector3.zero;
        Vector3 sumAv = Vector3.zero;
        Vector3 avoidVector, avoidForce;
        if (neighbours.Count <= 0)
        {
            return (sumT, sumH, sumAv);
        }

        int n = 0;
        int nAV = 0;
        foreach(Transform t in neighbours)
        {
            sumT += t.position;
            sumH += t.forward;
            if ((t.position - transform.position).sqrMagnitude <= sqrRepulsionRadius)
            {
                //calculate avoidance force such that its magnitude increases with proximity rather than distance
                avoidVector = (transform.position - t.position);
                avoidForce = avoidVector.normalized * (1 / avoidVector.magnitude);

                sumAv += avoidForce;
                nAV++;
            }
            n++;
        }
        if(nAV <= 0)
        {
            return (sumT / n, ((sumH / n).normalized), sumAv);
        }
        return (sumT/n, ((sumH/n).normalized), sumAv/nAV);
    }

    

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "SwarmMissile":
                break;
            default:
                kill();
                break;
        }
        
    }

    public void kill()
    {
        Instantiate(Resources.Load("JMO Assets/WarFX/_Effects/Explosions/WFX_Explosion"), transform.position, transform.rotation);
        Destroy(gameObject);
    }


    void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.DrawWireSphere(transform.position, neighbourhoodRadius);
            Gizmos.DrawRay(transform.position, (centre - transform.position));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, centre);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(centre, heading * 5);
            Gizmos.DrawWireSphere(transform.position, repulsionRadius);

            if (Application.isPlaying)
            {
                Debug.Log(neighbours.Count);
                Debug.Log(velocityCurrent.magnitude);
            }
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawRay(transform.position, (transform.TransformDirection(Vector3.forward)) * 5);

    }


}
