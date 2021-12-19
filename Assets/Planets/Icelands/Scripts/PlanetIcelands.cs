using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlanetIcelands : MonoBehaviour, PlanetInterface
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
    public Gradient AtmosphereColor;

    [Header("Seeds")]
    [Range(1f, 10f)] public float LandSeed = 1f;
    [Range(1f, 10f)] public float WaterSeed = 1f;
    [Range(1f, 10f)] public float CloudsSeed = 1f;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float CloudCover = 0.5f;
    [Range(0f, 1f)] public float WaterFlow = 0.52f;

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
        _Land.SetMaterialProperty(ShaderProperties.Seed, LandSeed);
        _Water.SetMaterialProperty(ShaderProperties.Seed, WaterSeed);
        _Clouds.SetMaterialProperty(ShaderProperties.Seed, CloudsSeed);

        _Water.SetMaterialProperty(ShaderProperties.FlowRate, WaterFlow);
        _Clouds.SetMaterialProperty(ShaderProperties.CloudCover, CloudCover);
    }

    public void SetPixels(float ppu)
    {
        _Land.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Water.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Clouds.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Land.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Water.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Clouds.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Land.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Water.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Clouds.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Land.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
        _Water.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
        _Clouds.SetMaterialProperty(ShaderProperties.Speed, CloudSpeed);
    }

    public void SetColors()
    {
        Dictionary<string, float> colors;

        // Set Land colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
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

        // Set clouds colors.
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

        // Set atmosphere colors.
        _Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor.Evaluate(1));
    }

    public void UpdateMaterial()
    {
        _Land.UpdateMaterial();
        _Water.UpdateMaterial();
        _Clouds.UpdateMaterial();
        _Atmosphere.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Water.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Clouds.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Water.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Clouds.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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
