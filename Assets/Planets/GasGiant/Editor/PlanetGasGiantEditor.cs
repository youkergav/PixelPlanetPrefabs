using UnityEditor;

[CustomEditor(typeof(PlanetGasGiant))]
public class PlanetGasGiantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetGasGiant planet = (PlanetGasGiant)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Surface,
                planet.Clouds1,
                planet.Clouds2
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