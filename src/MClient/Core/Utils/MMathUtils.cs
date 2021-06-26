using System;
using System.Linq;
using DuckGame;
using MClientCore.MClient.Core;

namespace MClient.Utils
{
    internal static class MMathUtils
    {

        public const float DegToRad = (float)0.0174533;

        public const float RadToDeg = (float)57.2958;

        public const float Epsilon = (float)0.00001;

        public static Vec2 CalcHitPoint(Vec2 start, Vec2 end, MaterialThing mThing)
        {
            Vec2[] intersects = new Vec2[] {
            CalcIntersection(start, end, mThing.topLeft, mThing.bottomLeft),
            CalcIntersection(start, end, mThing.topLeft, mThing.topRight),
            CalcIntersection(start, end, mThing.topRight, mThing.bottomRight),
            CalcIntersection(start, end, mThing.bottomRight, mThing.bottomLeft)
            };

            Vec2 nearest = end;
            float distance = (end - start).length;

            foreach (Vec2 vec in intersects)
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

        public static Vec2 CalcIntersection(Vec2 s1, Vec2 e1, Vec2 s2, Vec2 e2, bool zeroIfNotOnLine = true, float accuracy = 0f)
        {
            float a1 = e1.y - s1.y;
            float b1 = s1.x - e1.x;
            float c1 = a1 * s1.x + b1 * s1.y;

            float a2 = e2.y - s2.y;
            float b2 = s2.x - e2.x;
            float c2 = a2 * s2.x + b2 * s2.y;

            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                return Vec2.Zero;
            }

            Vec2 intersect = new Vec2((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);

            if (!zeroIfNotOnLine)
            {
                return intersect;
            }
            else if (PointOnLine(intersect, s1, e1, accuracy) && PointOnLine(intersect, s2, e2, accuracy))
            {
                return intersect;
            }

            return Vec2.Zero;
        }

        public static bool PointOnLine(Vec2 point, Vec2 s, Vec2 e, float accuracy = 0f)
        {
            float dist1 = Dist(s, point);
            float dist2 = Dist(e, point);
            float dist3 = Dist(s, e);
            float dist4 = dist1 + dist2;

            return dist4 - dist3 <= accuracy;
        }

        public static float Dist(Vec2 s, Vec2 e)
        {
            return (s - e).Length();
        }

        public static float DistSqr(Vec2 s, Vec2 e)
        {
            return (s - e).LengthSquared();
        }

        public static Vec2 CalcPerpCw(Vec2 vec)
        {
            return new Vec2(vec.y, -vec.x);
        }

        public static Vec2 CalcPerpCcw(Vec2 vec)
        {
            return new Vec2(-vec.y, vec.x);
        }

        public static Vec2 CalcPerpCw(Vec2 start, Vec2 end)
        {
            return CalcPerpCw(end - start);
        }
        
        public static Vec2 CalcPerpCcw(Vec2 start, Vec2 end)
        {
            return CalcPerpCcw(end - start);
        }

        public static Vec2 CalcVec(float degrees, float magnitude, int offDir)
        {
            if (offDir == -1)
            {
                degrees = 180 - degrees;
            }
            degrees *= DegToRad;
            return new Vec2((float)(magnitude * Math.Cos(degrees)), (float)(magnitude * Math.Sin(degrees)));
        }


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

        public static Vec2 CalcClosestPoint(Vec2[] points, Vec2 origin, out int index)
        {
            float closest = float.MaxValue;
            index = -1;

            for (int i = 1; i < points.Length; i++)
            {
                float dist = (points[i] - origin).length;
                if (dist < closest)
                {
                    closest = dist;
                    index = i;
                }
            }

            if (index != -1)
            {
                return points[index];
            }

            return origin;

        }

        public static float CalcRadians(Vec2 vec)
        {
            return (float)(Math.Tan(vec.x / vec.y) * -1);
        }

        public static float CalcDegreesBetween(Vec2 start, Vec2 end)
        {
            return CalcRadians(start - end) * RadToDeg;
        }


        public static double Cbrt(double d)
        {
            return Math.Ceiling(Math.Pow(d, 1 / 3));
        }

        public static double Abs(double d)
        {
            if (d < 0)
            {
                return d * -1;
            }

            return d;
        }

        #region Usefuls

        public static float Lerp1(float current, float to, float amount)
        {
            return current * (1 - amount) + to * amount;
        }

        public static Vec2 Lerp2(Vec2 current, Vec2 to, float amount)
        {
            return current * (1 - amount) + to * amount;
        }

        public static Vec3 Lerp3(Vec3 current, Vec3 to, float amount)
        {
            return current * (1 - amount) + to * amount;
        }

        public static Vec4 Lerp4(Vec4 current, Vec4 to, float amount)
        {
            return current * (1 - amount) + to * amount;
        }

        public static float Mod1(float x, float y)
        {
            return (float) (x - y * Math.Floor(x / y));
        }

        public static Vec2 Mod2(Vec2 x, Vec2 y)
        {
            return new Vec2(Mod1(x.x, y.x), Mod1(x.y, y.y));
        }

        public static Vec3 Mod3(Vec3 x, Vec3 y)
        {
            return new Vec3(Mod1(x.x, y.x), Mod1(x.y, y.y), Mod1(x.z, y.z));
        }

        public static Vec4 Mod4(Vec4 x, Vec4 y)
        {
            return new Vec4(Mod1(x.x, y.x), Mod1(x.y, y.y), Mod1(x.z, y.z), Mod1(x.w, y.w));
        }

        public static bool Compare(float x, float y)
        {
            return Math.Abs(x - y) < Epsilon;
        }

        public static bool Compare2(Vec2 x, Vec2 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y);
        }

        public static bool Compare3(Vec3 x, Vec3 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y) && Compare(x.z, y.z);
        }

        public static bool Compare4(Vec4 x, Vec4 y)
        {
            return Compare(x.x, y.x) && Compare(x.y, y.y) && Compare(x.z, y.z) && Compare(x.w, y.w);
        }

        public static float Max(float x, float y)
        {
            return x > y ? x : y;
        }
        public static Vec2 Max2(Vec2 x, Vec2 y)
        {
            return new Vec2(Max(x.x, y.x), Max(x.y, y.y));
        }

        public static Vec2 Max2Length(Vec2 x, Vec2 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        public static Vec3 Max3(Vec3 x, Vec3 y)
        {
            return new Vec3(Max(x.x, y.x), Max(x.y, y.y), Max(x.z, y.z));
        }

        public static Vec3 Max3Length(Vec3 x, Vec3 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        public static Vec4 Max4(Vec4 x, Vec4 y)
        {
            return new Vec4(Max(x.x, y.x), Max(x.y, y.y), Max(x.z, y.z), Max(x.w, y.w));
        }

        public static Vec4 Max4Length(Vec4 x, Vec4 y)
        {
            return x.LengthSquared() > y.LengthSquared() ? x : y;
        }

        public static float Floor(float x)
        {
            return (float) Math.Floor(x);
        }

        public static Vec2 Floor2(Vec2 x)
        {
            return new Vec2(Floor(x.x), Floor(x.y));
        }

        public static Vec3 Floor3(Vec3 x)
        {
            return new Vec3(Floor(x.x), Floor(x.y), Floor(x.z));
        }

        public static Vec4 Floor4(Vec4 x)
        {
            return new Vec4(Floor(x.x), Floor(x.y), Floor(x.z), Floor(x.w));
        }
        
        public static Vec3 RGBToHSV(Vec3 c)
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

                if (r == max) h = deltaB - deltaG;
                else if (g == max) h = (1f / 3f) + deltaR - deltaB;
                else if (b == max) h = (2f / 3f) + deltaG - deltaR;

                if (h < 0f) h += 1f;
                if (h > 1f) h -= 1f;
            }

            return new Vec3(h, s, v);
        }

        public static Vec3 HSVtoRGB(Vec3 c)
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
                if (vh == 6) vh = 0;
                float vi = (float) Math.Floor(vh);
                float v1 = v * (1 - s);
                float v2 = v * (1 - s * (vh - vi));
                float v3 = v * (1 - s * (1 - (vh - vi)));

                if (vi == 0)
                {
                    r = v; g = v3; b = v1;
                }
                else if (vi == 1)
                {
                    r = v2; g = v; b = v1;
                }
                else if (vi == 2)
                {
                    r = v1; g = v; b = v3;
                }
                else if (vi == 3)
                {
                    r = v1; g = v2; b = v;
                }
                else if (vi == 4)
                {
                    r = v3; g = v1; b = v;
                }
                else
                {
                    r = v; g = v1; b = v2;
                }
            }

            return new Vec3(r * 255, g * 255, b * 255);
        }

        #endregion



    }
}
