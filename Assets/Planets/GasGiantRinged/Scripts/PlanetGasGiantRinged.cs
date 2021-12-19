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
    [Range(1f, 10f)] public float PlanetSeed = 1f;
    [Range(1f, 10f)] public float RingSeed = 1f;

    [Header("Misc")]
    public bool RingEnabled = true;
    public Vector2 LightOrigin = new Vector2(0.3f, 0.7f);
    [Range(0f, 256)] public int Pixels = 128;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Planet;
    private PlanetLayer _Ring;

    private float _Timestamp = 0f;
    private bool _Initiated = false;

    private void Awake()
    {
        SpriteRenderer planetRenderer = transform.Find("Planet").GetComponent<SpriteRenderer>();
        SpriteRenderer ringRenderer = transform.Find("Ring").GetComponent<SpriteRenderer>();

        Material planetMaterial = new Material(planetRenderer.sharedMaterial);
        Material ringMaterial = new Material(ringRenderer.sharedMaterial);

        _Planet = new PlanetLayer(gameObject, planetRenderer, planetMaterial);
        _Ring = new PlanetLayer(gameObject, ringRenderer, ringMaterial);

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate();
        SetLight(LightOrigin);
        SetSpeed();
        EnableRing(RingEnabled);

        _Initiated = true;
        UpdateMaterial();
    }

    public void SetSeed()
    {
        _Planet.SetMaterialProperty(ShaderProperties.Seed, PlanetSeed);
        _Ring.SetMaterialProperty(ShaderProperties.Seed, RingSeed);
    }

    public void SetPixels(float ppu)
    {
        _Planet.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Ring.SetMaterialProperty(ShaderProperties.Pixels, ppu * 3f);
    }

    public void SetLight(Vector2 position)
    {
        _Planet.SetMaterialProperty(ShaderProperties.LightOrigin, position * 1.3f);
        _Ring.SetMaterialProperty(ShaderProperties.LightOrigin, position * 1.3f);
    }

    public void SetRotate()
    {
        _Planet.SetMaterialProperty(ShaderProperties.Rotation, PlanetRotation);
        _Ring.SetMaterialProperty(ShaderProperties.Rotation, RingRotation);
    }

    public void SetRotate(float rotation)
    {
        _Planet.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Ring.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);

        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Planet.SetMaterialProperty(ShaderProperties.Speed, PlanetSpeed);
        _Ring.SetMaterialProperty(ShaderProperties.Speed, RingSpeed);
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

        _Planet.SetMaterialProperty(ShaderProperties.GradientTex2, PlanetUtil.GenTexture(gradientLight));
        _Planet.SetMaterialProperty(ShaderProperties.GradientTex3, PlanetUtil.GenTexture(gradientDark));

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

        _Ring.SetMaterialProperty(ShaderProperties.GradientTex2, PlanetUtil.GenTexture(gradientLight));
        _Ring.SetMaterialProperty(ShaderProperties.GradientTex3, PlanetUtil.GenTexture(gradientDark));
    }

    public void EnableRing(bool enabled)
    {
        _Ring.SetEnabled(enabled);
    }

    public void UpdateMaterial()
    {
        _Planet.UpdateMaterial();
        _Ring.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Planet.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Ring.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f * -3f);
    }

    public void UpdateTime(float time)
    {
        _Planet.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f);
        _Ring.SetMaterialProperty(ShaderProperties.Timestamp, time * 0.5f * -3f);
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