using System;
using System.Linq;
using DuckGame;

namespace MClient.Core.Utils
{
    
    /// <summary>
    /// A collection of useful maths and math-related functions.
    /// </summary>
    public static class MMathUtils
    {

        //Multiply a value in degrees by this number to convert to (approximate) radians.
        public const float DegToRad = (float)0.0174533;

        //Multiply a value in radians by this number to convert to (approximate) degrees.
        public const float RadToDeg = (float)57.2958;

        /*
         Mostly for internal use, this is a precision limiter value.
         Values below this amount will be considered to be zero in value comparisons (in this class)
        */
        public const float Epsilon = (float)0.00001;

        /// <summary>
        /// Calculates the position that a line would hit a object at.
        /// </summary>
        /// <param name="start">The start position for the line, in Game/World space</param>
        /// <param name="end">The end position for the line, in Game/World space</param>
        /// <param name="thing">The <see cref="Thing"/> to check a hit against</param>
        /// <returns>
        /// The point at which the line would hit the object,
        /// or the end point of the line if it does not intersect the object.
        /// </returns>
        public static Vec2 CalcHitPoint(Vec2 start, Vec2 end, Thing thing)
        {
            if (thing is null) return end;
            
            Vec2[] intersects = new Vec2[] {
            CalcIntersection(start, end, thing.topLeft, thing.bottomLeft),
            CalcIntersection(start, end, thing.topLeft, thing.topRight),
            CalcIntersection(start, end, thing.topRight, thing.bottomRight),
            CalcIntersection(start, end, thing.bottomRight, thing.bottomLeft)
            };

            var nearest = end;
            float distance = (end - start).length;

            foreach (var vec in intersects)
            {
                if (float.IsNaN(vec.x) || float.IsNaN(vec.y))
                {
                    continue;
                }

                float dist = (vec - start).length;
                if (!(dist < distance)) continue;
                distance = dist;
                nearest = vec;

            }

            return nearest;
        }

        /// <summary>
        /// Calculates the intersection point between two lines.
        /// </summary>
        /// <param name="startA">Start position of the first line.</param>
        /// <param name="endA">End position of the first line.</param>
        /// <param name="startB">Start position of the second line.</param>
        /// <param name="endB">End position of the second line.</param>
        /// <param name="zeroIfNone">
        /// Whether to return zero if there isn't an intersection.
        /// If false, the method may return NaN or a incorrect value if the lines do not intersect.
        /// </param>
        /// <returns>The intersection point between the two lines</returns>
        public static Vec2 CalcIntersection(Vec2 startA, Vec2 endA, Vec2 startB, Vec2 endB, bool zeroIfNone = true)
        {
            float a1 = endA.y - startA.y;
            float b1 = startA.x - endA.x;
            float c1 = a1 * startA.x + b1 * startA.y;

            float a2 = endB.y - startB.y;
            float b2 = startB.x - endB.x;
            float c2 = a2 * startB.x + b2 * startB.y;

            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                return Vec2.Zero;
            }

            var intersect = new Vec2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

            if (!zeroIfNone)
            {
                return intersect;
            }
            
            if (PointOnLine(intersect, startA, endA) && PointOnLine(intersect, startB, endB))
            {
                return intersect;
            }

            return Vec2.Zero;
        }

        /// <summary>
        /// Checks whether a given position is on a given line.
        /// </summary>
        /// <param name="point">The position to check</param>
        /// <param name="start">The start position of the line</param>
        /// <param name="end">The end position of the line</param>
        /// <returns>Whether the position is on the line</returns>
        public static bool PointOnLine(Vec2 point, Vec2 start, Vec2 end)
        {
            float dist1 = Dist(start, point);
            float dist2 = Dist(end, point);
            float dist3 = Dist(start, end);
            float dist4 = dist1 + dist2;

            return dist4 - dist3 <= Epsilon;
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="pointA">The first position</param>
        /// <param name="pointB">The second position</param>
        /// <returns>The distance between the two points</returns>
        public static float Dist(Vec2 pointA, Vec2 pointB)
        {
            return (pointA - pointB).Length();
        }

        /// <summary>
        /// Calculates the squared distance between two points.
        /// </summary>
        /// <param name="pointA">The first position</param>
        /// <param name="pointB">The second position</param>
        /// <returns>The squared distance between the two points</returns>
        public static float DistSqr(Vec2 pointA, Vec2 pointB)
        {
            return (pointA - pointB).LengthSquared();
        }

        /// <summary>
        /// Calculates the perpendicular to a direction vector, rotating Clockwise
        /// </summary>
        /// <param name="vec">The direction vector</param>
        /// <returns>A vector that has the same magnitude as the given vector, but points 90 degrees clockwise of it.</returns>
        public static Vec2 CalcPerpendicularCw(Vec2 vec)
        {
            return new Vec2(vec.y, -vec.x);
        }

        /// <summary>
        /// Calculates the perpendicular to a direction vector, rotating CounterClockwise
        /// </summary>
        /// <param name="vec">The direction vector</param>
        /// <returns>A vector that has the same magnitude as the given vector, but points 90 degrees counter-clockwise of it.</returns>
        public static Vec2 CalcPerpendicularCcw(Vec2 vec)
        {
            return new Vec2(-vec.y, vec.x);
        }

        /// <summary>
        /// Calculates the perpendicular to a line, rotating Clockwise
        /// </summary>
        /// <param name="start">The start position of the line</param>
        /// <param name="end">The end position of the line</param>
        /// <returns>A vector that points 90 degrees clockwise of the given line.</returns>
        public static Vec2 CalcPerpendicularCw(Vec2 start, Vec2 end)
        {
            return CalcPerpendicularCw(end - start);
        }

        /// <summary>
        /// Calculates the perpendicular to a line, rotating CounterClockwise
        /// </summary>
        /// <param name="start">The start position of the line</param>
        /// <param name="end">The end position of the line</param>
        /// <returns>A vector that points 90 degrees counter-clockwise of the given line.</returns>
        public static Vec2 CalcPerpendicularCcw(Vec2 start, Vec2 end)
        {
            return CalcPerpendicularCcw(end - start);
        }

        /// <summary>
        /// Calculates a direction vector from degrees and magnitude
        /// </summary>
        /// <param name="degrees">The angle for the vector</param>
        /// <param name="magnitude">The magnitude for the vector</param>
        /// <param name="offDir">
        /// An optional parameter to help with the way Duck Game handles flipping objects.
        /// It will flip the vector across the Y axis if set to -1.
        /// </param>
        /// <returns>A vector with the direction and length as specified by the angle and magnitude given</returns>
        public static Vec2 CalcVec(float degrees, float magnitude, int offDir = 0)
        {
            if (offDir == -1)
            {
                degrees = 180 - degrees;
            }
            degrees *= DegToRad;
            return new Vec2((float)(magnitude * Math.Cos(degrees)), (float)(magnitude * Math.Sin(degrees)));
        }

        /// <summary>
        /// Calculates the closest points to the given position
        /// </summary>
        /// <param name="points">The array of points to search through</param>
        /// <param name="origin">The position to check distance to</param>
        /// <param name="number">The number of closest points to return</param>
        /// <returns>
        /// The closest points to the given position, in order of increasing distance,
        /// or null if the input array is smaller than the number of requested points.
        /// </returns>
        public static Vec2[] CalcClosestPoints(Vec2[] points, Vec2 origin, int number)
        {
            if (number > points.Length)
            {
                MLogger.Log("Less points than expected in: MathUtils.CalcClosestPoints");
                return null;
            }

            points = points.OrderBy(x => -DistSqr(x, origin)).ToArray();
            points = points.Reverse().ToArray();

            return new ArraySegment<Vec2>(points, 0, number).Array;
        }

        /// <summary>
        /// Calculates the single closest point to a position.
        /// </summary>
        /// <param name="points">The array of points to search through</param>
        /// <param name="origin">The position to check the distance to</param>
        /// <param name="index">(Out Parameter) The index in the original array at which the closest point is</param>
        /// <returns>The closest point to the given position</returns>
        public static Vec2 CalcClosestPoint(Vec2[] points, Vec2 origin, out int index)
        {
            float closest = float.MaxValue;
            index = -1;

            for (int i = 1; i < points.Length; i++)
            {
                float dist = (points[i] - origin).length;
                if (!(dist < closest)) continue;
                closest = dist;
                index = i;
            }

            return index != -1 ? points[index] : origin;
        }

        /// <summary>
        /// Calculates the angle, in radians, of a direction vector.
        /// </summary>
        /// <param name="vec">The direction vector.</param>
        /// <returns>The angle, in radians, of the direction vector.</returns>
        public static float CalcRadians(Vec2 vec)
        {
            return (float)(Math.Tan(vec.x / vec.y) * -1);
        }

        /// <summary>
        /// Calculates the angle, in degrees, of the line connecting two points
        /// </summary>
        /// <param name="start">The start position of the line</param>
        /// <param name="end">The end position of the line</param>
        /// <returns>The angle, in degrees, of the line between the two points</returns>
        public static float CalcDegreesBetween(Vec2 start, Vec2 end)
        {
            return CalcRadians(start - end) * RadToDeg;
        }

        /// <summary>
        /// Calculates the cube root of a value
        /// </summary>
        /// <param name="d">The value</param>
        /// <returns>The cube root of the value</returns>
        public static double CubeRoot(double d)
        {
            return Math.Ceiling(Math.Pow(d, 1d / 3d));
        }

        /// <summary>
        /// Calculates the absolute value of a value.
        /// </summary>
        /// <param name="d">The value</param>
        /// <returns>The absolute value of the value</returns>
        public static double Abs(double d)
        {
            if (d < 0)
            {
                return d * -1;
            }

            return d;
        }

        #region Usefuls

        /// <summary>
        /// A clean lerp function that linearly interpolates - lerps - between two values.
        /// </summary>
        /// <param name="from">The value to lerp from</param>
        /// <param name="to">The value to lerp towards</param>
        /// <param name="amount">
        /// The amount to lerp between the values.
        /// Note that it is unclamped and values outside of 0-1 will
        /// return values outside of the to-from range.
        /// </param>
        /// <returns>A value that is amount % between from and to</returns>
        public static float Lerp1(float from, float to, float amount)
        {
            return from * (1 - amount) + to * amount;
        }

        /// <summary>
        /// A clean lerp function that linearly interpolates - lerps - between two values.
        /// </summary>
        /// <param name="from">The value to lerp from</param>
        /// <param name="to">The value to lerp towards</param>
        /// <param name="amount">
        /// The amount to lerp between the values.
        /// Note that it is unclamped and values outside of 0-1 will
        /// return values outside of the to-from range.
        /// </param>
        /// <returns>A value that is amount % between from and to</returns>
        public static Vec2 Lerp2(Vec2 from, Vec2 to, float amount)
        {
            return from * (1 - amount) + to * amount;
        }

        /// <summary>
        /// A clean lerp function that linearly interpolates - lerps - between two values.
        /// </summary>
        /// <param name="from">The value to lerp from</param>
        /// <param name="to">The value to lerp towards</param>
        /// <param name="amount">
        /// The amount to lerp between the values.
        /// Note that it is unclamped and values outside of 0-1 will
        /// return values outside of the to-from range.
        /// </param>
        /// <returns>A value that is amount % between from and to</returns>
        public static Vec3 Lerp3(Vec3 from, Vec3 to, float amount)
        {
            return from * (1 - amount) + to * amount;
        }

        /// <summary>
        /// A clean lerp function that linearly interpolates - lerps - between two values.
        /// </summary>
        /// <param name="from">The value to lerp from</param>
        /// <param name="to">The value to lerp towards</param>
        /// <param name="amount">
        /// The amount to lerp between the values.
        /// Note that it is unclamped and values outside of 0-1 will
        /// return values outside of the to-from range.
        /// </param>
        /// <returns>A value that is amount % between from and to</returns>
        public static Vec4 Lerp4(Vec4 from, Vec4 to, float amount)
        {
            return from * (1 - amount) + to * amount;
        }

        /// <summary>
        /// A clean modulus function that uses the OpenGL formula.
        /// </summary>
        /// <param name="value">The value to modulo</param>
        /// <param name="modulo">The modulo value</param>
        /// <returns>The remainder when dividing the value by the modulo</returns>
        public static float Mod1(float value, float modulo)
        {
            return (float) (value - modulo * Math.Floor(value / modulo));
        }

        /// <summary>
        /// A clean modulus function that uses the OpenGL formula.
        /// </summary>
        /// <param name="value">The value to modulo</param>
        /// <param name="modulo">The modulo value</param>
        /// <returns>The remainder when dividing the value by the modulo</returns>
        /// <remarks>This function performs the modulo separately on each component of the inputs</remarks>
        public static Vec2 Mod2(Vec2 value, Vec2 modulo)
        {
            return new Vec2(Mod1(value.x, modulo.x), Mod1(value.y, modulo.y));
        }

        /// <summary>
        /// A clean modulus function that uses the OpenGL formula.
        /// </summary>
        /// <param name="value">The value to modulo</param>
        /// <param name="modulo">The modulo value</param>
        /// <returns>The remainder when dividing the value by the modulo</returns>
        /// <remarks>This function performs the modulo separately on each component of the inputs</remarks>
        public static Vec3 Mod3(Vec3 value, Vec3 modulo)
        {
            return new Vec3(Mod1(value.x, modulo.x), Mod1(value.y, modulo.y), Mod1(value.z, modulo.z));
        }

        /// <summary>
        /// A clean modulus function that uses the OpenGL formula.
        /// </summary>
        /// <param name="value">The value to modulo</param>
        /// <param name="modulo">The modulo value</param>
        /// <returns>The remainder when dividing the value by the modulo</returns>
        /// <remarks>This function performs the modulo separately on each component of the inputs</remarks>
        public static Vec4 Mod4(Vec4 value, Vec4 modulo)
        {
            return new Vec4(Mod1(value.x, modulo.x), Mod1(value.y, modulo.y), Mod1(value.z, modulo.z), Mod1(value.w, modulo.w));
        }

        /// <summary>
        /// Compares two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whether the absolute value of x - y is less than the epsilon</returns>
        public static bool Compare(float x, float y)
        {
            return Math.Abs(x - y) < Epsilon;
        }

        /// <summary>
        /// Compares two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whether the absolute value of x - y is less than the epsilon</returns>
        /// <remarks>This function performs the comparision separately on each component of the input and combines each result with the logical AND</remarks>
        public static bool Compare2(Vec2 x, Vec2 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y);
        }

        /// <summary>
        /// Compares two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whether the absolute value of x - y is less than the epsilon</returns>
        /// <remarks>This function performs the comparision separately on each component of the input and combines each result with the logical AND</remarks>
        public static bool Compare3(Vec3 x, Vec3 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y) && Compare(x.z, y.z);
        }

        /// <summary>
        /// Compares two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whether the absolute value of x - y is less than the epsilon</returns>
        /// <remarks>This function performs the comparision separately on each component of the input and combines each result with the logical AND</remarks>
        public static bool Compare4(Vec4 x, Vec4 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y) && Compare(x.z, y.z) && Compare(x.w, y.w);
        }

        /// <summary>
        /// Returns the greater of two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever value is greater</returns>
        public static float Max(float x, float y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the greater of two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever value is greater</returns>
        /// <remarks>This function performs the Max on each component of the input and returns a new vector that has the Max value for each component</remarks>
        public static Vec2 Max2(Vec2 x, Vec2 y)
        {
            return new Vec2(Max(x.x, y.x), Max(x.y, y.y));
        }

        /// <summary>
        /// Returns the greater of two vectors, by length.
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever vector is greater, by length</returns>
        public static Vec2 Max2Length(Vec2 x, Vec2 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        /// <summary>
        /// Returns the greater of two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever value is greater</returns>
        /// <remarks>This function performs the Max on each component of the input and returns a new vector that has the Max value for each component</remarks>
        public static Vec3 Max3(Vec3 x, Vec3 y)
        {
            return new Vec3(Max(x.x, y.x), Max(x.y, y.y), Max(x.z, y.z));
        }

        /// <summary>
        /// Returns the greater of two vectors, by length.
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever vector is greater, by length</returns>
        public static Vec3 Max3Length(Vec3 x, Vec3 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        /// <summary>
        /// Returns the greater of two values
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever value is greater</returns>
        /// <remarks>This function performs the Max on each component of the input and returns a new vector that has the Max value for each component</remarks>
        public static Vec4 Max4(Vec4 x, Vec4 y)
        {
            return new Vec4(Max(x.x, y.x), Max(x.y, y.y), Max(x.z, y.z), Max(x.w, y.w));
        }

        /// <summary>
        /// Returns the greater of two vectors, by length.
        /// </summary>
        /// <param name="x">The first value</param>
        /// <param name="y">The second value</param>
        /// <returns>Whichever vector is greater, by length</returns>
        public static Vec4 Max4Length(Vec4 x, Vec4 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        /// <summary>
        /// Calculates the nearest integer below the given value
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The nearest integer below the given value</returns>
        public static float Floor(float x)
        {
            return (float) Math.Floor(x);
        }

        /// <summary>
        /// Calculates the nearest integer below the given value
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The nearest integer below the given value</returns>
        /// <remarks>This function performs the Floor on each component of the input and returns a new vector that has the Floor value for each component</remarks>
        public static Vec2 Floor2(Vec2 x)
        {
            return new Vec2(Floor(x.x), Floor(x.y));
        }

        /// <summary>
        /// Calculates the nearest integer below the given value
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The nearest integer below the given value</returns>
        /// <remarks>This function performs the Floor on each component of the input and returns a new vector that has the Floor value for each component</remarks>
        public static Vec3 Floor3(Vec3 x)
        {
            return new Vec3(Floor(x.x), Floor(x.y), Floor(x.z));
        }

        /// <summary>
        /// Calculates the nearest integer below the given value
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The nearest integer below the given value</returns>
        /// <remarks>This function performs the Floor on each component of the input and returns a new vector that has the Floor value for each component</remarks>
        public static Vec4 Floor4(Vec4 x)
        {
            return new Vec4(Floor(x.x), Floor(x.y), Floor(x.z), Floor(x.w));
        }
        
        /// <summary>
        /// Converts a RGB value to a HSV value.
        /// </summary>
        /// <param name="c">The colour to convert, passed as a vector</param>
        /// <returns>The HSV equivalent of the RGB value</returns>
        public static Vec3 RgbToHsv(Vec3 c)
        {
            float r = (c.x / 255f);
            float g = (c.y / 255f);
            float b = (c.z / 255f);

            float min = Math.Min(r, Math.Min(g, b));
            float max = Math.Max(r, Math.Max(g, b));
            float delta = max - min;

            float h = 0;
            float s = 0;
            float v = max;
            
            if (v == 0)
            {
                h = 0;
                s = 0;
            }
            else
            {
                s = delta / max;

                float deltaR = (((max - r) / 6f) + (delta / 2f)) / delta;
                float deltaB = (((max - b) / 6f) + (delta / 2f)) / delta;
                float deltaG = (((max - g) / 6f) + (delta / 2f)) / delta;

                if (Math.Abs(r - max) < Epsilon) h = deltaB - deltaG;
                else if (Math.Abs(g - max) < Epsilon) h = (1f / 3f) + deltaR - deltaB;
                else if (Math.Abs(b - max) < Epsilon) h = (2f / 3f) + deltaG - deltaR;

                if (h < 0f) h += 1f;
                if (h > 1f) h -= 1f;
            }

            return new Vec3(h, s, v);
        }

        /// <summary>
        /// Converts a HSV value to a RGB value.
        /// </summary>
        /// <param name="c">The colour to convert, passed as a vector</param>
        /// <returns>The RGB equivalent of the HSV value</returns>
        public static Vec3 HsVtoRgb(Vec3 c)
        {
            float r;
            float g;
            float b;

            float h = c.x;
            float s = c.y;
            float v = c.z;
            
            if (s == 0)
            {
                r = v * 255; g = v * 255; b = v * 255;
            }
            else
            {
                float vh = h * 6;
                if (Math.Abs(vh - 6) < Epsilon) vh = 0;
                float vi = (float) Math.Floor(vh);
                float v1 = v * (1 - s);
                float v2 = v * (1 - s * (vh - vi));
                float v3 = v * (1 - s * (1 - (vh - vi)));

                switch (vi)
                {
                    case 0:
                        r = v; g = v3; b = v1;
                        break;
                    case 1:
                        r = v2; g = v; b = v1;
                        break;
                    case 2:
                        r = v1; g = v; b = v3;
                        break;
                    case 3:
                        r = v1; g = v2; b = v;
                        break;
                    case 4:
                        r = v3; g = v1; b = v;
                        break;
                    default:
                        r = v; g = v1; b = v2;
                        break;
                }
            }

            return new Vec3(r * 255, g * 255, b * 255);
        }

        #endregion



    }
}
