using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenScript : MonoBehaviour
{

    public Text Points;
    public GameObject Victory, GameOver, UI;

    public void LoseScreen(int hKills, int tKills, float playTime, int cHealth, int pHealth)
    {
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
        gameObject.SetActive(true);
        GameOver.SetActive(true);
        Victory.SetActive(false);
        UI.SetActive(false);
        //convert playTime to a string
        string timeString = TimeToString(playTime);
        int points = CalcPoints(hKills, tKills, playTime, cHealth, pHealth);
        DisplayPoints(hKills, tKills, timeString, cHealth, pHealth, points);
    }

    public void WinScreen(int hKills, int tKills, float playTime, int cHealth, int pHealth)
    {
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
        gameObject.SetActive(true);
        GameOver.SetActive(false);
        Victory.SetActive(true);
        UI.SetActive(false);
        //convert playTime to a string
        string timeString = TimeToString(playTime);
        int points = CalcPoints(hKills, tKills, playTime, cHealth, pHealth);
        DisplayPoints(hKills, tKills, timeString, cHealth, pHealth, points);
    }

    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    void DisplayPoints(int hKills, int tKills, string playTime, int cHealth, int pHealth, int points)
    {
        Points.text = "Helicopter Kills:\t\t\t" + hKills + "\n" +
                    "Missile Truck Kills:\t\t" + tKills + "\n" +
                    "Time:\t\t\t\t\t   " + playTime + "\n" +
                    "Carrier Health:\t\t" + cHealth + " / 100\n" +
                    "Player Health:\t\t\t  " + pHealth + " / 20\n" +
                    "TOTAL:\t\t\t" + points + "PTS";
    }

    string TimeToString(float time)
    {
        int hours, minutes, seconds;
        hours = (int)(time / 3600);
        minutes = (int)((time % 3600)/60);
        seconds = (int)(time % 60);
        return hours.ToString().PadLeft(2, '0') + ":" + minutes.ToString().PadLeft(2, '0') + ":" + seconds.ToString().PadLeft(2, '0');
    }

    int CalcPoints(int hKills, int tKills, float playTime, int cHealth, int pHealth)
    {
        int timeScore = (int)(500 - playTime);
        int points = (100 * hKills) + (150 * tKills) + (timeScore) + (10 * cHealth) + (5 * pHealth);
        return points;
    }

}
