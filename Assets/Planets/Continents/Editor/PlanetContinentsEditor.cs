using UnityEditor;

[CustomEditor(typeof(PlanetContinents))]
public class PlanetContinentsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetContinents planet = (PlanetContinents)target;

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
