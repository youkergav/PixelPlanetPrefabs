using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlanetGasGiant : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient SurfaceColor;
    public Gradient Clouds1Color;
    public Gradient Clouds2Color;

    [Header("Seeds")]
    [Range(1, 100)] public int SurfaceSeed = 1;
    [Range(1, 100)] public int Clouds1Seed = 1;
    [Range(1, 100)] public int Clouds2Seed = 1;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float CloudCover1 = 0.35f;
    [Range(0f, 1f)] public float CloudCover2 = 0.5f;

    [HideInInspector] public PlanetLayer Surface;
    [HideInInspector] public PlanetLayer Clouds1;
    [HideInInspector] public PlanetLayer Clouds2;

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
        SpriteRenderer clouds1Renderer = transform.Find("Clouds1").GetComponent<SpriteRenderer>();
        SpriteRenderer clouds2Renderer = transform.Find("Clouds2").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material clouds1Material = new Material(clouds1Renderer.sharedMaterial);
        Material clouds2Material = new Material(clouds2Renderer.sharedMaterial);

        Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        Clouds1 = new PlanetLayer(gameObject, clouds1Renderer, clouds1Material);
        Clouds2 = new PlanetLayer(gameObject, clouds2Renderer, clouds2Material);

        return true;
    }

    public void SetSeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        Clouds1.SetMaterialProperty(ShaderProperties.Seed, Clouds1Seed);
        Clouds2.SetMaterialProperty(ShaderProperties.Seed, Clouds2Seed);

        Clouds1.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover1);
        Clouds2.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover2);
    }

    public void SetPixels(float ppu)
    {
        Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Clouds1.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Clouds2.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Clouds1.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Clouds2.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Clouds1.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Clouds2.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Clouds1.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Clouds2.SetMaterialProperty(ShaderProperties.Speed, Speed);
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

        // Set clouds1 colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color3, 0.33f },
            { ShaderProperties.Color2, 0.66f },
            { ShaderProperties.Color4, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Clouds1.SetMaterialProperty(element.Key, Clouds1Color.Evaluate(element.Value));
        }

        // Set clouds2 colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color3, 0.33f },
            { ShaderProperties.Color2, 0.66f },
            { ShaderProperties.Color4, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Clouds2.SetMaterialProperty(element.Key, Clouds2Color.Evaluate(element.Value));
        }
    }

    public void UpdateMaterial()
    {
        Surface.UpdateMaterial();
        Clouds1.UpdateMaterial();
        Clouds2.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Clouds1.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Clouds2.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Clouds1.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Clouds2.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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