using UnityEditor;

[CustomEditor(typeof(PlanetDeserts))]
public class PlanetDesertsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetDeserts planet = (PlanetDeserts)target;

        if (planet.Initiated)
        {
            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetLight(planet.LightOrigin);
            planet.SetSpeed(planet.Speed);

            planet.UpdateMaterial();
        }
    }
}
