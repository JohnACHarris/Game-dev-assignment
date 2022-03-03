using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissileController : MonoBehaviour
{
    float speed = 100.0f;
    float initSpeed;

    // Start is called before the first frame update
    void Start()
    {
        initSpeed = GameObject.Find("F35").GetComponent<PlaneController>().currentSpeed;
        
        //limit lifetime for performance
        Destroy(gameObject, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * (speed + initSpeed) * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            //explode
            Destroy(gameObject);
            Instantiate(Resources.Load("JMO Assets/WarFX/_Effects/Explosions/WFX_Explosion"), transform.position, transform.rotation);
        }
    }

    
}
