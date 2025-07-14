#ifndef H3MATRIXOPERATION_H
#define H3MATRIXOPERATION_H

#include "H3Matrix.h"

enum Strel_Type
{
	disk,
};

 enum fspecial_Type
 {
	 gaussian,
 };

 enum Conv_Type
 {
	 same,
	 full,
 };

 enum Pad_Type
 {
	 both,
 };

class  CH3MatrixOperations
{
//attributs
public:

private:

//méthodes

public:
	static H3_MATRIX_FLT32 Alta_CreateCoefMap3(int radius);
	static H3_MATRIX_UINT8 Alta_strel(Strel_Type type, int radius);
	static H3_MATRIX_UINT8 Alta_imDilate(H3_MATRIX_UINT8 SourceMatrix,H3_MATRIX_UINT8 &StrelMatrix);
	static H3_MATRIX_FLT32 fspecial(fspecial_Type type,double p2,double p3);
	static H3_MATRIX_FLT32 conv(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix,Conv_Type type);
	static H3_MATRIX_FLT32 padarray(H3_MATRIX_FLT32 &AMatrix,H3_FLT32 value,int before,int after ,Pad_Type type);
	static H3_MATRIX_FLT32 sparse(H3_MATRIX_FLT32 iVector,H3_MATRIX_FLT32 jVector,H3_MATRIX_FLT32 valueVector,int iMax = -1,int jMax=-1,int sMax= -1);
	static H3_FLT32 mean(H3_MATRIX_FLT32 & Matrix);
	static H3_MATRIX_FLT32 mat_sqrt(H3_MATRIX_FLT32 & Matrix);
	static H3_MATRIX_FLT32 scalar_mul(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix);
	static H3_MATRIX_FLT32 affineTransformation(H3_MATRIX_FLT32 gxMatrix,H3_MATRIX_FLT32 gyMatrix,H3_MATRIX_FLT32 &D11Matrix, H3_MATRIX_FLT32 &D12Matrix,H3_MATRIX_FLT32 &D22Matrix);
	static int eig_2x2(H3_MATRIX_FLT32 &sourceMatrix,H3_MATRIX_FLT32 &eigenValues, H3_MATRIX_FLT32 &eigenVectors);
	static H3_MATRIX_FLT32 laplacian_matrix_tensor(int H,int W,H3_MATRIX_FLT32 D11,H3_MATRIX_FLT32 D12,H3_MATRIX_FLT32 D21,H3_MATRIX_FLT32 D22);
	static H3_MATRIX_FLT32 calculate_f_tensor(H3_MATRIX_FLT32 p,H3_MATRIX_FLT32 q,H3_MATRIX_FLT32 D11,H3_MATRIX_FLT32 D12,H3_MATRIX_FLT32 D21,H3_MATRIX_FLT32 D22);

private:
	static H3_MATRIX_UINT8 Create_DiskStrelMatrix(int radius);
	static void ReplaceDilateMatrix(H3_MATRIX_UINT8 &sourceMatrix, H3_MATRIX_UINT8 &strelMatrix, long xPos,long yPos);
	static void meshgrid(int x_start,int x_end,int y_start,int y_end, H3_MATRIX_FLT32 &xMatrix,H3_MATRIX_FLT32 &yMatrix);
	static H3_MATRIX_FLT32 conv_full(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix);
	static H3_MATRIX_FLT32 fspecial_gaussian(double p2,double p3);
	static H3_FLT32 LocalConv(H3_MATRIX_FLT32 &AMatrix,H3_MATRIX_FLT32 &BMatrix,int LiPos,int CoPos);
	static H3_MATRIX_FLT32 padarray_Both(H3_MATRIX_FLT32 &AMatrix,H3_FLT32 value,int before,int after);
	static H3_FLT32 maxValue(H3_MATRIX_FLT32 &Matrix);
};

#endif // H3MATRIXOPERATION_H