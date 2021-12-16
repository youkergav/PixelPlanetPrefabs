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
    public Gradient AtmosphereColor;

    [Header("Seeds")]
    [Range(1f, 10f)] public float LandSeed = 1f;
    [Range(1f, 10f)] public float CratersSeed = 1f;
    [Range(1f, 10f)] public float LavaSeed = 1f;

    [Header("Misc")]
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(8f, 256)] public int Pixels = 128;
    [Range(0f, 1f)] public float LavaFlow = 0.4f;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Land;
    private PlanetLayer _Craters;
    private PlanetLayer _Lava;
    private PlanetLayer _Atmosphere;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer landRenderer = transform.Find("Land").GetComponent<SpriteRenderer>();
        SpriteRenderer cratersRenderer = transform.Find("Craters").GetComponent<SpriteRenderer>();
        SpriteRenderer lavaRenderer = transform.Find("Lava").GetComponent<SpriteRenderer>();
        SpriteRenderer atmosphereRenderer = transform.Find("Atmosphere").GetComponent<SpriteRenderer>();

        Material landMaterial = new Material(landRenderer.sharedMaterial);
        Material cratersMaterial = new Material(cratersRenderer.sharedMaterial);
        Material lavaMaterial = new Material(lavaRenderer.sharedMaterial);
        Material atmosphereMaterial = new Material(atmosphereRenderer.sharedMaterial);

        _Land = new PlanetLayer(gameObject, landRenderer, landMaterial);
        _Craters = new PlanetLayer(gameObject, cratersRenderer, cratersMaterial);
        _Lava = new PlanetLayer(gameObject, lavaRenderer, lavaMaterial);
        _Atmosphere = new PlanetLayer(gameObject, atmosphereRenderer, atmosphereMaterial);

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

    public bool Is_Initiated()
    {
        return _Initiated;
    }

    public void SetSeed()
    {
        _Land.SetMaterialProperty(ShaderProperties.Seed, LandSeed);
        _Craters.SetMaterialProperty(ShaderProperties.Seed, CratersSeed);
        _Lava.SetMaterialProperty(ShaderProperties.Seed, LavaSeed);

        _Lava.SetMaterialProperty(ShaderProperties.FlowRate, LavaFlow);
    }

    public void SetPixels(float ppu)
    {
        _Land.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Craters.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Lava.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Atmosphere.SetMaterialProperty(ShaderProperties.Pixels, ppu);
    }

    public void SetLight(Vector2 position)
    {
        _Land.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Craters.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Lava.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Atmosphere.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Land.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Craters.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Lava.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed(float speed)
    {
        _Land.SetMaterialProperty(ShaderProperties.Speed, speed);
        _Craters.SetMaterialProperty(ShaderProperties.Speed, speed);
        _Lava.SetMaterialProperty(ShaderProperties.Speed, speed);
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
            _Land.SetMaterialProperty(element.Key, LandColor.Evaluate(element.Value));
        }

        // Set craters colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Craters.SetMaterialProperty(element.Key, CratersColor.Evaluate(element.Value));
        }

        // Set lava colors.
        colors = new Dictionary<string, float> {
            { ShaderProperties.Color, 0f },
            { ShaderProperties.Color2, 0.5f },
            { ShaderProperties.Color3, 1f  }
        };

        foreach (KeyValuePair<string, float> element in colors)
        {
            _Lava.SetMaterialProperty(element.Key, LavaColor.Evaluate(element.Value));
        }

        // Set atmosphere color.
        _Atmosphere.SetMaterialProperty(ShaderProperties.Color, AtmosphereColor.Evaluate(1));
    }

    public void UpdateMaterial()
    {
        _Land.UpdateMaterial();
        _Craters.UpdateMaterial();
        _Lava.UpdateMaterial();
        _Atmosphere.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Lava.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
    }

    public void UpdateTime(float time)
    {
        _Land.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Craters.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Lava.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
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
