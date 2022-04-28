using UnityEngine;

[CreateAssetMenu(fileName = "Blade", menuName = "ScriptableObjects/BladeSettings", order = 1)]
public class BladeSettings : ScriptableObject
{
    [Header("Health/Damage settings")]
    public float maxHealth;
    public float wallDmg, bladeDmg, smallHitDmg, dmgRange, healthRange;


    [Header("Force settings")]
    public float wallForce;
    public float bladeForce;
    public float constForce;
    public float spinRotation;
    public float smallHitForce;
    public float relSpeedCap;
    public float stayTime;
    public float initialForce;


    [Header("Animation settings")]
    public float spinSpeed;

}