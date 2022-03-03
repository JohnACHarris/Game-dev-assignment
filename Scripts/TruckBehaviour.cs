using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckBehaviour : MonoBehaviour
{

    public int health = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Kill();
        }
    }


    public void Kill()
    {
        GameObject corpse = Instantiate(Resources.Load("TruckCorpse"), transform.position, transform.rotation) as GameObject;

        Destroy(gameObject);

        //should despawn the corpse after 30s, not sure if this'll still work from a dead script though
        Destroy(corpse, 30f);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerMissile")
        {
            //explode
            Kill();
        }
    }

}
