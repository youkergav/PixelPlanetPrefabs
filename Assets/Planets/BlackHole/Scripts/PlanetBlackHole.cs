using UnityEngine;

[ExecuteInEditMode]
public class PlanetBlackHole : MonoBehaviour, PlanetInterface
{
    [Header("Transform")]
    [Range(0f, 2f)] public float Size = 1.0f;
    [Range(0f, 6.28f)] public float Rotation = 3.75f;
    [Range(-1f, 1f)] public float Speed = 0.5f;

    [Header("Colors")]
    public Gradient HoleColor;
    public Gradient DiskColor;

    [Header("Seeds")]
    [Range(1, 100)] public int DiskSeed = 1;

    [Header("Misc")]
    [Range(0, 256)] public int Pixels = 128;

    [HideInInspector] public PlanetLayer Hole;
    [HideInInspector] public PlanetLayer Disk;

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
        SetSpeed();

        UpdateMaterial();
    }

    public bool Initialize()
    {
        SpriteRenderer holeRenderer = transform.Find("Hole").GetComponent<SpriteRenderer>();
        SpriteRenderer diskRenderer = transform.Find("Disk").GetComponent<SpriteRenderer>();

        Material holeMaterial = new Material(holeRenderer.sharedMaterial);
        Material diskMaterial = new Material(diskRenderer.sharedMaterial);

        Hole = new PlanetLayer(gameObject, holeRenderer, holeMaterial);
        Disk = new PlanetLayer(gameObject, diskRenderer, diskMaterial);

        return true;
    }

    public void SetSeed()
    {
        Disk.SetMaterialProperty(ShaderProperties.Seed, DiskSeed);
    }

    public void SetPixels(float ppu)
    {
        Hole.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        Disk.SetMaterialProperty(ShaderProperties.Pixels, ppu * 3);
    }

    public void SetLight(Vector2 position)
    {
        Hole.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        Disk.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        Hole.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        Disk.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);
        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        Disk.SetMaterialProperty(ShaderProperties.Speed, Speed);
    }

    public void SetColors()
    {
        // Set the hole colors.
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[2];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];

        float[] oldTimes = { 0.50f, 1f };
        float[] newTimes = { 0f, 1f };

        for (int i = 0; i < colorKey.Length; i++)
        {
            colorKey[i].color = HoleColor.Evaluate(oldTimes[i]);
            colorKey[i].time = newTimes[i];

            alphaKey[i].alpha = 1;
            alphaKey[i].time = newTimes[i];
        }
        gradient.SetKeys(colorKey, alphaKey);

        Hole.SetMaterialProperty(ShaderProperties.Color, HoleColor.Evaluate(0));
        Hole.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(gradient));

        // Set the disk colors.
        Disk.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(DiskColor));
    }

    public void UpdateMaterial()
    {
        Hole.UpdateMaterial();
        Disk.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        Hole.SetMaterialProperty(ShaderProperties.Timestamp, time);
        Disk.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    public void UpdateTime(float time)
    {
        Hole.SetMaterialProperty(ShaderProperties.Timestamp, time);
        Disk.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
