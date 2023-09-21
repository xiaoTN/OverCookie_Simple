using UnityEngine;

namespace TN.Common
{
    public static class MathUtils
    {
        public static int Pow(int i, int p)
        {
            if (p == 0) return 1;
            int res = 1;
            for (int j = 0; j < p; j++)
            {
                res *= i;
            }

            return res;
        }
        public static float GetNormalDistributionRandom()
        {
            float mean = 0; // 正态分布的平均值
            float stdDev = 1; // 正态分布的标准差
            
            // 生成两个独立的、均匀分布的随机数
            float u1 = Random.Range(0,1f);
            float u2 = Random.Range(0,1f);

            // 使用Box-Muller转换算法生成正态分布的随机数
            float z1 = Mathf.Sqrt(-2 * Mathf.Log(u1)) * Mathf.Cos(2 * Mathf.PI * u2);
            float x1 = mean + stdDev * z1+2;
            return x1/4f;
        }

        public static float Remap(float value, float minValue, float maxValue, float targetMinValue = 0, float targetMaxValue = 1)
        {
            float v = Mathf.Clamp(value, minValue, maxValue);
            float scale = (targetMaxValue - targetMinValue) / (maxValue - minValue);
            return targetMinValue + (v - minValue) * scale;
        }
    }
}