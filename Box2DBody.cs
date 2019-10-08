/*
  NoZ Game Engine

  Copyright(c) 2019 NoZ Games, LLC

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files(the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions :

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace NoZ.Platform.Box2D
{
    public class Box2DBody : IBody
    {
        internal Body _body;

        public CollisionEnterDelegate OnCollisionEnter { get;  set; }

        public NoZ.Object UserData { get; set; }

        public bool IsBullet {
            get => _body.IsBullet;
            set => _body.IsBullet = value;
        }

        public bool IsSensor {
            set => _body.IsSensor = value;
        }

        Vector2 IBody.LinearVelocity {
            get => new Vector2(_body.LinearVelocity.X, _body.LinearVelocity.Y);
            set => _body.LinearVelocity = new Microsoft.Xna.Framework.Vector2(value.x, value.y);
        }

        float IBody.LinearDamping {
            get => _body.LinearDamping;
            set => _body.LinearDamping = value;
        }

        bool IBody.IsEnabled {
            get => _body.Enabled;
            set => _body.Enabled = value;
        }

        bool IBody.IsKinematic {
            set => _body.IsKinematic = value;
        }

        public Box2DBody(FarseerPhysics.Dynamics.World world, BodyType bodyType)
        {
            _body = new Body(world);
            _body.UserData = this;
            _body.BodyType = bodyType;
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (null == OnCollisionEnter)
                return true;

            var collision = new Collision();
            if (fixtureA.Body == _body)
            {
                collision.Collider = (fixtureA.UserData as Box2DCollider).Node;
                collision.OtherCollider = (fixtureB.UserData as Box2DCollider).Node;
            }
            else
            {
                collision.OtherCollider = (fixtureA.UserData as Box2DCollider).Node;
                collision.Collider = (fixtureB.UserData as Box2DCollider).Node;
            }
            OnCollisionEnter(collision);

            return true;
        }

        public uint CollisionMask {
            set => _body.CollisionCategories = (Category)value;
        }

        public uint CollidesWithMask {
            set => _body.CollidesWith = (Category)value;
        }

        void IBody.ApplyForce(in Vector2 force)
        {
            _body.ApplyForce(force.ToXna());
        }

        Vector2 IBody.Position {
            get => new Vector2(_body.Position.X, _body.Position.Y);
            set {
                _body.Position = new Microsoft.Xna.Framework.Vector2(value.x, value.y);
            }
        }

        ICollider IBody.AddBoxCollider(in Vector2 position, in Vector2 size)
        {
            var collider = Box2DCollider.CreateBox(this, position, size);
            _body.OnCollision += OnCollision;
            return collider;
        }

        ICollider IBody.AddCircleCollider(in Vector2 position, float radius)
        {
            var collider = Box2DCollider.CreateCircle(this, position, radius);
            _body.OnCollision += OnCollision;
            return collider;
        }

        ICollider IBody.AddPolygonCollider(in Vector2 position, in Vector2[] points)
        {
            var collider = Box2DCollider.CreatePolygon(this, position, points);
            _body.OnCollision += OnCollision;
            return collider;
        }

        ICollider IBody.AddEdgeCollider (in Vector2 start, in Vector2 end)
        {
            var collider = Box2DCollider.CreateEdge (this, start, end);
            _body.OnCollision += OnCollision;
            return collider;
        }

        ICollider IBody.AddChainCollider(in Vector2 position, Vector2[] points, bool loop)
        {
            var collider = Box2DCollider.CreateChain(this, position, points, loop);
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
