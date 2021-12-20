using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PlanetVolcanoes : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 0f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient LandColor;
    public Gradient CratersColor;
    public Gradient LavaColor;
    public Color AtmosphereColor;

    [Header("Seeds")]
    [Range(1, 100)] public int LandSeed = 1;
    [Range(1, 100)] public int CratersSeed = 1;
    [Range(1, 100)] public int LavaSeed = 1;

    [Header("Misc")]
    public bool CratersEnabled = true;
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float LavaFlow = 0.4f;

    [HideInInspector] public PlanetLayer Land;
    [HideInInspector] public PlanetLayer Craters;
    [HideInInspector] public PlanetLayer Lava;
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
        EnableCraters(CratersEnabled);

        UpdateMaterial();
    }

    public bool Initialize()
    {
        SpriteRenderer landRenderer = transform.Find("Land").GetComponent<SpriteRenderer>();
        SpriteRenderer cratersRenderer = transform.Find("Craters").GetComponent<SpriteRenderer>();
        SpriteRenderer lavaRenderer = transform.Find("Lava").GetComponent<SpriteRenderer>();
        SpriteRenderer atmosphereRenderer = transform.Find("Atmosphere").GetComponent<SpriteRenderer>();

        Material landMaterial = new Material(landRenderer.sharedMaterial);
        Material cratersMaterial = new Material(cratersRenderer.sharedMaterial);
        Material lavaMaterial = new Material(lavaRenderer.sharedMaterial);
        Material atmosphereMaterial = new Material(atmosphereRenderer.sharedMaterial);

        Land = new PlanetLayer(gameObject, landRenderer, landMaterial);
        Craters = new PlanetLayer(gameObject, cratersRenderer, cratersMaterial);
        Lava = new PlanetLayer(gameObject, lavaRenderer, lavaMaterial);
        Atmosphere = new PlanetLayer(gameObject, atmosphereRenderer, atmosphereMaterial);

        return true;
    }

    public void SetSeed()
    {
        Land.SetMaterialProperty(ShaderProperties.Seed, LandSeed);
        Craters.SetMaterialProperty(ShaderProperties.Seed, CratersSeed);
        Lava.SetMaterialProperty(ShaderProperties.Seed, LavaSeed);

        Lava.SetMaterialProperty(ShaderProperties.FlowRate, LavaFlow);
    }

    public void SetPixels(float ppu)
    {
        Land.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Craters.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Lava.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        Land.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Craters.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Lava.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        Land.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Craters.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Lava.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Land.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Craters.SetMaterialProperty(ShaderProperties.Speed, Speed);
        Lava.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        Dictionary<string, float> colors;

        // Set land colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Land.SetMaterialProperty(element.Key, LandColor.Evaluate(element.Value));
        }

        // Set craters colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Craters.SetMaterialProperty(element.Key, CratersColor.Evaluate(element.Value));
        }

        // Set lava colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            Lava.SetMaterialProperty(element.Key, LavaColor.Evaluate(element.Value));
        }

        // Set atmosphere color.
        Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor);
    }

    public void EnableCraters(bool enabled)
    {
        Craters.SetEnabled(enabled);
    }

    public void UpdateMaterial()
    {
        Land.UpdateMaterial();
        Craters.UpdateMaterial();
        Lava.UpdateMaterial();
        Atmosphere.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Land.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Lava.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        Land.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Lava.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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
