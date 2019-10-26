using System;

namespace DockerGui
{
    public static class NumberExtend
    {
        public static decimal ToFixed(this decimal number, int digit = 2)
        {
            return Math.Round(number, digit, MidpointRounding.AwayFromZero);
        }
    }
}