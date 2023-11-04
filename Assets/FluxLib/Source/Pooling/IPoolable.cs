namespace FluxLib.Source.Pooling
{
    public interface IPoolable
    {
        /// <summary>
        /// Called when the object is retrieved from the pool. Used to set the object up.
        /// </summary>
        void OnGet();

        /// <summary>
        /// Called when the object is released back into the pool. Used to reset the object state.
        /// </summary>
        void OnRelease();
    }
}