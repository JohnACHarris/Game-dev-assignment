using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMissiles : MonoBehaviour
{

    public float fireDelay = 30f;
    public float perShotDelay = 0.2f;
    public float minDelay = 1f;
    public float maxDelay = 10f;
    float nextFire;
    int shotsFired = 0;
    public int volleyCount = 12;
    public Vector3 firePos = new Vector3(0f, 4f, 0.3f);
    public Vector3 fireAngle = new Vector3(-80f, 0f, 0f);
    public Transform target;
    Object missileObject;

    // Start is called before the first frame update
    void Awake()
    {
        nextFire = Random.Range(minDelay, maxDelay);
        missileObject = Resources.Load("SwarmMissile");
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeSinceLevelLoad >= nextFire)
        {
            Fire();
            shotsFired++;

            //if the number of shots per volley has been reached, set the time till the next shot to the firing delay and reset the shots fired counter
            if (shotsFired >= volleyCount)
            {
                nextFire = Time.timeSinceLevelLoad + fireDelay;
                shotsFired = 0;
            }
            else
            {
                nextFire = Time.timeSinceLevelLoad + perShotDelay;
            }
        }
    }

    void Fire()
    {
        //fire a missile
        GameObject missile = Instantiate(missileObject, transform.position + firePos, transform.rotation) as GameObject;
        missile.transform.Rotate(fireAngle, Space.Self);
        missile.GetComponent<SwarmController>().target = target;
    }



}
