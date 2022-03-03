using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierTracker : MonoBehaviour
{

    public int health = 100;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SwarmMissile")
        {
            health--;
        }
    }
}
