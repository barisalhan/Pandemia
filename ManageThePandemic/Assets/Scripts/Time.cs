using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }




    
}
