using UnityEditor;

[CustomEditor(typeof(PlanetGasGiant))]
public class PlanetGasGiantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetGasGiant planet = (PlanetGasGiant)target;

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