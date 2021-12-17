using UnityEngine;

public class PlanetLayer
{
    private GameObject _GameObject;
    private SpriteRenderer _Renderer;
    private Material _Material;

    public PlanetLayer(GameObject gameObject, SpriteRenderer renderer, Material material)
    {
        _GameObject = gameObject;
        _Renderer = renderer;
        _Material = material;
    }

    public void SetEnabled(bool enabled)
    {
        _Renderer.enabled = enabled;
    }

    public void SetMaterialProperty(string key, float value)
    {
        if (isPrefab())
        {
            _Renderer.sharedMaterial.SetFloat(key, value);
        }
        else
        {
            _Material.SetFloat(key, value);
        }
    }

    public void SetMaterialProperty(string key, Vector2 value)
    {
        if (isPrefab())
        {
            _Renderer.sharedMaterial.SetVector(key, value);
        }
        else
        {
            _Material.SetVector(key, value);
        }
    }

    public void SetMaterialProperty(string key, Color value)
    {
        if (isPrefab())
        {
            _Renderer.sharedMaterial.SetColor(key, value);
        }
        else
        {
            _Material.SetColor(key, value);
        }
    }

    public void SetMaterialProperty(string key, Texture2D value)
    {
        if (isPrefab())
        {
            _Renderer.sharedMaterial.SetTexture(key, value);
        }
        else
        {
            _Material.SetTexture(key, value);
        }
    }

    public void UpdateMaterial()
    {
        if (!isPrefab())
        {
            _Renderer.material = _Material;
        }
    }

    // This is a hack because I am not smart enough to figure it out.
    private bool isPrefab()
    {
        return _GameObject.scene.name == null || _GameObject.scene.name == _GameObject.name;
    }
}