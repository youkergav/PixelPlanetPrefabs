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
    [Range(1f, 10f)] public float SurfaceSeed = 1f;
    [Range(1f, 10f)] public float Clouds1Seed = 1f;
    [Range(1f, 10f)] public float Clouds2Seed = 1f;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(8f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float CloudCover1 = 0.35f;
    [Range(0f, 1f)] public float CloudCover2 = 0.5f;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Surface;
    private PlanetLayer _Clouds1;
    private PlanetLayer _Clouds2;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer surfaceRenderer = transform.Find("Surface").GetComponent<SpriteRenderer>();
        SpriteRenderer clouds1Renderer = transform.Find("Clouds1").GetComponent<SpriteRenderer>();
        SpriteRenderer clouds2Renderer = transform.Find("Clouds2").GetComponent<SpriteRenderer>();

        Material surfaceMaterial = new Material(surfaceRenderer.sharedMaterial);
        Material clouds1Material = new Material(clouds1Renderer.sharedMaterial);
        Material clouds2Material = new Material(clouds2Renderer.sharedMaterial);

        _Surface = new PlanetLayer(gameObject, surfaceRenderer, surfaceMaterial);
        _Clouds1 = new PlanetLayer(gameObject, clouds1Renderer, clouds1Material);
        _Clouds2 = new PlanetLayer(gameObject, clouds2Renderer, clouds2Material);

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

    public bool Is_Initiated()
    {
        return _Initiated;
    }

    public void SetSeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Seed, SurfaceSeed);
        _Clouds1.SetMaterialProperty(ShaderProperties.Seed, Clouds1Seed);
        _Clouds2.SetMaterialProperty(ShaderProperties.Seed, Clouds2Seed);

        _Clouds1.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover1);
        _Clouds2.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover2);
    }

    public void SetPixels(float ppu)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Clouds1.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Clouds2.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Surface.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Clouds1.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Clouds2.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Clouds1.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Clouds2.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Surface.SetMaterialProperty(ShaderProperties.Speed, Speed);
        _Clouds1.SetMaterialProperty(ShaderProperties.Speed, Speed);
        _Clouds2.SetMaterialProperty(ShaderProperties.Speed, Speed);
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

        // Set clouds1 colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color3, 0.33f },
            { ShaderProperties.Color2, 0.66f },
            { ShaderProperties.Color4, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Clouds1.SetMaterialProperty(element.Key, Clouds1Color.Evaluate(element.Value));
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
            _Clouds2.SetMaterialProperty(element.Key, Clouds2Color.Evaluate(element.Value));
        }
    }

    public void UpdateMaterial()
    {
        _Surface.UpdateMaterial();
        _Clouds1.UpdateMaterial();
        _Clouds2.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Clouds1.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Clouds2.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        _Surface.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Clouds1.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Clouds2.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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