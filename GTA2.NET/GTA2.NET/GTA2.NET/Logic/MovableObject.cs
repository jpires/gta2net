﻿//Created: 16.02.2010

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hiale.GTA2NET.Core.Map;
using Microsoft.Xna.Framework;
using Hiale.GTA2NET.Helper;

namespace Hiale.GTA2NET.Logic
{
    public class MovableObject
    {
        public static event EventHandler<GenericEventArgs<MovableObject>> ObjectCreated;
        public event EventHandler PositionChanged;
        public event EventHandler RotationChanged;

        /// <summary>
        /// Current position of this object. It represents the centre of the object.
        /// </summary>
        public Vector3 Position3 { get; protected set; }

        /// <summary>
        /// 2D position of the object.
        /// </summary>
        public Vector2 Position2
        {
            get
            {
                return new Vector2(Position3.X, Position3.Y);
            }
        }


        private Vector2 _origin;
        /// <summary>
        /// Origin of the object in 2D space.
        /// </summary>
        public Vector2 Origin
        {
            get
            {
                if (_origin == Vector2.Zero)
                    _origin = new Vector2(WidthHalf, HeightHalf);
                return _origin;
            }
            //set { _origin = value; }
        }

        private const float Circumference = 2 * (float)Math.PI;

        private float _rotationAngle;
        /// <summary>
        /// Current rotation angle in radians.
        /// </summary>
        public float RotationAngle
        {
            get { return _rotationAngle; }
            protected set
            {
                if (value < -Circumference)
                    value += Circumference;
                else if (value > Circumference)
                    value -= Circumference;
                _rotationAngle = value;
            }
        }

        /// <summary>
        /// 2D top left point of the object.
        /// </summary>
        public Vector2 TopLeft2
        {
            get
            {
                Vector2 topLeft = new Vector2(Position3.X - WidthHalf, Position3.Y - HeightHalf);
                return MainGame.RotatePoint(topLeft, Position2, RotationAngle);
            }
        }

        /// <summary>
        /// 3D top left point of the object.
        /// </summary>
        public Vector3 TopLeft3
        {
            get
            {
                Vector2 topLeft = TopLeft2;
                return new Vector3(topLeft.X, topLeft.Y, Position3.Z);
            }
        }
        

        /// <summary>
        /// 2D top right point of the object.
        /// </summary>
        public Vector2 TopRight2
        {
            get
            {
                Vector2 topRight = new Vector2(Position3.X + WidthHalf, Position3.Y - HeightHalf);
                return MainGame.RotatePoint(topRight, Position2, RotationAngle);
            }
        }

        /// <summary>
        /// 3D top right point of the object.
        /// </summary>
        public Vector3 TopRight3
        {
            get
            {
                Vector2 topRight = TopRight2;
                return new Vector3(topRight.X, topRight.Y, Position3.Z);
            }
        }

        /// <summary>
        /// 2D bottom right point of the object.
        /// </summary>
        public Vector2 BottomRight2
        {
            get
            {
                Vector2 bottomRight = new Vector2(Position3.X + WidthHalf, Position3.Y + HeightHalf);
                return MainGame.RotatePoint(bottomRight, Position2, RotationAngle);
            }
        }

        /// <summary>
        /// 3D bottom right point of the object.
        /// </summary>
        public Vector3 BottomRight3
        {
            get
            {
                Vector2 bottomRight = BottomRight2;
                return new Vector3(bottomRight.X, bottomRight.Y, Position3.Z);
            }
        }

        /// <summary>
        /// 2D bottom left point of the object.
        /// </summary>
        public Vector2 BottomLeft2
        {
            get
            {
                Vector2 bottomLeft = new Vector2(Position3.X - WidthHalf, Position3.Y + HeightHalf);
                return MainGame.RotatePoint(bottomLeft, Position2, RotationAngle);
            }
        }

        /// <summary>
        /// 3D bottom left point of the object.
        /// </summary>
        public Vector3 BottomLeft3
        {
            get
            {
                Vector2 bottomLeft = BottomLeft2;
                return new Vector3(bottomLeft.X, bottomLeft.Y, Position3.Z);
            }
        }

        /// <summary>
        /// Width of the object. Used by collision detection.
        /// </summary>
        public float Width { get; protected set; }

        protected float WidthHalf;

        /// <summary>
        /// Height of the object. Used by collision detection.
        /// </summary>
        public float Height { get; protected set; }

        protected float HeightHalf;

        /// <summary>
        /// Helper variable to calculate the distance moved.
        /// </summary>
        protected static readonly Vector2 OriginZero = Vector2.Zero;

        protected float Velocity;

        protected MovableObject(Vector3 startUpPosition, float width , float height)
        {
            Position3 = startUpPosition;
            Width = width / 64;
            WidthHalf = Width / 2;
            Height = height / 64;
            HeightHalf = Height / 2;
        }

        protected void CreateSprite()
        {
            if (ObjectCreated != null)
                ObjectCreated(this, new GenericEventArgs<MovableObject>(this));
        }

        /// <summary>
        /// Moves the object forward or backwards and changes the rotation angle.
        /// </summary>
        /// <param name="forwardDelta">Positive values mean 'go forward', negative 'go backward'</param>
        /// <param name="rotationDelta">ToDo</param>
        /// <param name="elapsedGameTime">The amount of elapsedGameTime since the last update</param>
        public void Move(ref float forwardDelta, ref float rotationDelta, ref float elapsedGameTime)
        {
            if (forwardDelta < 0) //Backwards
                rotationDelta *= -1;

            if (forwardDelta == 0)
                return;

            float rotationAngleNew = RotationAngle;
            rotationAngleNew += MathHelper.ToRadians(rotationDelta);
            Vector2 direction = MainGame.RotatePoint(new Vector2(0, forwardDelta), OriginZero, rotationAngleNew);

            //Bugs:
            //Low/High Slops only allow from proper edge!

            Vector2 topLeft = TopLeft2; //Create these for cache
            Vector2 topRight = TopRight2;
            Vector2 bottomRight = BottomRight2;
            Vector2 bottomLeft = BottomLeft2;
            CheckCollision(ref direction, ref topLeft, ref topRight, ref bottomRight, ref bottomLeft);
            
            //Culculate height, check all 4 points of the object and take the maximum value of those.
            float maxZ = float.MinValue;
            float currentHeight = Position3.Z;
            SetCorrectHeight(ref currentHeight, topLeft + direction);
            SetMaxF(ref maxZ, currentHeight);
            currentHeight = Position3.Z;
            SetCorrectHeight(ref currentHeight, topRight + direction);
            SetMaxF(ref maxZ, currentHeight);
            currentHeight = Position3.Z;
            SetCorrectHeight(ref currentHeight, bottomRight + direction);
            SetMaxF(ref maxZ, currentHeight);
            currentHeight = Position3.Z;
            SetCorrectHeight(ref currentHeight, bottomLeft + direction);
            SetMaxF(ref maxZ, currentHeight);

            if (maxZ < 0)
                maxZ = Position3.Z;

            RotationAngle = rotationAngleNew;

            //check wether this height is on empty space

            float newPositionX = Position3.X + direction.X;
            float newPositionY = Position3.Y + direction.Y;
            //ApplyGravity(ref newPositionX, ref newPositionY, ref weightedHeight);
            Position3 = new Vector3(newPositionX, newPositionY, maxZ);
            MainGame.WindowTitle = maxZ.ToString();

            if (PositionChanged != null)
                PositionChanged(this, EventArgs.Empty);
        }

        private void CheckCollision(ref Vector2 direction, ref Vector2 topLeft, ref Vector2 topRight, ref Vector2 bottomRight, ref Vector2 bottomLeft)
        {
            Vector2 newTopLeft = topLeft + direction;
            Vector2 newTopRight = topRight + direction;
            Vector2 newBottomRight = bottomRight + direction;
            Vector2 newBottomLeft = bottomLeft + direction;

            int minBlockX = (int)newTopLeft.X;
            int maxBlockX = minBlockX;
            int minBlockY = (int)newTopLeft.Y;
            int maxBlockY = minBlockY;
            SetMinMax(ref minBlockX, ref maxBlockX, newTopLeft.X);
            SetMinMax(ref minBlockY, ref maxBlockY, newTopLeft.Y);
            SetMinMax(ref minBlockX, ref maxBlockX, newTopRight.X);
            SetMinMax(ref minBlockY, ref maxBlockY, newTopRight.Y);
            SetMinMax(ref minBlockX, ref maxBlockX, newBottomRight.X);
            SetMinMax(ref minBlockY, ref maxBlockY, newBottomRight.Y);
            SetMinMax(ref minBlockX, ref maxBlockX, newBottomLeft.X);
            SetMinMax(ref minBlockY, ref maxBlockY, newBottomLeft.Y);

            int minBlockZ = (int)Position3.Z;
            int maxBlockZ = minBlockZ;
            minBlockZ = minBlockZ - 1;
            maxBlockZ = maxBlockZ + 1;
            if (minBlockZ < 0)
                minBlockZ = 0;
            if (maxBlockZ > 7)
                maxBlockZ = 7;

            for (int x = minBlockX; x < maxBlockX + 1; x++)
            {
                for (int y = minBlockY; y < maxBlockY + 1; y++)
                {
                    for (int z = minBlockZ; z < maxBlockZ + 1; z++)
                    {
                        CheckBlock(ref x, ref y, ref z, ref newTopLeft, ref newTopRight, ref newBottomRight, ref newBottomLeft, ref direction);
                    }
                }
            }
        }

        private void CheckBlock(ref int x, ref int y, ref int z, ref Vector2 newTopLeft, ref Vector2 newTopRight, ref Vector2 newBottomRight, ref Vector2 newBottomLeft, ref Vector2 direction)
        {
            BlockInfo block = MainGame.Map.CityBlocks[x, y, z];
            bool movableSlope = false; //a movable Slope is a block which actually intersecs with the object, but the object can move above it to change the height.
            bool blockAboveStops = false;
            BlockInfo blockAbove = null;
            if (!ProcessBlock(ref block, ref x, ref y, ref z, ref movableSlope, ref blockAboveStops, ref blockAbove))
                return;
            if (movableSlope || blockAboveStops)
            {
                Polygon polygonObject = new Polygon();
                polygonObject.Points.Add(newTopLeft);
                polygonObject.Points.Add(newTopRight);
                polygonObject.Points.Add(newBottomRight);
                polygonObject.Points.Add(newBottomLeft);

                BlockInfo blockPolygon = block;
                if (blockAboveStops)
                    blockPolygon = blockAbove;
                Polygon polygonBlock = CreateBlockPolygon(ref blockPolygon, ref x, ref y);
                PolygonCollisionResult collisionResult = SeparatingAxisTheorem.PolygonCollision(ref polygonObject, ref polygonBlock, ref direction);
                if (collisionResult.Intersect)
                {
                    if (block.IsMovableSlope)
                    {
                        if (AllowSlopeBlock(ref x, ref y, ref z, ref block, ref polygonObject))
                            return;
                    }
                    //direction.X = 0;
                    //direction.Y = 0;
                    direction.X = collisionResult.MinimumTranslationVector.X;
                    direction.Y = collisionResult.MinimumTranslationVector.Y;
                    return;
                }
            }
        }

        private static bool AllowSlopeBlock(ref int x, ref int y, ref int z, ref BlockInfo block, ref Polygon polygonObject)
        {
            Vector2 dummy = Vector2.Zero;

            //check the four edges of the block, these slops only some directions to come from...
            Polygon polygonTop = new Polygon();
            polygonTop.Points.Add(new Vector2(x, y));
            polygonTop.Points.Add(new Vector2(x + 1, y));

            Polygon polygonRight = new Polygon();
            polygonRight.Points.Add(new Vector2(x + 1, y));
            polygonRight.Points.Add(new Vector2(x + 1, y + 1));

            Polygon polygonBottom = new Polygon();
            polygonBottom.Points.Add(new Vector2(x + 1, y + 1));
            polygonBottom.Points.Add(new Vector2(x, y + 1));

            Polygon polygonLeft = new Polygon();
            polygonLeft.Points.Add(new Vector2(x, y + 1));
            polygonLeft.Points.Add(new Vector2(x, y));

            PolygonCollisionResult edgeCollisionResult = SeparatingAxisTheorem.PolygonCollision(ref polygonObject, ref polygonTop, ref dummy);
            bool collisionTop = edgeCollisionResult.Intersect;
            edgeCollisionResult = SeparatingAxisTheorem.PolygonCollision(ref polygonObject, ref polygonRight, ref dummy);
            bool collisionRight = edgeCollisionResult.Intersect;
            edgeCollisionResult = SeparatingAxisTheorem.PolygonCollision(ref polygonObject, ref polygonBottom, ref dummy);
            bool collisionBottom = edgeCollisionResult.Intersect;
            edgeCollisionResult = SeparatingAxisTheorem.PolygonCollision(ref polygonObject, ref polygonLeft, ref dummy);
            bool collisionLeft = edgeCollisionResult.Intersect;

            switch (block.SlopeType)
            {
                case SlopeType.Up26Low:
                case SlopeType.Up7Low:
                    if (!collisionTop && !collisionRight && collisionBottom && !collisionLeft)
                        return true;
                    break;
                case SlopeType.Down26Low:
                case SlopeType.Down7Low:
                    if (collisionTop && !collisionRight && !collisionBottom && !collisionLeft)
                        return true;
                    break;
                case SlopeType.Left26Low:
                case SlopeType.Left7Low:
                    if (!collisionTop && collisionRight && !collisionBottom && !collisionLeft)
                        return true;
                    break;
                case SlopeType.Right26Low:
                case SlopeType.Right7Low:
                    if (!collisionTop && !collisionRight && !collisionBottom && collisionLeft)
                        return true;
                    break;
            }
            return false;
        }

        private bool ProcessBlock(ref BlockInfo block, ref int x, ref int y, ref int z, ref bool movableSlope, ref bool blockAboveStops, ref BlockInfo blockAbove)
        {
            movableSlope = false;

            int currentZ = (int)Math.Round(Position3.Z, 0);
            
            //check the block above the current block. If this block is empty, process the current block.
            blockAbove = MainGame.Map.CityBlocks[x, y, currentZ + 1];
            if (z == currentZ && !blockAbove.IsMovableSlope)
            {
                if (!blockAbove.IsEmpty)
                    blockAboveStops = true;
                return true;
            }

            if (Position3.Z % 1 != 0) //07.03.2010, let's see if it works...
                return false;

            if (block.IsEmpty) //new 6.3.2010
                return false;

            //check one block above only if it's a low/high slope
            if (z == currentZ + 1)
            {
                if (block.IsMovableSlope)
                {
                    movableSlope = true;
                    return true;
                }
            }
            return false;
        }

        private void ApplyGravity(ref float x, ref float y, ref float z)
        {
            if (z == 0)
                return;
            BlockInfo block = MainGame.Map.CityBlocks[(int)x, (int)y, (int)z];
            if (block.Lid != null && block.Lid.TileNumber > 0)
                return;
            if (z % 1 == 0)
            {
                BlockInfo blockBelow = MainGame.Map.CityBlocks[(int)x, (int)y, (int)(z - 1)];
                if (blockBelow.IsEmpty)
                {
                    z -= 0.1f;
                }
            }
        }

        private static void SetMinMax(ref int minBlock, ref int maxBlock, float currentValue)
        {
            if (currentValue < minBlock)
                minBlock = (int)currentValue;
            if (currentValue > maxBlock)
                maxBlock = (int)currentValue;
        }

        private static void SetMaxF(ref float maxBlock, float currentValue)
        {
            if (currentValue > maxBlock)
                maxBlock = currentValue;
        }

        private static Polygon CreateBlockPolygon(ref BlockInfo block, ref int x, ref int y)
        {
            Polygon polygon = new Polygon();
            SlopeType slope = block.SlopeType;

            switch (slope)
            {
                case SlopeType.None:
                    polygon.Points.Add(new Vector2(x, y));
                    polygon.Points.Add(new Vector2(x + 1, y));
                    polygon.Points.Add(new Vector2(x + 1, y + 1));
                    polygon.Points.Add(new Vector2(x, y + 1));
                    break;
                case SlopeType.DiagonalFacingDownLeft:
                    polygon.Points.Add(new Vector2(x, y));
                    polygon.Points.Add(new Vector2(x + 1, y));
                    polygon.Points.Add(new Vector2(x + 1, y + 1));
                    break;
                case SlopeType.DiagonalFacingDownRight:
                    polygon.Points.Add(new Vector2(x, y));
                    polygon.Points.Add(new Vector2(x + 1, y));
                    polygon.Points.Add(new Vector2(x, y + 1));
                    break;
                case SlopeType.DiagonalFacingUpLeft:
                    polygon.Points.Add(new Vector2(x + 1, y));
                    polygon.Points.Add(new Vector2(x + 1, y + 1));
                    polygon.Points.Add(new Vector2(x, y + 1));
                    break;
                case SlopeType.DiagonalFacingUpRight:
                    polygon.Points.Add(new Vector2(x, y));
                    polygon.Points.Add(new Vector2(x + 1, y + 1));
                    polygon.Points.Add(new Vector2(x, y + 1));
                    break;
                default: //ToDo: implement all slopes!
                    polygon.Points.Add(new Vector2(x, y));
                    polygon.Points.Add(new Vector2(x + 1, y));
                    polygon.Points.Add(new Vector2(x + 1, y + 1));
                    polygon.Points.Add(new Vector2(x, y + 1));
                    break;
            }
            return polygon;
        }

        private static void SetCorrectHeight(ref float value, Vector2 point)
        {
            float x = point.X;
            float y = point.Y;
            int z = (int)Math.Round(value, 0);
            BlockInfo blockAbove = MainGame.Map.CityBlocks[(int)point.X, (int)point.Y, z + 1];
            if (blockAbove.IsMovableSlope)
                z++;
            BlockInfo blockBelow = MainGame.Map.CityBlocks[(int)point.X, (int)point.Y, z - 1];
            if (blockBelow.IsMovableSlope)
                z--;
            float newValue = MainGame.GetHeightF(ref x, ref y, ref z);
            if (newValue != value)
            {
                value = newValue;
            }
        }
    }
}
