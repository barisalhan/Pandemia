using UnityEngine;

/*
 * Global time of the game. It follows singleton pattern.
 * GameController and each model is dependent to this class.
 */
public class Time
{
    private int day;

    private int week;
    
    public int Week
    {
        get { return week; }
        set { week = value; }
    }



    private static Time time;

    private Time()
    {
        day = 1;
        week = 1;
    }


    public static void CreateTime()
    {
        if (time == null)
        {
            time = new Time();
            Debug.Log("New Time object is created.");
        }
        else
        {
            Debug.Log("Time is tried to be created multiple times.");
        }
    }


    public static Time GetInstance()
    {
        if (time == null)
        {
            CreateTime();
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

            if (currentDay % 7 == 0)
            {
                time.week++;
            }
        }
    }


    public int GetDay()
    {
        return day;
    }

    public void SetDay(int newDay)
    {
        day = newDay;
    }
}
