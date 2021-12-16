using UnityEditor;

[CustomEditor(typeof(PlanetContinents))]
public class PlanetContinentsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetContinents planet = (PlanetContinents)target;

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
