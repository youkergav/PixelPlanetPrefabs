using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlanetContinents : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float PlanetSpeed = 0.5f;
    [Range(-1f, 1f)] public float CloudSpeed = 0.5f;

    [Header("Colors")]
    public Gradient LandColor;
    public Gradient WaterColor;
    public Gradient CloudsColor;
    public Color AtmosphereColor;

    [Header("Seeds")]
    [Range(1, 100)] public int LandSeed = 1;
    [Range(1, 100)] public int WaterSeed = 1;
    [Range(1, 100)] public int CloudsSeed = 1;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float CloudCover = 0.5f;
    [Range(0f, 1f)] public float WaterFlow = 0.55f;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Land;
    private PlanetLayer _Water;
    private PlanetLayer _Clouds;
    private PlanetLayer _Atmosphere;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer landRenderer = transform.Find("Land").GetComponent<SpriteRenderer>();
        SpriteRenderer waterRenderer = transform.Find("Water").GetComponent<SpriteRenderer>();
        SpriteRenderer cloudsRenderer = transform.Find("Clouds").GetComponent<SpriteRenderer>();
        SpriteRenderer atmosphereRenderer = transform.Find("Atmosphere").GetComponent<SpriteRenderer>();

        Material landMaterial = new Material(landRenderer.sharedMaterial);
        Material waterMaterial = new Material(waterRenderer.sharedMaterial);
        Material cloudsMaterial = new Material(cloudsRenderer.sharedMaterial);
        Material atmosphereMaterial = new Material(atmosphereRenderer.sharedMaterial);

        _Land = new PlanetLayer(gameObject, landRenderer, landMaterial);
        _Water = new PlanetLayer(gameObject, waterRenderer, waterMaterial);
        _Clouds = new PlanetLayer(gameObject, cloudsRenderer, cloudsMaterial);
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
        _Water.SetMaterialProperty(ShaderProperties.Seed, WaterSeed);
        _Land.SetMaterialProperty(ShaderProperties.Seed, LandSeed);
        _Clouds.SetMaterialProperty(ShaderProperties.Seed, CloudsSeed);

        _Land.SetMaterialProperty(ShaderProperties.FlowRate, WaterFlow);
        _Clouds.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover);
    }

    public void SetPixels(float ppu)
    {
        _Water.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Land.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Clouds.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Water.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Land.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Clouds.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Water.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Land.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Clouds.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Clouds.SetMaterialProperty(ShaderProperties.Speed, CloudSpeed);
        _Water.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
        _Land.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
    }

    public void SetColors()
    {
        Dictionary<string, float> colors;

        // Set land colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.33f },
            { ShaderProperties.Color3, 0.66f },
            { ShaderProperties.Color4, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Land.SetMaterialProperty(element.Key, LandColor.Evaluate(element.Value));
        }

        // Set water colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Water.SetMaterialProperty(element.Key, WaterColor.Evaluate(element.Value));
        }

        // Set cloud colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color3, 0.33f },
            { ShaderProperties.Color2, 0.66f },
            { ShaderProperties.Color4, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Clouds.SetMaterialProperty(element.Key, CloudsColor.Evaluate(element.Value));
        }

        // Set atmostphere color.
        _Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor);
    }

    public void UpdateMaterial()
    {
        _Land.UpdateMaterial();
        _Water.UpdateMaterial();
        _Clouds.UpdateMaterial();
        _Atmosphere.UpdateMaterial();
    }

    public void UpdateMaterial(SpriteRenderer renderer, Material material)
    {
        renderer.sharedMaterial = material;
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Clouds.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Water.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    public void UpdateTime(float time)
    {
        _Clouds.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Water.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
