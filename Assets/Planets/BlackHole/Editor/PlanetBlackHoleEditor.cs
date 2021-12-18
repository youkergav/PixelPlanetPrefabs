using UnityEditor;

[CustomEditor(typeof(PlanetBlackHole))]
public class PlanetBlackHoleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetBlackHole planet = (PlanetBlackHole)target;

        if (planet.Initiated)
        {
            planet.SetSeed();
            planet.SetColors();
            planet.SetPixels(planet.Pixels);
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetSpeed();

            planet.UpdateMaterial();
        }
    }
}
