using System;
using UnityEngine;

// ReSharper disable NotAccessedField.Global

//https://docs.unity3d.com/Manual/class-ScriptableObject.html
//https://docs.unity3d.com/ScriptReference/ScriptableObject.html

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Game")]
    public float GameTime;

    [Range(0, 200)]
    public int Balls;

    [Header("Player Settings")]
    public PlayerSettings Player;

    [Header("Ball Settings")]
    public BallSettings Ball;

    [Header("Physics Materials")]
    public PhysicsMaterial2D[] PhysicsMaterials;

    // If you want to create a nested structure in your Scriptable object
    // you need to use a struct
    [Serializable]
    public struct PlayerSettings
    {
        public GameObject PlayerPrefab;
        public KeyCode KickButton;
        public RigidbodySettings Rigidbody;
        public float Speed;
        public float KickForce;
    }

    [Serializable]
    public struct BallSettings
    {
        public GameObject BallPrefab;
        public RigidbodySettings Rigidbody;
    }

    [Serializable]
    public struct RigidbodySettings
    {
        public float AngularDrag;
        public float Drag;
        public float GravityScale;
        public float Mass;
        public int PhysicsMaterialIndex;
    }
}