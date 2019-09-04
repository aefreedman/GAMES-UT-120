using UnityEngine;

namespace ScriptableObjects
{
    public class TunableBall : TunableObject
    {
        public override void UpdateTunables()
        {
            Rigidbody2D.drag = Settings.Ball.Rigidbody.Drag;
            Rigidbody2D.angularDrag = Settings.Ball.Rigidbody.AngularDrag;
            Rigidbody2D.mass = Settings.Ball.Rigidbody.Mass;
            Rigidbody2D.gravityScale = Settings.Ball.Rigidbody.GravityScale;
            if (Settings.PhysicsMaterials == null || Settings.PhysicsMaterials.Length <= 0) return;
            if (Settings.Ball.Rigidbody.PhysicsMaterialIndex < Settings.PhysicsMaterials.Length)
                Rigidbody2D.sharedMaterial = Settings.PhysicsMaterials[Settings.Ball.Rigidbody.PhysicsMaterialIndex];
        }
    }
}