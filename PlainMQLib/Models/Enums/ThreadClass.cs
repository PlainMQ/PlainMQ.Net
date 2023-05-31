namespace PlainMQLib.Models.Enums
{
    /// <summary>
    /// Enum used for matching execution of ManagedThreads with GlobalEventQueue events
    /// </summary>
    public enum ThreadClass
    {
        MAIN,
        BROADCAST,
        LOGGER,
        TERMINATE,
        READER
    }
}
