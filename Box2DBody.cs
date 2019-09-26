using System;
using System.Collections.Generic;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace NoZ.Platform.Box2D
{
    public class Box2DBody : Physics.IBody
    {
        internal Body _body;

        public NoZ.Physics.CollisionEnterDelegate OnCollisionEnter { get;  set; }

        Vector2 Physics.IBody.LinearVelocity {
            get => new Vector2(_body.LinearVelocity.X, _body.LinearVelocity.Y);
            set => _body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(value.x, value.y);
        }

        public Box2DBody(World world, BodyType bodyType)
        {
            _body = new Body(world);
            _body.UserData = this;
            _body.BodyType = bodyType;
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            return OnCollisionEnter != null ? OnCollisionEnter() : false;
        }

        public Physics.PhysicsLayer Layers {
            set => _body.CollisionCategories = (Category)value;
        }

        public Physics.PhysicsLayer CollidesWithLayers {
            set => _body.CollidesWith = (Category)value;
        }

        Vector2 Physics.IBody.Position {
            get => new Vector2(_body.Position.X, _body.Position.Y);
            set {
                _body.Position = new Microsoft.Xna.Framework.Vector2(value.x, value.y);
            }
        }

        Physics.ICollider Physics.IBody.AddBoxCollider(in Vector2 position, in Vector2 size)
        {
            var collider = new Box2DCollider(this, position, size);
            _body.OnCollision += OnCollision;
            return collider;
        }

        Physics.ICollider Physics.IBody.AddCircleCollider(in Vector2 position, float radius)
        {
            var collider = new Box2DCollider(this, position, radius);
            _body.OnCollision += OnCollision;
            return collider;
        }

        public void Dispose()
        {
            _body?.Dispose();
            _body = null;
        }
    }
}
