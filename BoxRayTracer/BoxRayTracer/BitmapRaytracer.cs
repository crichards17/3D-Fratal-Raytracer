﻿using Scene;
using System.Drawing;
using Color = System.Drawing.Color;

namespace BoxRayTracer
{
    internal class BitmapRaytracer
    {
        private readonly IDistanceEstimatable dE;
        private readonly Camera camera;
        private readonly int dWidth;
        private readonly int dHeight;
        private readonly double maxDist;
        private readonly Color objColor;
        private readonly Color backColor;

        // Lights
        private readonly AmbientLight globalAmbient;
        private readonly GlobalDiffuseLight[] globalDiffuseLights;
        private readonly IPointLight[] pointLights;

        public BitmapRaytracer(IDistanceEstimatable dE, double maxDist, int dWidth, int dHeight, double fov, Vector camPos, Vector lookAt, System.Windows.Media.Color objColor, System.Windows.Media.Color backColor, AmbientLight globalAmbient, GlobalDiffuseLight[] globalDiffuseLights, IPointLight[] pointLights)
        {
            this.dE = dE;
            this.dWidth = dWidth;
            this.dHeight = dHeight;
            this.maxDist = maxDist;
            this.camera = new Camera(camPos, lookAt, fov, 1, (double)dHeight / dWidth, dWidth, dHeight);
            // Color enumeration comes from Windows.Media.Color, but Bitmap requires Drawing.Color,
            //  so we convert here:
            this.objColor = Color.FromArgb(objColor.A, objColor.R, objColor.G, objColor.B);
            this.backColor = Color.FromArgb(backColor.A, backColor.R, backColor.G, backColor.B);
            this.globalAmbient = globalAmbient;
            this.globalDiffuseLights = globalDiffuseLights;
            this.pointLights = pointLights;
        }

        public Bitmap Render()
        {
            Bitmap image = new(dWidth, dHeight);
            for (int i = 0; i < dWidth; i++)
            {
                for (int j = 0; j < dHeight; j++)
                {
                    image.SetPixel(i, j, RayMarch((uint)i, (uint)j));
                }
            }
            return image;
        }

        private Color RayMarch(uint x, uint y)
        {
            camera.RayForPixel(x, y, out Vector pos, out Vector rayDir);
            double currentDist = dE.DE(pos);
            while (currentDist <= maxDist)
            {
                if (Utilities.IsEqualApprox(currentDist, 0))
                {
                    // Do the B.P. thing, then return the color
                    return objColor;
                }
                pos += rayDir * currentDist;
                currentDist = dE.DE(pos);
            }
            return backColor;
        }

        public void GetSceneParams(out Vector camPos, out Vector camFrus, out Vector camRoll)
        {
            camPos = camera.camPos;
            camFrus = camera.vFrus;
            camRoll = camera.vRoll;
        }
    }
}
