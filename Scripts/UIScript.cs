using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIScript : MonoBehaviour
{

    public int playerHealth = 20;
    
    public EndScreenScript EndScreen;
    public PlaneController player;
    public CarrierTracker carrier;
    public Text UIText;

    int numHStart, hKills, numTStart, tKills;
    public int cHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        numHStart = GameObject.FindGameObjectsWithTag("Enemy").Length;
        numTStart = GameObject.FindGameObjectsWithTag("EnemyTruck").Length;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cHealth = carrier.health;
        //health check
        if(playerHealth <= 0 || cHealth <= 0)
        {
            hKills = numHStart - GameObject.FindGameObjectsWithTag("Enemy").Length;
            tKills = numTStart - GameObject.FindGameObjectsWithTag("EnemyTruck").Length;
            EndScreen.LoseScreen(hKills, tKills, Time.timeSinceLevelLoad, cHealth, playerHealth);
        }

        //all threats dead check
        if((GameObject.FindGameObjectsWithTag("EnemyTruck").Length + GameObject.FindGameObjectsWithTag("Enemy").Length + GameObject.FindGameObjectsWithTag("SwarmMissile").Length) == 0)
        {
            EndScreen.WinScreen(numHStart, numTStart, Time.timeSinceLevelLoad, cHealth, playerHealth);
        }

        UIText.text = ("Helicopters Remaining:\t\t\t" + GameObject.FindGameObjectsWithTag("Enemy").Length + "\n" +
                    "Missile Trucks Remaining:\t\t" + GameObject.FindGameObjectsWithTag("EnemyTruck").Length + "\n" +
                    "Carrier Health:\t\t" + cHealth + " / 100\n" +
                    "Player Health:\t\t\t  " + playerHealth + " / 20");

    }
}
