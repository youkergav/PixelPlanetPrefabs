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
    [Range(1f, 10f)] public float HoleSeed = 1f;
    [Range(1f, 10f)] public float DiskSeed = 1f;

    [Header("Misc")]
    [Range(0, 256)] public int Pixels = 128;

    public bool Initiated
    {
        get
        {
            return _Initiated;
        }
    }

    private PlanetLayer _Hole;
    private PlanetLayer _Disk;

    private bool _Initiated = false;
    private float _Timestamp = 0f;

    private void Awake()
    {
        SpriteRenderer holeRenderer = transform.Find("Hole").GetComponent<SpriteRenderer>();
        SpriteRenderer diskRenderer = transform.Find("Disk").GetComponent<SpriteRenderer>();

        Material holeMaterial = new Material(holeRenderer.sharedMaterial);
        Material diskMaterial = new Material(diskRenderer.sharedMaterial);

        _Hole = new PlanetLayer(gameObject, holeRenderer, holeMaterial);
        _Disk = new PlanetLayer(gameObject, diskRenderer, diskMaterial);

        SetSeed();
        SetColors();
        SetPixels(Pixels);
        SetSize(Size);
        SetRotate(Rotation);
        SetSpeed();

        _Initiated = true;
        UpdateMaterial();
    }

    public void SetSeed()
    {
        _Hole.SetMaterialProperty(ShaderProperties.Seed, HoleSeed);
        _Disk.SetMaterialProperty(ShaderProperties.Seed, DiskSeed);
    }

    public void SetPixels(float ppu)
    {
        _Hole.SetMaterialProperty(ShaderProperties.Pixels, ppu);
        _Disk.SetMaterialProperty(ShaderProperties.Pixels, ppu * 3);
    }

    public void SetLight(Vector2 position)
    {
        _Hole.SetMaterialProperty(ShaderProperties.LightOrigin, position);
        _Disk.SetMaterialProperty(ShaderProperties.LightOrigin, position);
    }

    public void SetRotate(float rotation)
    {
        _Hole.SetMaterialProperty(ShaderProperties.Rotation, rotation);
        _Disk.SetMaterialProperty(ShaderProperties.Rotation, rotation);
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, transform.localScale.z);
        SetPixels(Pixels * size);
    }

    public void SetSpeed()
    {
        _Disk.SetMaterialProperty(ShaderProperties.Speed, Speed);
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

        _Hole.SetMaterialProperty(ShaderProperties.Color, HoleColor.Evaluate(0));
        _Hole.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(gradient));

        // Set the disk colors.
        _Disk.SetMaterialProperty(ShaderProperties.GradientTex, PlanetUtil.GenTexture(DiskColor));
    }

    public void UpdateMaterial()
    {
        _Hole.UpdateMaterial();
        _Disk.UpdateMaterial();
    }

    public void SetStartTime(float start)
    {
        float time = 10f + start * 60f;

        _Hole.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Disk.SetMaterialProperty(ShaderProperties.Timestamp, time);
    }

    public void UpdateTime(float time)
    {
        _Hole.SetMaterialProperty(ShaderProperties.Timestamp, time);
        _Disk.SetMaterialProperty(ShaderProperties.Timestamp, time);
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
