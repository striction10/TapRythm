namespace TapRythm.Interfaces
{
    public interface IMovable
    {
        void Move(float speed);
        bool HasReachedTarget();
    }
}