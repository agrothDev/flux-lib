namespace FluxLib.Source.Pooling
{
    public enum ObjectInitializationMode
    {
        Eager, // All objects will be initialized on creation
        Lazy   // Objects will be initialized on first use
    }
}