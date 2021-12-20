using UnityEditor;

[CustomEditor(typeof(PlanetGasGiantRinged))]
public class PlanetGasGiantRingedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetGasGiantRinged planet = (PlanetGasGiantRinged)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Planet,
                planet.Ring
            };

            if (System.Array.Exists(layers, element => element == null))
            {
                planet.Initialize();
            }

            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate();
            planet.SetLight(planet.LightOrigin);
            planet.SetSpeed();
            planet.EnableRing(planet.RingEnabled);

            planet.UpdateMaterial();
        }
    }
}