namespace Capsule.Util
{
    public class Tuple<T, U>
    {
        public T First;
        public U Second;
        public Tuple()
        {
            this.First = default;
            this.Second = default;
        }
        public Tuple(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
