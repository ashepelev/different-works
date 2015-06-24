#include <sys/time.h>
#include "mpi.h"

int __MPI_COMM_WORLD = (MPI_Comm )91;
int __MPI_COMM_SELF = (MPI_Comm )92;

int __MPI_CHAR = (MPI_Datatype )1;
int __MPI_INTEGER1 = (MPI_Datatype )3;
int __MPI_INTEGER2 = (MPI_Datatype )4;
int __MPI_INTEGER4 = (MPI_Datatype )6;
int __MPI_INTEGER8 = (MPI_Datatype )8;

int __MPI_REAL4 = (MPI_Datatype )10;
int __MPI_REAL8 = (MPI_Datatype )11;

#define Width 131072 // Ширина
#define Height 1000 // Высота

#define PROCESSES_COUNT           2
#define ELEMENTS_IN_EACH_PROCESS 65536
#define ITERATIONS_COUNT 100

#define tau 1.0
#define a 0.25
#define h 1.0


int main(int argc, char** argv)
{
	int i, t;
	double *Old;
	double *New;
	double coeff, controlSum, *BL, *BR;
//	double time1, time2;
	struct timeval tp1;
	struct timeval tp2;
	coeff = (double)1/4;
	BL = (double*)malloc(Height * sizeof(double));
	BR = (double*)malloc(Height * sizeof(double));
//	matr = (double*)malloc(Height * Width * sizeof(double));
	printf("a = %f, h = %f, tau = %f\n",a,h,tau);
	for (i = 0; i < Height; i = i + 1)
	{
		BL[i] = 1;
		BR[i] = 1;
	}

	{
		Old = (double*)malloc(Width * sizeof(double));
		for (i = 0; i < Width; i = i + 1)
		{
			Old[i] = 1;
		}
	}

	{
		New = (double*)malloc(Width * sizeof(double));
	}
	gettimeofday(&tp1,NULL);
	{
		for (t = 0; t < Height; t = t + 1)
		{
			for (i = 0; i < Width; i = i + 1)
			{
				if(i == 0)
				{
					New[i] = BL[t];
				}
				else if(i == Width-1)
				{
					New[i] = BR[t];
				}
				else
				{
				    if (t == 0)
                        New[i] = (a/h) * (Old[i - 1] - 2 *  Old[i] + Old[i + 1]);
                    else
                        New[i] = (New[i] - Old[i])/tau - (a/h) * (Old[i - 1] - 2 *  Old[i] + Old[i + 1]);
				}
			}

			for (i = 0; i < Width; i = i + 1)
			{
				Old[i] = New[i];
			}
		}
	}

	gettimeofday(&tp2,NULL);
	long f =tp2.tv_usec-tp1.tv_usec+1000000*(tp2.tv_sec-tp1.tv_sec);
	printf("time = %f\n", f );

    free(Old);
    free(New);
	free(BL);
	free(BR);
}
