namespace FluxLib.Source.Pooling
{
    public interface IPoolable
    {
        void OnGet();
        void OnRelease();
    }
}