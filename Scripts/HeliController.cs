using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour
{
    Transform player;
    public float turnRate = 5.0f;
    public float moveSpeed = 10.0f;

    public int health = 10;

    Vector3 targetVector;
    Quaternion targetQuat;
    bool alive;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("F35").transform;
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {

        //turn towards player
        TargetUpdate();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, Time.deltaTime * turnRate);

        //health check
        if(health <= 0)
        {
            kill();
        }
        
    }

    void TargetUpdate()
    {
        targetVector = (player.position - transform.position);
        targetVector.Set(targetVector.x, 0.0f, targetVector.z);
        targetVector = targetVector.normalized;
        targetQuat = Quaternion.LookRotation(targetVector);
    }

    void kill()
    {
        GameObject corpse = Instantiate(Resources.Load("HeliCorpse"), transform.position, transform.rotation) as GameObject;
      
        if (corpse.GetComponent<Rigidbody>().angularVelocity != null)
        {
            corpse.GetComponent<Rigidbody>().angularVelocity = Vector3.up * 10.0f;
        }
        
        Destroy(gameObject);

        //should despawn the corpse after 30s, not sure if this'll still work from a dead script though
        Destroy(corpse, 30f);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerMissile" && alive)
        {
            //explode
            kill();
            alive = false;
        }
    }


}
