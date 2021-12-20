using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlanetDead : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient SurfaceColor;
    public Gradient CraterColor;

    [Header("Seeds")]
    [Range(1, 100)] public int SurfaceSeed = 1;
    [Range(1, 100)] public int CraterSeed = 1;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;

    [HideInInspector] public PlanetLayer Surface;
    [HideInInspector] public PlanetLayer Craters;

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
        SpriteRenderer cratersRenderer = transform.Find("Craters").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material cratersMaterial = new Material(cratersRenderer.sharedMaterial);

        Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        Craters = new PlanetLayer(gameObject, cratersRenderer, cratersMaterial);

        return true;
    }

    public void SetSeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        Craters.SetMaterialProperty(ShaderProperties.Seed, CraterSeed);
    }

    public void SetPixels(float ppu)
    {
        Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Craters.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Craters.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Craters.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Craters.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        Dictionary<string, float> colors;

        // Set surface colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Surface.SetMaterialProperty(element.Key, SurfaceColor.Evaluate(element.Value));
        }

        // Set crater colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Craters.SetMaterialProperty(element.Key, CraterColor.Evaluate(element.Value));
        }
    }

    public void UpdateMaterial()
    {
        Surface.UpdateMaterial();
        Craters.UpdateMaterial();
    }

    public void UpdateMaterial(SpriteRenderer renderer, Material material)
    {
        renderer.sharedMaterial = material;
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    private void Update()
    {
        if (Application.IsPlaying(transform))
        {
            _Timestamp += Time.deltaTime;
            UpdateTime(_Timestamp);
        }
    }
}