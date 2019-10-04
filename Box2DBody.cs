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
    public class Box2DBody : Physics.IBody
    {
        internal Body _body;

        public NoZ.Physics.CollisionEnterDelegate OnCollisionEnter { get;  set; }

        public NoZ.Object UserData { get; set; }

        public bool IsBullet {
            get => _body.IsBullet;
            set => _body.IsBullet = value;
        }

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
            if (null == OnCollisionEnter)
                return true;

            var collision = new Physics.Collision();
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

        Physics.ICollider Physics.IBody.AddPolygonCollider(in Vector2 position, in Vector2[] points)
        {
            var collider = new Box2DCollider(this, position, points);
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
