using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour
{

    public float startSpeed = 100.0f;
    public float maxSpeed = 500.0f;
    public float minSpeed = 50.0f;
    public float inputAcceleration = 50.0f;
    public float maxAcceleration = 40.0f;

    public float pitchSensetivity = 1.2f;
    public float yawSensetivity = 0.3f;
    public float rollRate = 0.5f;

    public float currentSpeed;
    private float desiredSpeed, yawInput, pitchInput;
    private float velRef = 0.0f;

    private Vector3 rotate = new Vector3();

    float missileXDisplacement = 2.0f;
    float missileZDisplacement = 1.0f;
    int missileAmmo = 30;
    float missileFireRate = 8.0f;
    float nextFireMissile = 0f;
    Vector3 firePosMissile;

    float bulletZDisplacement = 3.0f;
    float bulletFireRate = 20.0f;
    float nextFireBullet = 0f;
    Vector3 firePosBullet;
    RaycastHit hitInfo;

    public Text missileCounter;

    public UIScript UI;

    public ParticleSystem muzzleFlash;

    GameObject bulletHit;
    Object missile;



    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = startSpeed;
        desiredSpeed = startSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        bulletHit = Resources.Load("JMO Assets/WarFX/_Effects/Bullet Impacts/WFX_BImpact Metal") as GameObject;
        missile = Resources.Load("PlayerMissile");
    }

    // Update is called once per frame
    void Update()
    {
        RPYUpdate();
        SpeedUpdate();
        CheckFire();
        HUDUpdate();
    }

    void RPYUpdate()
    {
        //measure mouse input
        yawInput = Input.GetAxis("Mouse X");
        pitchInput = Input.GetAxis("Mouse Y");

        //record and scale mouse input
        rotate = (Vector3.up * yawInput * yawSensetivity) + (Vector3.left * pitchInput * pitchSensetivity);

        //read Q/E/A/D roll / fast roll input
        if(Input.GetKey(KeyCode.Q))
        {
            //roll left
            rotate += (Vector3.forward * rollRate);
        }
        else if(Input.GetKey(KeyCode.E))
        {
            //roll right
            rotate -= (Vector3.forward * rollRate);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            //roll right
            rotate += (Vector3.forward * rollRate * 2f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            //roll right
            rotate -= (Vector3.forward * rollRate * 2f);
        }

        //rotate player
        transform.Rotate(rotate * Time.deltaTime * 100);

    }

    void SpeedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            desiredSpeed += inputAcceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            desiredSpeed -= inputAcceleration * Time.deltaTime;
        }

        desiredSpeed = Mathf.Clamp(desiredSpeed, minSpeed, maxSpeed);

        currentSpeed = Mathf.SmoothDamp(currentSpeed, desiredSpeed, ref velRef, 0.2f, maxAcceleration);

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    void HUDUpdate()
    {
        missileCounter.text = "Ammo: " + missileAmmo;
    }

    void CheckFire()
    {
        //gun
        //check M1, if yes then make boolet
        if (Input.GetMouseButton(0) && (Time.time >= nextFireBullet))
        {
            nextFireBullet = Time.time + (1 / bulletFireRate);
            firePosBullet = (transform.position + (transform.forward * bulletZDisplacement));
            if (Physics.Raycast(firePosBullet, transform.TransformDirection(Vector3.forward), out hitInfo))
            {
                switch (hitInfo.collider.gameObject.tag)
                {
                    case "Enemy":
                        hitInfo.collider.gameObject.GetComponent<HeliController>().health -= 1;
                        break;
                    case "SwarmMissile":
                        hitInfo.collider.gameObject.GetComponent<SwarmController>().kill();
                        break;
                    case "EnemyTruck":
                        hitInfo.collider.gameObject.GetComponent<TruckBehaviour>().health -= 1;
                        break;
                    default:
                        break;
                }

                Instantiate(bulletHit, hitInfo.point, Quaternion.FromToRotation(Vector3.forward, hitInfo.normal));
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            muzzleFlash.Play();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            muzzleFlash.Stop();
        }




        //check M2, if yes then missile
            if (Input.GetMouseButtonDown(1) && (Time.time >= nextFireMissile) && (missileAmmo > 0))
        {
            nextFireMissile = Time.time + (1 / missileFireRate);
            missileAmmo--;

            //instantiate missile
            firePosMissile = (transform.right * missileXDisplacement) + (transform.forward * missileZDisplacement);
            Instantiate(missile, transform.position + firePosMissile, transform.rotation);

            missileXDisplacement = -missileXDisplacement;
        }

    }

    
    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "SwarmMissile":
                UI.playerHealth--;
                break;

            case "PlayerMissile":
                break;

            default:
                UI.playerHealth = 0;
                break;
        }
    }

    
}
