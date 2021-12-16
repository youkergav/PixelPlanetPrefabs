using UnityEditor;

[CustomEditor(typeof(PlanetGasGiantRinged))]
public class PlanetGasGiantRingedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetGasGiantRinged planet = (PlanetGasGiantRinged)target;

        if (planet.Initiated)
        {
            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate();
            planet.SetLight(planet.LightOrigin);
            planet.SetSpeed(planet.Speed);

            planet.UpdateMaterial();
        }
    }
}