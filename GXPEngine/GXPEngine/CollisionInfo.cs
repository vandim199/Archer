using GXPEngine; // For GameObject

public class CollisionInfo {
	public readonly Vec2 normal;
	public readonly GameObject other;
	public readonly float timeOfImpact;
    public readonly Vec2 pointOfImpact;

    public CollisionInfo(Vec2 pNormal, GameObject pOther, float pTimeOfImpact, Vec2 pPointOfImpact = new Vec2()) {
		normal = pNormal;
		other = pOther;
		timeOfImpact = pTimeOfImpact;
        pointOfImpact = pPointOfImpact;
	}
}
