using UnityEditor;

[CustomEditor(typeof(PlanetIcelands))]
public class PlanetIcelandsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetIcelands planet = (PlanetIcelands)target;

        if (planet.Initiated)
        {
            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetLight(planet.LightOrigin);
            planet.SetSpeed();

            planet.UpdateMaterial();
        }
    }
}
