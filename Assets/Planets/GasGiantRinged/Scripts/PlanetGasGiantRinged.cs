using UnityEngine;

[ExecuteInEditMode]
public class PlanetGasGiantRinged : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float PlanetRotation = 0f;
    [Range(0f, 6.28f)] public float RingRotation = 0f;
    [Range(-1f, 1f)] public float PlanetSpeed = 0.5f;
    [Range(-1f, 1f)] public float RingSpeed = 0.5f;

    [Header("Colors")]
    public Gradient PlanetColor;
    public Gradient RingColor;

    [Header("Seeds")]
    [Range(1, 100)] public int PlanetSeed = 100;
    [Range(1, 100)] public int RingSeed = 100;

    [Header("Misc")]
    public bool RingEnabled = true;
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;

    [HideInInspector] public PlanetLayer Planet;
    [HideInInspector] public PlanetLayer Ring;

    public bool Initialized
    {
        get
        {
            return _Initialized;
        }
    }

    private float _Timestamp = 0f;
    private bool _Initialized = false;

    private void Awake()
    {
        _Initialized = Initialize();

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate();
        SetLight(LightOrigin);
        SetSpeed();
        EnableRing(RingEnabled);

        UpdateMaterial();
    }

    public bool Initialize()
    {
        SpriteRenderer planetRenderer = transform.Find("Planet").GetComponent<SpriteRenderer>();
        SpriteRenderer ringRenderer = transform.Find("Ring").GetComponent<SpriteRenderer>();

        Material planetMaterial = new Material(planetRenderer.sharedMaterial);
        Material ringMaterial = new Material(ringRenderer.sharedMaterial);

        Planet = new PlanetLayer(gameObject, planetRenderer, planetMaterial);
        Ring = new PlanetLayer(gameObject, ringRenderer, ringMaterial);

        return true;
    }

    public void SetSeed()
    {
        Planet.SetMaterialProperty(ShaderProperties.Seed, PlanetSeed);
        Ring.SetMaterialProperty(ShaderProperties.Seed, RingSeed);
    }

    public void SetPixels(float ppu)
    {
        Planet.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Ring.SetMaterialProperty(ShaderProperties.Pixels, ppu * 3f);
    }

    public void SetLight(Vector2 position)
    {
        Planet.SetMaterialProperty(ShaderProperties.LightOrigin, position * 1.3f);
        Ring.SetMaterialProperty(ShaderProperties.LightOrigin, position * 1.3f);
    }

    public void SetRotate()
    {
        Planet.SetMaterialProperty(ShaderProperties.Rotation, PlanetRotation);
        Ring.SetMaterialProperty(ShaderProperties.Rotation, RingRotation);
    }

    public void SetRotate(float rotation)
    {
        Planet.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Ring.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Planet.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
        Ring.SetMaterialProperty(ShaderProperties.Speed, RingSpeed);
    }

    public void SetColors()
    {
        Gradient gradientLight = new Gradient();
        Gradient gradientDark = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[3];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];
        float[] newTimes = new float[3] { 0f, 0.5f, 1f };
        float[] oldTimes = new float[3];

        // Set the planet colors.
        oldTimes = new float[3] { 0f, 0.2f, .4f };
        for (int i = 0; i < newTimes.Length; i++)
        {
            colorKey[i].color = PlanetColor.Evaluate(oldTimes[i]);
            colorKey[i].time = newTimes[i];

            alphaKey[i].alpha = 1;
            alphaKey[i].time = newTimes[i];
        }
        gradientLight.SetKeys(colorKey, alphaKey);

        oldTimes = new float[3] { 0.6f, 0.8f, 1f };
        for (int i = 0; i < newTimes.Length; i++)
        {
            colorKey[i].color = PlanetColor.Evaluate(oldTimes[i]);
            colorKey[i].time = newTimes[i];

            alphaKey[i].alpha = 1;
            alphaKey[i].time = newTimes[i];
        }
        gradientDark.SetKeys(colorKey, alphaKey);

        Planet.SetMaterialProperty(ShaderProperties.GradientTex2, PlanetUtil.GenTexture(gradientLight));
        Planet.SetMaterialProperty(ShaderProperties.GradientTex3, PlanetUtil.GenTexture(gradientDark));

        // Set the planet colors.
        oldTimes = new float[3] { 0f, 0.2f, .4f };
        for (int i = 0; i < newTimes.Length; i++)
        {
            colorKey[i].color = RingColor.Evaluate(oldTimes[i]);
            colorKey[i].time = newTimes[i];

            alphaKey[i].alpha = 1;
            alphaKey[i].time = newTimes[i];
        }
        gradientLight.SetKeys(colorKey, alphaKey);

        oldTimes = new float[3] { 0.6f, 0.8f, 1f };
        for (int i = 0; i < newTimes.Length; i++)
        {
            colorKey[i].color = RingColor.Evaluate(oldTimes[i]);
            colorKey[i].time = newTimes[i];

            alphaKey[i].alpha = 1;
            alphaKey[i].time = newTimes[i];
        }
        gradientDark.SetKeys(colorKey, alphaKey);

        Ring.SetMaterialProperty(ShaderProperties.GradientTex2, PlanetUtil.GenTexture(gradientLight));
        Ring.SetMaterialProperty(ShaderProperties.GradientTex3, PlanetUtil.GenTexture(gradientDark));
    }

    public void EnableRing(bool enabled)
    {
        Ring.SetEnabled(enabled);
    }

    public void UpdateMaterial()
    {
        Planet.UpdateMaterial();
        Ring.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Planet.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Ring.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f * -3f);
    }

    public void UpdateTime(float time)
    {
        Planet.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        Ring.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f * -3f);
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