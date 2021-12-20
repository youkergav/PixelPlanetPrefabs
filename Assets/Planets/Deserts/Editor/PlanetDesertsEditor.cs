using UnityEditor;

[CustomEditor(typeof(PlanetDeserts))]
public class PlanetDesertsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetDeserts planet = (PlanetDeserts)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Surface,
                planet.Atmosphere
            };

            if (System.Array.Exists(layers, element => element == null))
            {
                planet.Initialize();
            }

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
