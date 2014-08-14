// GTA2.NET
// 
// File: Pedestrian.cs
// Created: 16.02.2010
// 
// 
// Copyright (C) 2010-2013 Hiale
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
// is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies
// or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Grand Theft Auto (GTA) is a registred trademark of Rockstar Games.

using Hiale.GTA2NET.Core.Physics;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using Microsoft.Xna.Framework;
using System;

namespace Hiale.GTA2NET.Core.Logic
{
    /// <summary>
    /// Used to represent a Pedestrian.
    /// </summary>
    public class Pedestrian : ControlableGameObject
    {
        /// <summary>
        /// Creates an instance of Pedestrian.
        /// </summary>
        /// <param name="startUpPosition">The star position of the Pedestrian.</param>
        public Pedestrian(Vector3 startUpPosition) : base(startUpPosition, 0, new Helper.CompactRectangle(0, 0, 1, 1))
        {
        }

        /// <summary>
        /// Updates the state of the Object.
        /// </summary>        
        /// <param name="input">The Input to apply to the Object.</param>
        public override void Update(float elapsedTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates the state of the Object.
        /// </summary>        
        /// <param name="input">The Input to apply to the Object.</param>
        /// <param name="elapsedTime">The time occurred since the last Update.</param>
        public override void Update(ParticipantInput input, float elapsedTime)
        {
            if (input.Forward != 0)
            {
                if(input.Forward > 0)
                    Controller.Direction = Walk.Forward;
                else
                    Controller.Direction = Walk.Backward;
            }
            else
            {
                Controller.Direction = Walk.None;
            }

            if (input.Rotation != 0)
            {
                if (input.Rotation > 0)
                    Controller.Rotate = Rotate.Clockwise;
                else
                    Controller.Rotate = Rotate.Anticlockwise;
            }
            else
            {
                Controller.Rotate = Rotate.None;
            }

            Position3 = new Vector3(body.Position.X, body.Position.Y, body.Position.Z);

            // Calculate the angle on Z.
            // Seen on http://nghiaho.com/?page_id=846
            float r21 = body.Orientation.M21;
            float r11 = body.Orientation.M11;
            float angle = (float)Math.Atan2(r21, r11);
      
            RotationAngle = angle;
        }


        public PedestrianController Controller { get; set; }

        private RigidBody body;

        public RigidBody Body
        {
            get
            {
                if (body == null)
                    createBody();

                return body;
            }
            private set
            {
                body = value;
            }
        }

        private void createBody()
        {
            //ToDo: set the size in a more adjustable way.
            body = new RigidBody(new BoxShape(1.0f, 0.5f, 0.5f));
            body.Position = new JVector(Position3.X, Position3.Y, Position3.Z);

            JMatrix innertia = JMatrix.Identity;
            body.SetMassProperties(innertia, 1, true);
            body.IsActive = true;
            body.AffectedByGravity = true;
        }
    }
}
