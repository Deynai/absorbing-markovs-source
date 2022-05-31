#include "pch.h"
//#include "framework.h"
#include "AbsorbingMarkovDLL.h"
#include <stdexcept>

InternalMatrix::InternalMatrix(double* value, int columns, int rows)
{
	this->columns = columns;
	this->rows = rows;
	this->value = value;
}

InternalMatrix::InternalMatrix(int columns, int rows) {
	this->columns = columns;
	this->rows = rows;
	this->value = new double[columns * rows];
	InitArray();
}

double& InternalMatrix::At(int x, int y) {
	return value[x + columns * y];
}

InternalMatrix::~InternalMatrix() {
	if (value) {
		delete[] value;
	}
}

void InternalMatrix::InitArray() {
	for (size_t i = 0; i < columns; i++)
	{
		for (size_t j = 0; j < rows; j++)
		{
			value[i + columns * j] = 0;
		}
	}
}