using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphics_Task4_5
{
    public class Matrix
    {
        public double[,] matr { get; set; }

        public Matrix(double[,] m)
        {
            matr = m;
        }

        public Matrix(int rows, int col)
        {
            matr = new double[rows, col];
        }

        private static double[,] ZeroMatrix()
        {
            double[,] result = new double[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    result[i, j] = 0;
            return result;
        }

        public static Matrix OneMatrix()
        {
            double[,] result = new double[4, 4];
            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = 1;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix XRotation(double angle)
        {
            double[,] result = ZeroMatrix();
            double cos = Math.Cos(angle * Math.PI / 180);
            double sin = Math.Sin(angle * Math.PI / 180);
            result[0, 0] = 1;
            result[3, 3] = 1;
            result[1, 1] = cos;
            result[1, 2] = sin;
            result[2, 1] = -sin;
            result[2, 2] = cos;
            return new Matrix(result);
        }

        public static Matrix YRotation(double angle)
        {
            double[,] result = ZeroMatrix();
            double cos = Math.Cos(angle * Math.PI / 180);
            double sin = Math.Sin(angle * Math.PI / 180);
            result[1, 1] = 1;
            result[3, 3] = 1;
            result[0, 0] = cos;
            result[2, 2] = cos;
            result[0, 2] = -sin;
            result[2, 0] = sin;
            return new Matrix(result);
        }

        public static Matrix ZRotation(double angle)
        {
            double[,] result = ZeroMatrix();
            double cos = Math.Cos(angle * Math.PI / 180);
            double sin = Math.Sin(angle * Math.PI / 180);
            result[0, 0] = cos;
            result[0, 1] = sin;
            result[1, 0] = -sin;
            result[1, 1] = cos;
            result[2, 2] = 1;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix ReflectionXY()
        {
            double[,] result = ZeroMatrix();
            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = -1;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix ReflectionYZ()
        {
            double[,] result = ZeroMatrix();
            result[0, 0] = -1;
            result[1, 1] = 1;
            result[2, 2] = 1;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix ReflectionZX()
        {
            double[,] result = ZeroMatrix();
            result[0, 0] = 1;
            result[1, 1] = -1;
            result[2, 2] = 1;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix Compression(double a, double b, double c)
        {
            double[,] result = ZeroMatrix();
            result[0, 0] = a;
            result[1, 1] = b;
            result[2, 2] = c;
            result[3, 3] = 1;
            return new Matrix(result);
        }

        public static Matrix Shifting(double a, double b, double c)
        {
            double[,] result = ZeroMatrix();
            result[0, 0] = 1;
            result[1, 1] = 1;
            result[2, 2] = 1;
            result[3, 3] = 1;
            result[3, 0] = a;
            result[3, 1] = b;
            result[3, 2] = c;
            return new Matrix(result);
        }

        public static Matrix MultMatrix(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(a.matr.GetLength(0), b.matr.GetLength(1));
            for (int i = 0; i < result.matr.GetLength(0); i++)
            {
                for (int j = 0; j < result.matr.GetLength(1); j++)
                {
                    double elem = 0;
                    for (int k = 0; k < b.matr.GetLength(1); k++)
                    {
                        elem += a.matr[i, k] * b.matr[k, j];
                    }
                    result.matr[i,j] = elem;
                }
            }
            return result;
        }


        public static Matrix GetVector(Point p)
        {
            double[,] result = new double[1, 4] { { p.X, p.Y, p.Z, 1 } };
            return new Matrix(result);
        }

        public static Matrix RandomAxisTurn(Point p1, Point p2, double angle)
        {
            /* Расстояния между точками */
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;
            /* Расстояние от начала координат то точки Р(dx,dy,dz) */
            double r = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            /* Нормирование */
            double xnorm = dx / r;
            double ynorm = dy / r;
            double znorm = dz / r;
            /* Расстояние относительно y и z*/
            double d = Math.Sqrt(ynorm * ynorm + znorm * znorm);
            /* Вращаем вокруг Ox*/
            Matrix posAngleOx = new Matrix(new double[4, 4] { { 1, 0, 0, 0 }, { 0, znorm / d, ynorm / d, 0 }, { 0, -ynorm / d, znorm / d, 0 }, { 0, 0, 0, 1 } });
            Matrix negAngleOx = new Matrix(new double[4, 4] { { 1, 0, 0, 0 }, { 0, znorm / d, -ynorm / d, 0 }, { 0, ynorm / d, znorm / d, 0 }, { 0, 0, 0, 1 } });
            /* Вращаем вокруг Oy*/
            Matrix posAngleOy = new Matrix(new double[4, 4] { { xnorm, 0, d, 0 }, { 0, 1, 0, 0 }, { -d, 0, xnorm, 0 }, { 0, 0, 0, 1 } });
            Matrix negAngleOy = new Matrix(new double[4, 4] { { znorm, 0, -d, 0 }, { 0, 1, 0, 0 }, { d, 0, znorm, 0 }, { 0, 0, 0, 1 } });
            Matrix m11 = new Matrix(new double[4, 4]);
            double sin = Math.Sin(angle * Math.PI / 180);
            double cos = Math.Cos(angle * Math.PI / 180);
            /* Вращаем вокруг Oz*/
            Matrix MZ = new Matrix(new double[4, 4] { { cos, -sin, 0, 0 }, { sin, cos, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } });//Z
            if (dx == 0 || dz == 0)
                m11 = MultMatrix(MultMatrix(posAngleOx, MZ), negAngleOx);
            else
                m11 = MultMatrix(MultMatrix(MultMatrix(posAngleOx, posAngleOy), MultMatrix(MZ, negAngleOy)), negAngleOx);
            return m11;
        }
       
        public static Point PointFromVector(Matrix vec)
        {
            return new Point(vec.matr[0, 0], vec.matr[0, 1], vec.matr[0, 2]);
        }

       
    }
}
