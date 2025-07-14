
//Table de linearisation interpolee lineairement

typedef struct
{
	int N;
	double* LX;
	double* LY;
} SPG_LINTABLE;

//table de linearisation
int SPG_CONV SPG_LinInit(SPG_LINTABLE& P, double* D, int NX, int NY);
int SPG_CONV SPG_LinInit(SPG_LINTABLE& P, char* Workdir, char* F);
void SPG_CONV SPG_LinClose(SPG_LINTABLE& P);
double SPG_CONV SPG_Lin(double x, SPG_LINTABLE& P);
double SPG_CONV SPG_LinGetSlope(SPG_LINTABLE& P);
void SPG_CONV SPG_LinAddSlope(double S, SPG_LINTABLE& P);

//Liste lineaire de coefficients de polynome

typedef struct
{

	int N;
	double* A;
} SPG_LINPOLY;

//polynome de linearisation
int SPG_CONV SPG_PolyInit(SPG_LINPOLY& P, double* D, int NX, int NY);
int SPG_CONV SPG_PolyInit(SPG_LINPOLY& P, char* Workdir, char* F);
void SPG_CONV SPG_PolyClose(SPG_LINPOLY& P);
double SPG_CONV SPG_Poly(double x, SPG_LINPOLY& P);
double SPG_CONV SPG_PolyGetSlope(SPG_LINPOLY& P);
void SPG_CONV SPG_PolyAddSlope(double S, SPG_LINPOLY& P);


//##############  INTERPOLATION OU POLYNOME  ###############

typedef struct SPG_LINAUTO
{
	typedef union
	{
		SPG_LINTABLE Table;
		SPG_LINPOLY Poly;
	} SPG_LIN;
	union
	{
		int N;
		SPG_LIN Lin;
	};
	union
	{
		double (SPG_CONV *F)(double x, SPG_LINAUTO::SPG_LIN& P);
		double (SPG_CONV *TABLEF)(double x, SPG_LINTABLE& P);
		double (SPG_CONV *POLYF)(double x, SPG_LINPOLY& P);
	};
	union
	{
		double (SPG_CONV *GS)(SPG_LINAUTO::SPG_LIN& P);
		double (SPG_CONV *TABLEGS)(SPG_LINTABLE& P);
		double (SPG_CONV *POLYGS)(SPG_LINPOLY& P);
	};
	union
	{
		void (SPG_CONV *AS)(double S, SPG_LINAUTO::SPG_LIN& P);
		void (SPG_CONV *TABLEAS)(double S, SPG_LINTABLE& P);
		void (SPG_CONV *POLYAS)(double S, SPG_LINPOLY& P);
	};
	union
	{
		void (SPG_CONV *Close)(SPG_LINAUTO::SPG_LIN& P);
		void (SPG_CONV *TABLEClose)(SPG_LINTABLE& P);
		void (SPG_CONV *POLYClose)(SPG_LINPOLY& P);
	};
} SPG_LINAUTO;

int SPG_CONV SPG_LinAutoInit(SPG_LINAUTO& P, char* Workdir, char* F);
void SPG_CONV SPG_LinAutoClose(SPG_LINAUTO& P);
double SPG_CONV SPG_LinAuto(double x, SPG_LINAUTO& P);
double SPG_CONV SPG_LinAutoGetSlope(SPG_LINAUTO& P);
void SPG_CONV SPG_LinAutoAddSlope(double S, SPG_LINAUTO& P);

//##############  INTERPOLATION OU POLYNOME  ###############

//##############  SPLINES (non testé)  ###############

typedef struct
{
	double x;
	double a[];
} SPG_LINPOLYLINE;

#define SPG_POLYLINE(P,nx) (*(SPG_LINPOLYLINE*)(P.A+(nx)*(P.SLNX)))

typedef struct
{

	int SLNX;
	int SLNY;

	double* A; //SPG_LINPOLYLINE

	double xmin;
	double xmax;
	double xcoeff;
	int N;
	int Order;
} SPG_LINPOLYTABLE;

//polynome de linearisation
int SPG_CONV SPG_PolyTableInit(SPG_LINPOLYTABLE& P, char* Workdir, char* F);
int SPG_CONV SPG_PolyTableClose(SPG_LINPOLYTABLE& P);
double SPG_CONV SPG_PolyTable(double x, SPG_LINPOLYTABLE& P);

//##############  SPLINES (non testé)  ###############

