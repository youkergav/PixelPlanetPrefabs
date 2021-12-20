using UnityEditor;

[CustomEditor(typeof(PlanetStar))]
public class PlanetStarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlanetStar planet = (PlanetStar)target;

        if (planet.Initialized)
        {
            PlanetLayer[] layers = {
                planet.Surface,
                planet.Flares,
                planet.Emission
            };

            if (System.Array.Exists(layers, element => element == null))
            {
                planet.Initialize();
            }

            planet.SetSeed();
            planet.SetColors();
            planet.SetSize(planet.Size);
            planet.SetRotate(planet.Rotation);
            planet.SetSpeed();

            planet.UpdateMaterial();
        }
    }
}
