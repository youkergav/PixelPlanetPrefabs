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
    [Range(1f, 10f)] public float SurfaceSeed = 1f;
    [Range(1f, 10f)] public float CraterSeed = 1f;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(8f, 256)] public int Pixels = 128;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Surface;
    private PlanetLayer _Craters;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer cratersRenderer = transform.Find("Craters").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material cratersMaterial = new Material(cratersRenderer.sharedMaterial);

        _Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        _Craters = new PlanetLayer(gameObject, cratersRenderer, cratersMaterial);

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate(Rotation);
        SetLight(LightOrigin);
        SetSpeed(Speed);

        UpdateMaterial();
        _Initiated = true;
    }

    public void SetSeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        _Craters.SetMaterialProperty(ShaderProperties.Seed, CraterSeed);
    }

    public void SetPixels(float ppu)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Craters.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Craters.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Craters.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed(float speed)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
        _Craters.SetMaterialProperty(ShaderProperties.Speed, Speed);
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
            _Surface.SetMaterialProperty(element.Key, SurfaceColor.Evaluate(element.Value));
        }

        // Set crater colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Craters.SetMaterialProperty(element.Key, CraterColor.Evaluate(element.Value));
        }
    }

    public void UpdateMaterial()
    {
        _Surface.UpdateMaterial();
        _Craters.UpdateMaterial();
    }

    public void UpdateMaterial(SpriteRenderer renderer, Material material)
    {
        renderer.sharedMaterial = material;
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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