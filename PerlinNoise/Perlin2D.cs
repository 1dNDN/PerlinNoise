using System;

namespace PerlinNoise {
    internal class Perlin2D {
        private readonly byte[] permutationTable;

        public Perlin2D(int seed = 0)
        {
            Random rand = new(seed);
            permutationTable = new byte[1024];
            rand.NextBytes(permutationTable);
        }

        /// <summary>
        ///     Генерация псевдорандомного детерминированного вектора
        /// </summary>
        /// <param name="x">Координата вектора по x</param>
        /// <param name="y">Координата вектора по y</param>
        /// <returns></returns>
        private float[] GetPseudoRandomGradientVector(int x, int y)
        {
            int v = (int) (((x * 1836311903) ^ (y * 2971215073 + 4807526976)) & 1023);
            v = permutationTable[v] & 3;

            return v switch {
                0 => new float[] { 1, 0 },
                1 => new float[] { -1, 0 },
                2 => new float[] { 0, 1 },
                _ => new float[] { 0, -1 }
            };
        }

        /// <summary>
        ///     Квинтическая функция для искривления прямой возле граничных значений
        /// </summary>
        /// <param name="t">Координата точки внутри квадрата векторов</param>
        /// <returns></returns>
        private static float QunticCurve(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        ///     Линейная интерполяция
        /// </summary>
        /// <param name="a">Правая точка</param>
        /// <param name="b">Левая точка</param>
        /// <param name="t">Координаты точки внутри квадрата векторов</param>
        /// <returns></returns>
        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        ///     Скалярное произведение векторов
        /// </summary>
        /// <param name="a">Правый вектор</param>
        /// <param name="b">Левый вектор</param>
        /// <returns></returns>
        private static float Dot(float[] a, float[] b)
        {
            return a[0] * b[0] + a[1] * b[1];
        }

        public float Noise(float fx, float fy)
        {
            // Координаты левого верхнего вектора квадрата
            int left = (int) Math.Floor(fx);
            int top = (int) Math.Floor(fy);
            // Координаты точки относительно квадрата
            float pointInQuadX = fx - left;
            float pointInQuadY = fy - top;

            // Псевдорандомные детерминированные векторы для всех углов квадрата
            float[] topLeftGradient = GetPseudoRandomGradientVector(left, top);
            float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
            float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
            float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

            // Векторы от углов квадрата до точки
            float[] distanceToTopLeft = { pointInQuadX, pointInQuadY };
            float[] distanceToTopRight = { pointInQuadX - 1, pointInQuadY };
            float[] distanceToBottomLeft = { pointInQuadX, pointInQuadY - 1 };
            float[] distanceToBottomRight = { pointInQuadX - 1, pointInQuadY - 1 };

            // Скалярные произведения векторов от каждого угла
            float tl = Dot(distanceToTopLeft, topLeftGradient);
            float tr = Dot(distanceToTopRight, topRightGradient);
            float bl = Dot(distanceToBottomLeft, bottomLeftGradient);
            float br = Dot(distanceToBottomRight, bottomRightGradient);

            // Убираем линейность координат точки внутри квадрата
            pointInQuadX = QunticCurve(pointInQuadX);
            pointInQuadY = QunticCurve(pointInQuadY);

            // Интерполируем векторы
            float tx = Lerp(tl, tr, pointInQuadX);
            float bx = Lerp(bl, br, pointInQuadX);
            float tb = Lerp(tx, bx, pointInQuadY);

            return tb;
        }

        public float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
        {
            float amplitude = 1;
            float max = 0;
            float result = 0;

            while (octaves-- > 0)
            {
                max += amplitude;
                result += Noise(fx, fy) * amplitude;
                amplitude *= persistence;
                fx *= 2;
                fy *= 2;
            }

            return result / max;
        }
    }
}