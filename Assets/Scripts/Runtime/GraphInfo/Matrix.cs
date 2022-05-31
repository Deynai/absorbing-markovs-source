using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

#if !UNITY_WEBGL

namespace Deynai.Markov
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MatrixInfo
    {
        public int Columns { get; }
        public int Rows { get; }
        public double[] Value { get; }

        public MatrixInfo(int col, int row, double[] val)
        {
            Columns = col;
            Rows = row;
            Value = val;
        }
    }

    public partial class Matrix
    {
        public int Columns => info.Columns;
        public int Rows => info.Rows;
        public double[] Value => info.Value;
        public int Det => Internal_Determinant(m_InfoPointer);

        private IntPtr m_ValuePtr;
        private IntPtr m_InfoPointer;

        private MatrixInfo info;

        public Matrix(double[] matrix, int columns, int rows)
        {
            AllocateUnmanagedMemory(columns, rows);
            Internal_WriteValue(m_InfoPointer, matrix);
        }

        public Matrix(int columns, int rows)
        {
            AllocateUnmanagedMemory(columns, rows);
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Rows; i++)
            {
                if (i > 0) result += "\n";
                for (int j = 0; j < Columns; j++)
                {
                    result += (Value[j + i * Columns]).ToString("0.###") + "\t";
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m2.Columns, m1.Rows);
            Internal_MultiplyMatrix(m1.m_InfoPointer, m2.m_InfoPointer, result.m_InfoPointer);
            return result;
        }

        public static Matrix Multiply(Matrix m1, float scalar)
        {
            Matrix result = new Matrix(m1.Columns, m1.Rows);
            Internal_MultiplyMatrixScalar(m1.m_InfoPointer, scalar, result.m_InfoPointer);
            return result;
        }

        public static Matrix Add(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Columns, m1.Rows);
            Internal_AddMatrix(m1.m_InfoPointer, m2.m_InfoPointer, result.m_InfoPointer);
            return result;
        }

        public static Matrix Subtract(Matrix m1, Matrix m2)
        {
            Matrix result = new Matrix(m1.Columns, m1.Rows);
            Internal_SubtractMatrix(m1.m_InfoPointer, m2.m_InfoPointer, result.m_InfoPointer);
            return result;
        }

        public static Matrix Pow(Matrix m1, int exponent)
        {
            Matrix result = new Matrix(m1.Columns, m1.Rows);
            Internal_PowMatrix(m1.m_InfoPointer, exponent, result.m_InfoPointer);
            return result;
        }

        public static Matrix CreateCopy(Matrix m1)
        {
            Matrix result = new Matrix(m1.Columns, m1.Rows);
            Internal_CopyMatrix(m1.m_InfoPointer, result.m_InfoPointer);
            return result;
        }

        public static Matrix Invert(Matrix m)
        {
            Matrix result = new Matrix(m.Columns, m.Rows);
            Internal_InverseMatrix(m.m_InfoPointer, result.m_InfoPointer);
            return result;
        }
        
        public static Matrix Identity(int size)
        {
            Matrix result = new Matrix(size, size);
            Internal_SetIdentity(result.m_InfoPointer);
            return result;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return Multiply(m1, m2);
        }
        public static Matrix operator *(Matrix m1, float scalar)
        {
            return Multiply(m1, scalar);
        }
        public static Matrix operator *(float scalar, Matrix m1)
        {
            return Multiply(m1, scalar);
        }
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            return Add(m1, m2);
        }
        public static Matrix operator -(Matrix m1, Matrix m2){
            return Subtract(m1, m2);
        }

        private void AllocateUnmanagedMemory(int columns, int rows)
        {
            // alloc unmanaged memory for the matrix
            m_ValuePtr = Marshal.AllocHGlobal(columns * rows * Marshal.SizeOf(typeof(double)));
            double[] val = new double[columns * rows];
            Marshal.StructureToPtr(val, m_ValuePtr, false);

            // alloc unmanaged memory for the object info, size, etc.
            m_InfoPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MatrixInfo)));
            info = new MatrixInfo(columns, rows, val);
            Marshal.StructureToPtr(info, m_InfoPointer, false);
        }

        ~Matrix()
        {
            Marshal.FreeHGlobal(m_ValuePtr);
            Marshal.FreeHGlobal(m_InfoPointer);
        }
    }

    public partial class Matrix
        {
            // Define Dll imports here
            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_WriteValue")]
            private static extern void Internal_WriteValue(IntPtr m, double[] value);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_MultiplyMatrix")]
            private static extern void Internal_MultiplyMatrix(IntPtr m1, IntPtr m2, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_MultiplyMatrixScalar")]
            private static extern void Internal_MultiplyMatrixScalar(IntPtr m1, float scalar, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_AddMatrix")]
            private static extern void Internal_AddMatrix(IntPtr m1, IntPtr m2, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_SubtractMatrix")]
            private static extern void Internal_SubtractMatrix(IntPtr m1, IntPtr m2, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_Determinant")]
            private static extern int Internal_Determinant(IntPtr m);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_PowMatrix")]
            private static extern int Internal_PowMatrix(IntPtr m1, int power, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_CopyMatrix")]
            private static extern int Internal_CopyMatrix(IntPtr m, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_InverseMatrix")]
            private static extern int Internal_InverseMatrix(IntPtr m, IntPtr result);

            [DllImport("ABSORBINGMARKOVDLL", EntryPoint = "Internal_SetIdentity")]
            private static extern int Internal_SetIdentity(IntPtr m);
        }
}
#endif