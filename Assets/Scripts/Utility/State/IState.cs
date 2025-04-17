namespace Scripts.Utility
{
    public interface IState
    {
        public void StrongEnter();

        public void WeakExit();

        public void WeakEnter();

        public void StrongExit();
    }
}
