using UnityEditor;

[CustomEditor(typeof(PlanetIcelands))]
public class PlanetIcelandsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetIcelands planet = (PlanetIcelands)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Land,
                planet.Water,
                planet.Clouds,
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
