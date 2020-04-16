interface ITimeDrivable
{
    /* It is called in the very beginning of the game. */
    void SetDefaultEnvironment();

    /* It is called just before midnight. */
    void NextDay();

    /* It is called by NextDay(). */
    void UpdateFields();

    /* In the morning, events are executed */
    //void ExecuteEvents();

    /*
     *  23PM: NextDay() => Vaka sayisini acikla ve kedndi fieldlarimi guncelle.
     *  10AM: ExecuteEvents() => direkt parametreleri degistir.
     */
}
