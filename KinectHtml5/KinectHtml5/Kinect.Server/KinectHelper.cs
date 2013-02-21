using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.Research.Kinect.Nui;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace Kinect.Server
{
    /// <summary>
    /// Helper extension methods for skeleton joint scaling.
    /// Resource: http://c4fkinect.codeplex.com/SourceControl/changeset/view/70823#1215366.
    /// </summary>
    public static class KinectHelper
    {
        public static Joint ScaleTo(this Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
        {
            Vector pos = new Vector()
            {
                X = joint.Position.X * 1000f,//Scale(width, skeletonMaxX, joint.Position.X),
                Y = joint.Position.Y * 1000f,//Scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z * 1000f,//Scale(height, 1.0f, joint.Position.Z),
                W = joint.Position.W
            };

            Joint j = new Joint()
            {
                ID = joint.ID,
                TrackingState = joint.TrackingState,
                Position = pos
            };

            return j;
        }

        public static Joint ScaleTo(this Joint joint, int width, int height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }
    }
}
