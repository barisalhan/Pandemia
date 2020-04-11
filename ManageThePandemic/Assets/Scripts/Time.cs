using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Global time of the game. It follows singleton pattern.
 * GameController and each model is dependent to this class.
 */
public class Time
{
    private int day;

    private static Time time;

    private Time()
    {
        day = 1;
    }

    public int GetDay()
    {
        return day;
    }

    public void SetDay(int newDay)
    {
        day = newDay;
    }

    public static Time GetInstance()
    {
        if (time == null)
        {
            time = new Time();
            Debug.Log("New Time object is created.");
            return time;
        }
        else
        { 
            return time;
        }

    }

    public static void NextDay()
    {
        if (time == null)
        {
            Debug.Log("NextDay() is called before Time object is created.");
        }
        else
        {
            int currentDay = time.GetDay();
            time.SetDay(currentDay + 1);
            Debug.Log("NextDay() is invoked. Current day: " + time.GetDay());
        }
    }




    
}
