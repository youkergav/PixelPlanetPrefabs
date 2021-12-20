using UnityEngine;

[ExecuteInEditMode]
public class PlanetStar : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 5f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient SurfaceColor;
    public Gradient FlaresColor;
    public Color EmmisionColor;

    [Header("Seeds")]
    [Range(1, 100)] public int SurfaceSeed = 1;
    [Range(1, 100)] public int FlaresSeed = 1;
    [Range(1, 100)] public int EmmisionSeed = 1;

    [Header("Misc")]
    [Range(0f, 256)] public int Pixels = 128;

    [HideInInspector] public PlanetLayer Surface;
    [HideInInspector] public PlanetLayer Flares;
    [HideInInspector] public PlanetLayer Emission;

    public bool Initialized
    {
        get
        {
            return _Initialized;
        }
    }

    private bool _Initialized = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        _Initialized = Initialize();

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate(Rotation);
        SetSpeed();

        UpdateMaterial();
    }

    public bool Initialize()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer flaresRenderer = transform.Find("Flares").GetComponent<SpriteRenderer>();
        SpriteRenderer emissionRenderer = transform.Find("Emission").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material flaresMaterial = new Material(flaresRenderer.sharedMaterial);
        Material emissionMaterial = new Material(emissionRenderer.sharedMaterial);

        Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        Flares = new PlanetLayer(gameObject, flaresRenderer, flaresMaterial);
        Emission = new PlanetLayer(gameObject, emissionRenderer, emissionMaterial);

        return true;
    }

    public void SetSeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        Flares.SetMaterialProperty(ShaderProperties.Seed, FlaresSeed);
        Emission.SetMaterialProperty(ShaderProperties.Seed, EmmisionSeed);
    }

    public void SetPixels(float ppu)
    {
        Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Emission.SetMaterialProperty(ShaderProperties.Pixels, ppu * 2);
        Flares.SetMaterialProperty(ShaderProperties.Pixels, ppu * 2);
    }

    public void SetLight(Vector2 position)
    {
        return;
    }

    public void SetRotate(float rotation)
    {
        Emission.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Flares.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Flares.SetMaterialProperty(ShaderProperties.Speed, Speed * 0.5f);
        Emission.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        Surface.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(SurfaceColor));
        Flares.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(FlaresColor));
        Emission.SetMaterialProperty(ShaderProperties.Color, EmmisionColor);
    }

    public void UpdateMaterial()
    {
        Surface.UpdateMaterial();
        Flares.UpdateMaterial();
        Emission.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        Surface.SetMaterialProperty(ShaderProperties.Timestamp, start);
        Flares.SetMaterialProperty(ShaderProperties.Timestamp, start);
        Emission.SetMaterialProperty(ShaderProperties.Timestamp, start);
    }

    public void UpdateTime(float time)
    {
        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.1f);
        Flares.SetMaterialProperty(ShaderProperties.Timestamp, time);
        Emission.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    private void Update()
    {
        if (Application.IsPlaying(gameObject))
        {
            _Timestamp += Time.deltaTime;
            UpdateTime(_Timestamp);
        }
    }
}
