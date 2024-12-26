using UnityEngine;

namespace IAEngine
{
    public static class ColorEx
    {
        public static Color New(int pR, int pG, int pB, int pA = 255)
        {
            if (pA == 255)
            {
                return new Color(pR / 255.0f, pG / 255.0f, pB / 255.0f);
            }
            else
            {
                return new Color(pR / 255.0f, pG / 255.0f, pB / 255.0f, pA / 255.0f);
            }
        }
    }
}