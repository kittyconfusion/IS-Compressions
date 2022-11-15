﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Compressions.code.core;

internal static class Display
{
    public static readonly int tileSize = 32;
    public struct DisplaySettings
    {
        public bool mapStartEnabled;
        public int startScreenWidth, startScreenHeight;
        public float initialXOffset, initialYOffset, initialTilesShown;
        public double minTilesShown, maxTilesShown;
        public int baseR, baseG, baseB, outR, outG, outB;
        public int displayMode, pixelWidth, pixelHeight;

        public DisplaySettings(bool v1, int x, int y, float v2, float v3, float v4, double v5, double v6, int v7, int v8,
            int v9, int v10, int v11, int v12, int v13, int v14, int v15) : this()
        {
            mapStartEnabled = v1; startScreenWidth = x; startScreenHeight = y; initialXOffset = v2; initialYOffset = v3; initialTilesShown = v4;
            minTilesShown = v5; maxTilesShown = v6;
            baseR = v7; baseG = v8; baseB = v9; outR = v10; outG = v11; outB = v12; displayMode = v13; pixelWidth = v14; pixelHeight = v15;
        }
    }

    public static DisplaySettings settings;
}