using UnityEditor;

[CustomEditor(typeof(PlanetBlackHole))]
public class PlanetBlackHoleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetBlackHole planet = (PlanetBlackHole)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Hole,
                planet.Disk
            };

            if (System.Array.Exists(layers, element => element == null))
            {
                planet.Initialize();
            }

            planet.SetSeed();
            planet.SetColors();
            planet.SetPixels(planet.Pixels);
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetSpeed();

            planet.UpdateMaterial();
        }
    }
}
