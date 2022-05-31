
#ifdef ABSORBINGMARKOVDLL_EXPORTS
#define ABSORBINGMARKOVDLL_API __declspec(dllexport)
#else
#define ABSORBINGMARKOVDLL_API __declspec(dllimport)
#endif

extern "C" {
	class ABSORBINGMARKOVDLL_API InternalMatrix {
	public:
		int columns;
		int rows;
		double* value;

		InternalMatrix(double* value, int col, int row);
		InternalMatrix(int col, int row);
		double& At(int x, int y);
		~InternalMatrix();

	private:
		void InitArray();
	};

	ABSORBINGMARKOVDLL_API void Internal_WriteValue(InternalMatrix* obj, double* value);
	ABSORBINGMARKOVDLL_API void Internal_MultiplyMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result);
	ABSORBINGMARKOVDLL_API void Internal_MultiplyMatrixScalar(InternalMatrix* m1, float scalar, InternalMatrix* result);
	ABSORBINGMARKOVDLL_API void Internal_AddMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result);
	ABSORBINGMARKOVDLL_API void Internal_SubtractMatrix(InternalMatrix* m1, InternalMatrix* m2, InternalMatrix* result);
	ABSORBINGMARKOVDLL_API int Internal_Determinant(InternalMatrix* m);
	ABSORBINGMARKOVDLL_API void Internal_PowMatrix(InternalMatrix* m, int pow, InternalMatrix* result);
	ABSORBINGMARKOVDLL_API void Internal_SetIdentity(InternalMatrix* m);
	ABSORBINGMARKOVDLL_API void Internal_CopyMatrix(InternalMatrix* m, InternalMatrix* target);
	ABSORBINGMARKOVDLL_API void Internal_InverseMatrix(InternalMatrix* m, InternalMatrix* result);

	void Internal_Clear(InternalMatrix* m);
	void Inverse_MakeNthDiagonalOne(InternalMatrix* m, InternalMatrix* result, int diagonal);
	void Inverse_EliminateNonDiagonalRows(InternalMatrix* m, InternalMatrix* result, int diagonal);
	void RowOp_Add(InternalMatrix* m, int baseRow, int otherRow);
	void RowOp_Subtract(InternalMatrix* m, int baseRow, int otherRow);
	void RowOp_Multiply(InternalMatrix* m, int baseRow, double scalar);
	void RowOp_SubtractScaled(InternalMatrix* m, int baseRow, int otherRow, double scalar);
	bool MathOp_NearlyEqual(double a, double b);
	double& At(InternalMatrix* m, int x, int y);
}