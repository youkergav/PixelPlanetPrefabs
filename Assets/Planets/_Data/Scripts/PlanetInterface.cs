using UnityEngine;

public interface PlanetInterface
{
    void SetSeed();
    void SetPixels(float ppu);
    void SetLight(Vector2 position);
    void SetRotate(float rotation);
    void SetSize(float size);
    void SetSpeed();
    void SetColors();
    void UpdateMaterial();
    void SetStartTime(float time);
    void UpdateTime(float time);
}
