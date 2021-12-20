using UnityEditor;

[CustomEditor(typeof(PlanetDead))]
public class PlanetDeadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetDead planet = (PlanetDead)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Surface,
                planet.Craters
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
        }
    }
}
