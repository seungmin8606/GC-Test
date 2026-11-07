public class TargetInfoClass
{
    public Enemy Enemy { get; }
    public float DistanceSquared { get; }

    public TargetInfoClass(Enemy enemy, float distanceSquared)
    {
        Enemy = enemy;
        DistanceSquared = distanceSquared;
    }
}