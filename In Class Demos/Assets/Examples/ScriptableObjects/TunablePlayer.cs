using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    public class TunablePlayer : TunableObject
    {
        private List<Rigidbody2D> _ballContacts;

        protected override void Start()
        {
            base.Start();
            _ballContacts = new List<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Rigidbody2D.velocity = InputHandler.Instance.InputAxis * Settings.Player.Speed;

            if (Input.GetKeyDown(Settings.Player.KickButton))
                Kick();

            _ballContacts.Clear();
        }

        private void Kick()
        {
            // List<T>.ForEach() is equivalent to using a foreach loop using the List
            // The parameter passed into the method is an Anonymous Method
            _ballContacts.ForEach(ball => { ball.AddForce((ball.position - Rigidbody2D.position).normalized * Settings.Player.KickForce, ForceMode2D.Impulse); });

//            foreach (var ballContact in _ballContacts)
//            {
//                ballContact.AddForce((ballContact.position - Rigidbody2D.position).normalized * Settings.Player.KickForce, ForceMode2D.Impulse);;
//            }
        }

        public override void UpdateTunables()
        {
            Rigidbody2D.drag = Settings.Player.Rigidbody.Drag;
            Rigidbody2D.angularDrag = Settings.Player.Rigidbody.AngularDrag;
            Rigidbody2D.mass = Settings.Player.Rigidbody.Mass;
            Rigidbody2D.gravityScale = Settings.Player.Rigidbody.GravityScale;
            if (Settings.PhysicsMaterials == null || Settings.PhysicsMaterials.Length <= 0) return;
            if (Settings.Player.Rigidbody.PhysicsMaterialIndex < Settings.PhysicsMaterials.Length)
                Rigidbody2D.sharedMaterial = Settings.PhysicsMaterials[Settings.Player.Rigidbody.PhysicsMaterialIndex];
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("PickUp"))
                _ballContacts.Add(other.attachedRigidbody);
        }
    }
}