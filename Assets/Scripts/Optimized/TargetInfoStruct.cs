public struct TargetInfoStruct
{
    public Enemy Enemy { get; }
    public float DistanceSquared { get; }

    public TargetInfoStruct(Enemy enemy, float distanceSquared)
    {
        Enemy = enemy;
        DistanceSquared = distanceSquared;
    }
}