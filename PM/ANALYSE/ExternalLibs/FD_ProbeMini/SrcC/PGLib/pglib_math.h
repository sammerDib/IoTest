inline PGLVector operator*(const float f,const PGLVector& v)
{
	PGLVector r;
	r.x=v.x*f;
	r.y=v.y*f;
	r.z=v.z*f;
	return r;
}

inline PGLVector operator*=(PGLVector& v,const float f)
{
	return v=f*v;
}

inline PGLVector operator+(const PGLVector& v1,const PGLVector& v2)
{
	PGLVector r;
	r.x=v1.x+v2.x;
	r.y=v1.y+v2.y;
	r.z=v1.z+v2.z;
	return r;
}

inline PGLVector operator+=(PGLVector& v1,const PGLVector& v2)
{
	return v1=v1+v2;
}

inline PGLVector operator-(const PGLVector& v1,const PGLVector& v2)
{
	PGLVector r;
	r.x=v1.x-v2.x;
	r.y=v1.y-v2.y;
	r.z=v1.z-v2.z;
	return r;
}

inline PGLVector operator-=(PGLVector& v1,const PGLVector& v2)
{
	return v1=v1-v2;
}

inline bool operator==(const PGLVector& v1,const PGLVector& v2)
{
	return ((v1.x==v2.x)&&(v1.y==v2.y)&&(v1.z==v2.z));
}

inline bool operator!=(const PGLVector& v1,const PGLVector& v2)
{
	return ((v1.x!=v2.x)||(v1.y!=v2.y)||(v1.z!=v2.z));
}

inline PGLVector operator^(const PGLVector& v1,const PGLVector& v2)
{
	PGLVector r;
	r.x=v1.y*v2.z-v1.z*v2.y;
	r.y=v1.z*v2.x-v1.x*v2.z;
	r.z=v1.x*v2.y-v1.y*v2.x;
	return r;
}

inline PGLVector operator^=(PGLVector& v1,const PGLVector& v2)
{
	return v1=v1^v2;
}

inline float operator*(const PGLVector& v1,const PGLVector& v2)
{
	return (v1.x*v2.x+v1.y*v2.y+v1.z*v2.z);
}

inline PGLMatrix operator*(const PGLMatrix& M1,const PGLMatrix& M2)
{
	PGLMatrix R;

	for(uint i=0;i<4;i++)
		for(uint j=0;j<4;j++)
		{
			float r=0;
			for(uint c1=0;c1<4;c1++)
				for(uint l2=0;l2<4;l2++)
					r+=M1.ml[i+(c1<<2)]*M2.ml[l2+(j<<2)];
			R.ml[i+(j<<2)]=r;
		}
	return R;
}

inline PGLMatrix operator*=(PGLMatrix& M1,const PGLMatrix& M2)
{
	return M1=M1*M2;
}

inline PGLMatrix operator+(const PGLMatrix& M1,const PGLMatrix& M2)
{
	PGLMatrix R;

	for(uint i=0;i<4;i++)
		for(uint j=0;j<4;j++)
			R.ml[i+(j<<2)]=M1.ml[i+(j<<2)]+M2.ml[i+(j<<2)];
	return R;
}

inline PGLMatrix operator+=(PGLMatrix& M1,const PGLMatrix& M2)
{
	return M1=M1+M2;
}

inline PGLMatrix operator-(const PGLMatrix& M1,const PGLMatrix& M2)
{
	PGLMatrix R;

	for(uint i=0;i<4;i++)
		for(uint j=0;j<4;j++)
			R.ml[i+(j<<2)]=M1.ml[i+(j<<2)]-M2.ml[i+(j<<2)];
	return R;
}

inline PGLMatrix operator-=(PGLMatrix& M1,const PGLMatrix& M2)
{
	return M1=M1-M2;
}

inline PGLMatrix operator*(const float f,const PGLMatrix& M)
{
	PGLMatrix R;

	for(uint i=0;i<4;i++)
		for(uint j=0;j<4;j++)
			R.ml[i+(j<<2)]=f*M.ml[i+(j<<2)];
	return R;
}

inline PGLMatrix operator*=(PGLMatrix& M,const float f)
{
	return M=f*M;
}

inline bool operator==(const PGLMatrix& M1,const PGLMatrix& M2)
{
	for(uint j=0;j<4;j++)
		for(uint i=0;i<4;i++)
			if(M1.ml[i+(j<<2)]!=M2.ml[i+(j<<2)])
				return false;
	return true;
}

inline bool operator!=(const PGLMatrix& M1,const PGLMatrix& M2)
{
	for(uint j=0;j<4;j++)
		for(uint i=0;i<4;i++)
			if(M1.ml[i+(j<<2)]!=M2.ml[i+(j<<2)])
				return true;
	return false;
}

inline PGLVector operator*(const PGLMatrix& M,const PGLVector& V)
{
	PGLVector R;
	R.x=M._11*V.x+M._12*V.y+M._13*V.z+M._14; //1.0f=w
	R.y=M._21*V.x+M._22*V.y+M._23*V.z+M._24; //1.0f=w
	R.z=M._31*V.x+M._32*V.y+M._33*V.z+M._34; //1.0f=w
	//R.w
	return R;
}
