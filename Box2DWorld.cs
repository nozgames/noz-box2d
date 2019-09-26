﻿/*
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

namespace NoZ.Platform.Box2D
{
    public class Box2DWorld : Physics.IWorld
    {
        private World _world;

        public Box2DWorld()
        {
            _world = new World(new Microsoft.Xna.Framework.Vector2(0));
        }

        public Physics.IBody CreateRigidBody()
        {
            return new Box2DBody(_world, BodyType.Dynamic);
        }

        public Physics.IBody CreateStaticBody()
        {
            return new Box2DBody(_world, BodyType.Static);
        }

        public Physics.IBody CreateKinematicBody()
        {
            return new Box2DBody(_world, BodyType.Kinematic);
        }

        void Physics.IWorld.Step(float deltaTime) => _world.Step(deltaTime);

        public void RemoveRigidBody(Physics.IBody body)
        {
            (body as Box2DBody).Dispose();
        }

        void Physics.IWorld.DrawDebug(NoZ.GraphicsContext gc, Physics.PhysicsLayer layers)
        {
            gc.SetImage(null);
            gc.SetTransform(Matrix3.Identity);
            foreach (var body in _world.BodyList)
                foreach (var fixture in body.FixtureList)
                    if((fixture.CollisionCategories & (Category)layers) != 0)
                        (fixture.UserData as Box2DCollider).DrawDebug(gc);
        }

        public void Dispose()
        {
            _world = null;
        }
    }
}