using System;

public static class GameEvents
{
    // M1
    public static event Action<int> OnLaneChange;      // -1 gauche, +1 droite
    public static event Action<int> OnDash;            // -1 gauche, +1 droite
    // M2
    public static event Action<float> OnJump;          // puissance 0..1
    public static event Action OnSlide;
    // M3
    public static event Action OnSprint;
    public static event Action<float> OnEnduranceBoost; // 0..1
    // M4
    public static event Action OnBonusPose;
    public static event Action<bool> OnNightMode;
    // M5
    public static event Action OnShield;
    public static event Action<float> OnDistanceUnlock; // mètres

    public static void LaneChange(int dir) => OnLaneChange?.Invoke(dir);
    public static void Dash(int dir) => OnDash?.Invoke(dir);
    public static void Jump(float power) => OnJump?.Invoke(power);
    public static void Slide() => OnSlide?.Invoke();
    public static void Sprint() => OnSprint?.Invoke();
    public static void EnduranceBoost(float v) => OnEnduranceBoost?.Invoke(v);
    public static void BonusPose() => OnBonusPose?.Invoke();
    public static void NightMode(bool on) => OnNightMode?.Invoke(on);
    public static void Shield() => OnShield?.Invoke();
    public static void DistanceUnlock(float m) => OnDistanceUnlock?.Invoke(m);
}