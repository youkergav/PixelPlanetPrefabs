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

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Surface;
    private PlanetLayer _Atmosphere;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer atmosphereRenderer = transform.Find("Atmosphere").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material atmosphereMaterial = new Material(atmosphereRenderer.sharedMaterial);

        _Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        _Atmosphere = new PlanetLayer(gameObject, atmosphereRenderer, atmosphereMaterial);

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate(Rotation);
        SetLight(LightOrigin);
        SetSpeed();

        UpdateMaterial();
        _Initiated = true;
    }

    public void SetSeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
    }

    public void SetPixels(float ppu)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);
        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        _Surface.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(SurfaceColor));
        _Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor);
    }

    public void UpdateMaterial()
    {
        _Surface.UpdateMaterial();
        _Atmosphere.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    public void UpdateTime(float time)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
