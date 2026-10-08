using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Mario/EnemySettings")]
public class EnemySettings : ScriptableObject
{
    [Header("Piranha Plant")]
    public float PiranhaPlantOffsetY = 1.5f;
    public float PiranhaPlantAnimationDuration = 0.75f;
    public float PiranhaPlantActiveDuration = 2.5f;
    public float PiranhaPlantHiddenDurationMin = 2.0f;
    public float PiranhaPlantHiddenDurationMax = 4.0f;

    [Header("Fire Piranha Plant")]
    public float FirePiranhaPlantOffsetY = 2.5f;
    public float FirePiranhaPlantAnimationDuration = 0.75f;
    public float FirePiranhaPlantActiveDuration = 2.5f;
    public float FirePiranhaPlantHiddenDurationMin = 2.0f;
    public float FirePiranhaPlantHiddenDurationMax = 4.0f;

    [Header("Goomba")]
    public float GoombaSpeed = 3.0f;
    public float GoombaSquishedDuration = 0.6f;
}
