#include "pch.h"
//#include "framework.h"
#include "AbsorbingMarkovDLL.h"
#include <stdexcept>
#include <memory>

void Internal_WriteValue(InternalMatrix* m, double* value) {
	for (size_t i = 0; i < m->columns; i++)
	{
		for (size_t j = 0; j < m->rows; j++)
		{
			m->value[i + j * m->columns] = value[i + j * m->columns];
		}
	}
}

void Internal_MultiplyMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result)
{
	if (m1->columns != m2->rows) throw std::invalid_argument("M1 columns does not match M2 rows");
	Internal_Clear(result);

	for (size_t j = 0; j < m1->rows; j++)
	{
		for (size_t k = 0; k < m1->columns; k++)
		{
			for (size_t i = 0; i < m2->columns; i++)
			{
				result->value[i + result->columns * j] += m1->value[k + j * m1->columns] * m2->value[i + k * m2->columns];
			}
		}
	}
}

void Internal_MultiplyMatrixScalar(InternalMatrix* m1, float scalar, InternalMatrix* result)
{
	for (size_t i = 0; i < m1->columns; i++)
	{
		for (size_t j = 0; j < m1->rows; j++)
		{
			result->value[i + j * result->columns] = m1->value[i + j * m1->columns] * scalar;
		}
	}
}

void Internal_AddMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result)
{
	if (m1->columns != m2->columns || m1->rows != m2->rows) throw std::invalid_argument("M1 is not the same size as M2");

	for (size_t i = 0; i < m1->columns; i++)
	{
		for (size_t j = 0; j < m1->rows; j++)
		{
			result->value[i + j * result->columns] = m1->value[i + j * m1->columns] + m2->value[i + j * m2->columns];
		}
	}
}

void Internal_SubtractMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result)
{
	if (m1->columns != m2->columns || m1->rows != m2->rows) throw std::invalid_argument("M1 is not the same size as M2");

	for (size_t i = 0; i < m1->columns; i++)
	{
		for (size_t j = 0; j < m1->rows; j++)
		{
			result->value[i + j * result->columns] = m1->value[i + j * m1->columns] - m2->value[i + j * m2->columns];
		}
	}
}

int Internal_Determinant(InternalMatrix* m) {
	// only 2x2 implemented
	double* mat = m->value;
	return mat[0] * mat[3] - mat[1] * mat[2];
}

void Internal_PowMatrix(InternalMatrix* m, int pow, InternalMatrix* result) {
	if (m->columns != m->rows) return;

	Internal_SetIdentity(result);
	if (pow < 1) { return; }

	InternalMatrix* buffer = new InternalMatrix(m->columns, m->rows);
	InternalMatrix* powerOfM = new InternalMatrix(m->columns, m->rows);

	int maxIterations = 32;
	Internal_MultiplyMatrix(result, m, powerOfM);
	while (pow >= 1) {
		if (pow % 2 == 1) {
			Internal_MultiplyMatrix(result, powerOfM, buffer);
			Internal_CopyMatrix(buffer, result);
		}
		pow = pow >> 1;
		if (pow == 0) break;
		Internal_MultiplyMatrix(powerOfM, powerOfM, buffer);
		Internal_CopyMatrix(buffer, powerOfM);

		if (maxIterations-- < 0) break;
	}

	delete buffer;
	delete powerOfM;
}

void Internal_InverseMatrix(InternalMatrix* m, InternalMatrix* result) {
	if (m->columns != m->rows || m->columns != result->columns || m->rows != result->rows) return;

	InternalMatrix* copyM = new InternalMatrix(m->columns, m->rows);
	Internal_CopyMatrix(m, copyM);

	Internal_SetIdentity(result);

	for (size_t i = 0; i < m->columns; i++)
	{
		Inverse_MakeNthDiagonalOne(copyM, result, i);
		Inverse_EliminateNonDiagonalRows(copyM, result, i);
	}

	delete copyM;
}

void Inverse_MakeNthDiagonalOne(InternalMatrix* m, InternalMatrix* result, int diagonal) {
	if (MathOp_NearlyEqual(m->At(diagonal, diagonal), 1)) return;

	if (MathOp_NearlyEqual(m->At(diagonal, diagonal), 0)) {
		for (int j = diagonal+1; j < m->rows; j++)
		{
			if (!MathOp_NearlyEqual(m->At(diagonal, j), 0)) {
				double scalar = 1 / (m->At(diagonal, j));
				RowOp_Multiply(m, j, scalar);
				RowOp_Multiply(result, j, scalar);
				RowOp_Add(m, diagonal, j);
				RowOp_Add(result, diagonal, j);
			}
		}
	}

	if (!MathOp_NearlyEqual(m->At(diagonal, diagonal), 1)) {
		double scalar = 1 / (m->At(diagonal, diagonal));
		RowOp_Multiply(m, diagonal, scalar);
		RowOp_Multiply(result, diagonal, scalar);
	}
}

void Inverse_EliminateNonDiagonalRows(InternalMatrix* m, InternalMatrix* result, int diagonal) {
	for (int j = 0; j < m->rows; j++)
	{
		if (j == diagonal) continue;
		if (MathOp_NearlyEqual(m->At(diagonal, j), 0)) continue;

		double scalar = m->At(diagonal, j) / m->At(diagonal, diagonal);
		RowOp_SubtractScaled(m, j, diagonal, scalar);
		RowOp_SubtractScaled(result, j, diagonal, scalar);
		
		// Assert MathOp_NearlyEqual(m->At(i, diagonal), 0)
	}
}

void RowOp_Add(InternalMatrix* m, int baseRow, int otherRow) {
	for (size_t i = 0; i < m->columns; i++)
	{
		m->At(i, baseRow) += m->At(i, otherRow);
	}
}

void RowOp_Subtract(InternalMatrix* m, int baseRow, int otherRow) {
	for (size_t i = 0; i < m->columns; i++)
	{
		m->At(i, baseRow) -= m->At(i, otherRow);
	}
}

void RowOp_SubtractScaled(InternalMatrix* m, int baseRow, int otherRow, double scalar) {
	for (size_t i = 0; i < m->columns; i++)
	{
		m->At(i, baseRow) -= (m->At(i, otherRow) * scalar);
	}
}

void RowOp_Multiply(InternalMatrix* m, int row, double scalar){
	for (size_t i = 0; i < m->columns; i++)
	{
		m->At(i, row) *= scalar;
	}
}

bool MathOp_NearlyEqual(double a, double b) {
	double delta = a - b;
	if (delta < 0) delta = -delta;
	if (delta < 0.000000000001) return true;
	return false;
}

void Internal_SetIdentity(InternalMatrix* m) {
	if (m->columns != m->rows) return;

	for (size_t j = 0; j < m->rows; j++)
	{
		for (size_t i = 0; i < m->columns; i++)
		{
			if (i == j) m->At(i, j) = 1;
			else m->At(i, j) = 0;
		}
	}
}

void Internal_CopyMatrix(InternalMatrix* m, InternalMatrix* newCopy) {
	if (m->columns != newCopy->columns || m->rows != newCopy->rows) return;

	for (size_t i = 0; i < m->columns; i++)
	{
		for (size_t j = 0; j < m->rows; j++)
		{
			newCopy->At(i, j) = m->At(i, j);
		}
	}
}

void Internal_Clear(InternalMatrix* m) {
	for (size_t j = 0; j < m->rows; j++)
	{
		for (size_t i = 0; i < m->columns; i++)
		{
			m->At(i, j) = 0;
		}
	}
}

double& At(InternalMatrix* m, int i, int j) {
	if (i < 0 || i >= m->columns || j < 0 || j > m->rows) throw std::out_of_range("id out of range");
	return m->value[i + m->columns * j];
}
