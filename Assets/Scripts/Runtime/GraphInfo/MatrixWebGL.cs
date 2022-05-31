using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
	/// <summary>
	/// WebGL really doesn't play nice with marshalling for some reason.
	/// The non-WebGL builds work great and marshal as expected - the webGL version throws non-blittable errors with Matrix creation.
	/// After spending a day trying to resolve the problem I'm just going to create this mock internal Matrix
	/// In theory the calls to Internal could be a native library if we find a way to properly marshal InternalMatrix for browsers.
	/// 
	/// (Best guess is that it's something to do with WebGL compiling CPP code directly instead of through a DLL
	/// and because of that it knows more than it should, trying to marshal both directions C++->C# and C#->C++,
	/// instead of our original intent of C#->C++ only with C# allocating memory and allowing C++ to write to what we send)
	/// 
	/// This guess probably makes no sense, tl;dr I have no idea why it works for non-WebGL builds and not WebGL.
	/// 
	/// </summary>
#if UNITY_WEBGL
	public class Matrix
	{
		private struct InternalMatrix
		{
			public int columns;
			public int rows;
			public double[] value;

			public InternalMatrix(double[] val, int col, int row)
			{
				columns = col;
				rows = row;
				value = val;
			}

			public InternalMatrix(int col, int row)
			{
				columns = col;
				rows = row;
				value = new double[col * row];
				InitArray();
			}

			public ref double At(int x, int y)
			{
				return ref value[x + columns * y];
			}

			private void InitArray()
			{
				for (int i = 0; i < columns; i++)
				{
					for (int j = 0; j < rows; j++)
					{
						value[i + columns * j] = 0;
					}
				}
			}
		}

		public int Columns => _info.columns;
		public int Rows => _info.rows;
		public double[] Value => _info.value;
		public double Det => Internal_Determinant(ref _info);

		private double[] _value;
		private InternalMatrix _info;

		public Matrix(double[] matrix, int columns, int rows)
		{
			_value = new double[matrix.Length];
			_info = new InternalMatrix(_value, columns, rows);
			Internal_WriteValue(ref _info, matrix);
		}

		public Matrix(int columns, int rows)
		{
			_value = new double[columns * rows];
			_info = new InternalMatrix(_value, columns, rows);
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
			Internal_MultiplyMatrix(ref m1._info, ref m2._info, ref result._info);
			return result;
		}

		public static Matrix Multiply(Matrix m1, float scalar)
		{
			Matrix result = new Matrix(m1.Columns, m1.Rows);
			Internal_MultiplyMatrixScalar(ref m1._info, scalar, ref result._info);
			return result;
		}

		public static Matrix Add(Matrix m1, Matrix m2)
		{
			Matrix result = new Matrix(m1.Columns, m1.Rows);
			Internal_AddMatrix(ref m1._info, ref m2._info, ref result._info);
			return result;
		}

		public static Matrix Subtract(Matrix m1, Matrix m2)
		{
			Matrix result = new Matrix(m1.Columns, m1.Rows);
			Internal_SubtractMatrix(ref m1._info, ref m2._info, ref result._info);
			return result;
		}

		public static Matrix Pow(Matrix m1, int exponent)
		{
			Matrix result = new Matrix(m1.Columns, m1.Rows);
			Internal_PowMatrix(ref m1._info, exponent, ref result._info);
			return result;
		}

		public static Matrix CreateCopy(Matrix m1)
		{
			Matrix result = new Matrix(m1.Columns, m1.Rows);
			Internal_CopyMatrix(ref m1._info, ref result._info);
			return result;
		}

		public static Matrix Invert(Matrix m)
		{
			Matrix result = new Matrix(m.Columns, m.Rows);
			Internal_InverseMatrix(ref m._info, ref result._info);
			return result;
		}

		public static Matrix Identity(int size)
		{
			Matrix result = new Matrix(size, size);
			Internal_SetIdentity(ref result._info);
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
		public static Matrix operator -(Matrix m1, Matrix m2)
		{
			return Subtract(m1, m2);
		}

		// Internals

		private static void Internal_WriteValue(ref InternalMatrix m, double[] value)
		{
			for (int i = 0; i < m.columns; i++)
			{
				for (int j = 0; j < m.rows; j++)
				{
					m.value[i + j * m.columns] = value[i + j * m.columns];
				}
			}
		}

		private static void Internal_MultiplyMatrix(ref InternalMatrix m1, ref InternalMatrix m2, ref InternalMatrix result)
		{
			if (m1.columns != m2.rows) throw new System.Exception("M1 columns does not match M2 rows");
			Internal_Clear(ref result);

			for (int j = 0; j < m1.rows; j++)
			{
				for (int k = 0; k < m1.columns; k++)
				{
					for (int i = 0; i < m2.columns; i++)
					{
						result.value[i + result.columns * j] += m1.value[k + j * m1.columns] * m2.value[i + k * m2.columns];
					}
				}
			}
		}

		private static void Internal_MultiplyMatrixScalar(ref InternalMatrix m1, float scalar, ref InternalMatrix result)
		{
			for (int i = 0; i < m1.columns; i++)
			{
				for (int j = 0; j < m1.rows; j++)
				{
					result.value[i + j * result.columns] = m1.value[i + j * m1.columns] * scalar;
				}
			}
		}

		private static void Internal_AddMatrix(ref InternalMatrix m1, ref InternalMatrix m2, ref InternalMatrix result)
		{
			if (m1.columns != m2.columns || m1.rows != m2.rows) throw new System.Exception("M1 is not the same size as M2");

			for (int i = 0; i < m1.columns; i++)
			{
				for (int j = 0; j < m1.rows; j++)
				{
					result.value[i + j * result.columns] = m1.value[i + j * m1.columns] + m2.value[i + j * m2.columns];
				}
			}
		}

		private static void Internal_SubtractMatrix(ref InternalMatrix m1, ref InternalMatrix m2, ref InternalMatrix result)
		{
			if (m1.columns != m2.columns || m1.rows != m2.rows) throw new System.Exception("M1 is not the same size as M2");

			for (int i = 0; i < m1.columns; i++)
			{
				for (int j = 0; j < m1.rows; j++)
				{
					result.value[i + j * result.columns] = m1.value[i + j * m1.columns] - m2.value[i + j * m2.columns];
				}
			}
		}

		private static double Internal_Determinant(ref InternalMatrix m)
		{
			// only 2x2 implemented
			double[] mat = m.value;
			return (mat[0] * mat[3] - mat[1] * mat[2]);
		}

		private static void Internal_PowMatrix(ref InternalMatrix m, int pow, ref InternalMatrix result)
		{
			if (m.columns != m.rows) return;

			Internal_SetIdentity(ref result);
			if (pow < 1) { return; }

			InternalMatrix buffer = new InternalMatrix(m.columns, m.rows);
			InternalMatrix powerOfM = new InternalMatrix(m.columns, m.rows);

			int maxIterations = 32;
			Internal_MultiplyMatrix(ref result, ref m, ref powerOfM);
			while (pow >= 1)
			{
				if (pow % 2 == 1)
				{
					Internal_MultiplyMatrix(ref result, ref powerOfM, ref buffer);
					Internal_CopyMatrix(ref buffer, ref result);
				}
				pow = pow >> 1;
				if (pow == 0) break;
				Internal_MultiplyMatrix(ref powerOfM, ref powerOfM, ref buffer);
				Internal_CopyMatrix(ref buffer, ref powerOfM);

				if (maxIterations-- < 0) break;
			}
		}

		private static void Internal_InverseMatrix(ref InternalMatrix m, ref InternalMatrix result)
		{
			if (m.columns != m.rows || m.columns != result.columns || m.rows != result.rows) return;

			InternalMatrix copyM = new InternalMatrix(m.columns, m.rows);
			Internal_CopyMatrix(ref m, ref copyM);

			Internal_SetIdentity(ref result);

			for (int i = 0; i < m.columns; i++)
			{
				Inverse_MakeNthDiagonalOne(ref copyM, ref result, i);
				Inverse_EliminateNonDiagonalRows(ref copyM, ref result, i);
			}
		}

		private static void Inverse_MakeNthDiagonalOne(ref InternalMatrix m, ref InternalMatrix result, int diagonal)
		{
			if (MathOp_NearlyEqual(m.At(diagonal, diagonal), 1)) return;

			if (MathOp_NearlyEqual(m.At(diagonal, diagonal), 0))
			{
				for (int j = diagonal + 1; j < m.rows; j++)
				{
					if (!MathOp_NearlyEqual(m.At(diagonal, j), 0))
					{
						double scalar = 1 / (m.At(diagonal, j));
						RowOp_Multiply(ref m, j, scalar);
						RowOp_Multiply(ref result, j, scalar);
						RowOp_Add(ref m, diagonal, j);
						RowOp_Add(ref result, diagonal, j);
					}
				}
			}

			if (!MathOp_NearlyEqual(m.At(diagonal, diagonal), 1))
			{
				double scalar = 1 / (m.At(diagonal, diagonal));
				RowOp_Multiply(ref m, diagonal, scalar);
				RowOp_Multiply(ref result, diagonal, scalar);
			}
		}

		private static void Inverse_EliminateNonDiagonalRows(ref InternalMatrix m, ref InternalMatrix result, int diagonal)
		{
			for (int j = 0; j < m.rows; j++)
			{
				if (j == diagonal) continue;
				if (MathOp_NearlyEqual(m.At(diagonal, j), 0)) continue;

				double scalar = m.At(diagonal, j) / m.At(diagonal, diagonal);
				RowOp_SubtractScaled(ref m, j, diagonal, scalar);
				RowOp_SubtractScaled(ref result, j, diagonal, scalar);

				// Assert MathOp_NearlyEqual(m.At(i, diagonal), 0)
			}
		}

		private static void RowOp_Add(ref InternalMatrix m, int baseRow, int otherRow)
		{
			for (int i = 0; i < m.columns; i++)
			{
				m.At(i, baseRow) += m.At(i, otherRow);
			}
		}

		private static void RowOp_Subtract(ref InternalMatrix m, int baseRow, int otherRow)
		{
			for (int i = 0; i < m.columns; i++)
			{
				m.At(i, baseRow) -= m.At(i, otherRow);
			}
		}

		private static void RowOp_SubtractScaled(ref InternalMatrix m, int baseRow, int otherRow, double scalar)
		{
			for (int i = 0; i < m.columns; i++)
			{
				m.At(i, baseRow) -= (m.At(i, otherRow) * scalar);
			}
		}

		private static void RowOp_Multiply(ref InternalMatrix m, int row, double scalar)
		{
			for (int i = 0; i < m.columns; i++)
			{
				m.At(i, row) *= scalar;
			}
		}

		private static bool MathOp_NearlyEqual(double a, double b)
		{
			double delta = a - b;
			if (delta < 0) delta = -delta;
			if (delta < double.Epsilon) return true;
			return false;
		}

		private static void Internal_SetIdentity(ref InternalMatrix m)
		{
			if (m.columns != m.rows) return;

			for (int j = 0; j < m.rows; j++)
			{
				for (int i = 0; i < m.columns; i++)
				{
					if (i == j) m.At(i, j) = 1;
					else m.At(i, j) = 0;
				}
			}
		}

		private static void Internal_CopyMatrix(ref InternalMatrix m, ref InternalMatrix newCopy)
		{
			if (m.columns != newCopy.columns || m.rows != newCopy.rows) return;

			for (int i = 0; i < m.columns; i++)
			{
				for (int j = 0; j < m.rows; j++)
				{
					newCopy.At(i, j) = m.At(i, j);
				}
			}
		}

		private static void Internal_Clear(ref InternalMatrix m)
		{
			for (int j = 0; j < m.rows; j++)
			{
				for (int i = 0; i < m.columns; i++)
				{
					m.At(i, j) = 0;
				}
			}
		}
	}
}
#endif
