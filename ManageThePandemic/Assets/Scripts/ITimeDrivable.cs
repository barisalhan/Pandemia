interface ITimeDrivable
{
    /* It is called in the very beginning of the game. */
    void SetDefaultEnvironment();
    /* It is called just before midnight. */
    void NextDay();
    /* It is called by NextDay(). */
    void UpdateFields();
    
    
}
