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
    public Gradient EmmisionColor;

    [Header("Seeds")]
    [Range(1f, 10f)] public float SurfaceSeed = 1f;
    [Range(1f, 10f)] public float FlaresSeed = 1f;
    [Range(1f, 10f)] public float EmmisionSeed = 1f;

    [Header("Misc")]
    [Range(0f, 256)] public int Pixels = 128;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Surface;
    private PlanetLayer _Flares;
    private PlanetLayer _Emission;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer flaresRenderer = transform.Find("Flares").GetComponent<SpriteRenderer>();
        SpriteRenderer emissionRenderer = transform.Find("Emission").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material flaresMaterial = new Material(flaresRenderer.sharedMaterial);
        Material emissionMaterial = new Material(emissionRenderer.sharedMaterial);

        _Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        _Flares = new PlanetLayer(gameObject, flaresRenderer, flaresMaterial);
        _Emission = new PlanetLayer(gameObject, emissionRenderer, emissionMaterial);


        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate(Rotation);
        SetSpeed();

        UpdateMaterial();
        _Initiated = true;
    }

    public bool Is_Initiated()
    {
        return _Initiated;
    }

    public void SetSeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        _Flares.SetMaterialProperty(ShaderProperties.Seed, FlaresSeed);
        _Emission.SetMaterialProperty(ShaderProperties.Seed, EmmisionSeed);
    }

    public void SetPixels(float ppu)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Emission.SetMaterialProperty(ShaderProperties.Pixels, ppu * 2);
        _Flares.SetMaterialProperty(ShaderProperties.Pixels, ppu * 2);
    }

    public void SetLight(Vector2 position)
    {
        return;
    }

    public void SetRotate(float rotation)
    {
        _Emission.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Flares.SetMaterialProperty(ShaderProperties.Rotation, rotation);
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
        _Flares.SetMaterialProperty(ShaderProperties.Speed, Speed * 0.5f);
        _Emission.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        _Surface.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(SurfaceColor));
        _Flares.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(FlaresColor));
        _Emission.SetMaterialProperty(ShaderProperties.Color, EmmisionColor.Evaluate(0f));
    }

    public void UpdateMaterial()
    {
        _Surface.UpdateMaterial();
        _Flares.UpdateMaterial();
        _Emission.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, start);
        _Flares.SetMaterialProperty(ShaderProperties.Timestamp, start);
        _Emission.SetMaterialProperty(ShaderProperties.Timestamp, start);
    }

    public void UpdateTime(float time)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.1f);
        _Flares.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Emission.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
