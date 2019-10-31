using System;

namespace DockerGui.Service
{
    public static class NumberExtend
    {
        public static decimal ToFixed(this decimal number, int digit = 2)
        {
            return Math.Round(number, digit, MidpointRounding.AwayFromZero);
        }
    }
}