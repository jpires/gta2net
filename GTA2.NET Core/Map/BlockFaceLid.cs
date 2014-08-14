// GTA2.NET
// 
// File: BlockFaceLid.cs
// Created: 27.02.2013
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
using System.IO;
using Hiale.GTA2NET.Core.Helper;
using System;

namespace Hiale.GTA2NET.Core.Map
{
    /// <summary>
    /// Used to represent the lid of a Block
    /// </summary>
    public class BlockFaceLid : BlockFace
    {
        /// <summary>
        /// Lighting level marks which shading level to apply to a lid tile. 0 is normal brightness. 1-3 are increasing levels of darkness.
        /// </summary>
        public byte LightningLevel { get; private set; }

        /// <summary>
        /// Creates an instance of BlockFaceEdge with the default values.
        /// </summary>
        public BlockFaceLid() : base()
        {
            LightningLevel = 0;
        }

        /// <summary>
        /// Creates an instance of BlockFaceEdge
        /// </summary>
        /// <param name="value">The value read from the original map format, it will construct the Block with the correct values.</param>
        public BlockFaceLid(ushort value) : base(value)
        {
            Boolean bit10 = BitHelper.CheckBit(value, 10);
            Boolean bit11 = BitHelper.CheckBit(value, 11);

            if (!bit10 && !bit11)
            {
                LightningLevel = 0;
                return;
            }
            if (bit10 && !bit11)
            {
                LightningLevel = 1;
                return;
            }
            if (!bit10 && bit11)
            {
                LightningLevel = 2;
                return;
            }
            if (bit10 && bit11)
            {
                LightningLevel = 3;
                return;
            }
        }
    }
}
