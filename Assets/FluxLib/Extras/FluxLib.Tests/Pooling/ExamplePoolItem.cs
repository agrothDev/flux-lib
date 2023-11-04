using FluxLib.Source.Pooling;

namespace FluxLib.Extras.FluxLib.Tests.Pooling
{
    public class ExamplePoolItem : IPoolable
    {
        public int Value { get; private set; }

        public ExamplePoolItem()
        {
        }

        public void OnGet()
        {
            Value = 1;
        }

        public void OnRelease()
        {
            Value = 0;
        }
    }
}