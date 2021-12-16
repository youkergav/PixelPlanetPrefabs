using UnityEditor;

[CustomEditor(typeof(PlanetStar))]
public class PlanetStarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetStar planet = (PlanetStar)target;

        if (planet.Initiated)
        {
            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetSpeed(planet.Speed);

            planet.UpdateMaterial();
        }
    }
}
