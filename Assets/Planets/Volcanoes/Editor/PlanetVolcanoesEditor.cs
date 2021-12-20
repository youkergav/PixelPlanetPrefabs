using UnityEditor;

[CustomEditor(typeof(PlanetVolcanoes))]
public class PlanetVolcanoesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetVolcanoes planet = (PlanetVolcanoes)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Land,
                planet.Craters,
                planet.Lava,
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
            planet.EnableCraters(planet.CratersEnabled);

            planet.UpdateMaterial();
        }
    }
}
