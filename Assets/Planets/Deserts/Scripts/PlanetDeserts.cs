using UnityEngine;

[ExecuteInEditMode]
public class PlanetDeserts : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient SurfaceColor;
    public Color AtmosphereColor;

    [Header("Seeds")]
    [Range(1, 100)] public int SurfaceSeed = 100;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;

    [HideInInspector] public PlanetLayer Surface;
    [HideInInspector] public PlanetLayer Atmosphere;

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
        SetLight(LightOrigin);
        SetSpeed();

        UpdateMaterial();
    }

    public bool Initialize()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer atmosphereRenderer = transform.Find("Atmosphere").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material atmosphereMaterial = new Material(atmosphereRenderer.sharedMaterial);

        Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        Atmosphere = new PlanetLayer(gameObject, atmosphereRenderer, atmosphereMaterial);

        return true;
    }

    public void SetSeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
    }

    public void SetPixels(float ppu)
    {
        Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
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
    }

    public void SetColors()
    {
        Surface.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(SurfaceColor));
        Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor);
    }

    public void UpdateMaterial()
    {
        Surface.UpdateMaterial();
        Atmosphere.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    public void UpdateTime(float time)
    {
        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
