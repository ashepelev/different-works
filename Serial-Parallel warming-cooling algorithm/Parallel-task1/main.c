#include <stdio.h>
#include <stdlib.h>

#include <sys/time.h>
#include "mpi.h"


#define Width 131072    // ������ �������
#define Height 1000  // ������ �������

#define PROCESSES_COUNT           2
#define ELEMENTS_IN_EACH_PROCESS 65536

#define ITERATIONS_COUNT 100


int main(int argc, char** argv)
{
    /* MPI VARS */
    int id, numprocs;
    MPI_Status status;
    /* MPI ENDS */

    /* MPI INITIALIZE */
     MPI_Init(&argc,&argv);
     MPI_Comm_size(MPI_COMM_WORLD,&numprocs);
     MPI_Comm_rank(MPI_COMM_WORLD,&id);

    /* MPI init ENDS */

    /* LOCAL PROGRAM VARS */
	int i, t; // iterators
	double tau; // delta-time
	double *Old;  // Old
	double *New;  // New
	double coeff, // Equation coeff
	controlSum,
	*BL, // Left task boarder
	*BR; // Right
	double time1, time2;
	struct timeval tp1;
	struct timeval tp2;
	/* END LOCAL */

	/* VARS INIT */
	coeff = (double)1/4;
	BL = (double*)malloc(Height * sizeof(double)); // ����� ������� ����������
	BR = (double*)malloc(Height * sizeof(double)); // ������ ������� ����������
	for (i = 0; i < Height; i = i + 1)
	{
		BL[i] = 1;
		BR[i] = 1;
	}
	Old = (double*)malloc(Width * sizeof(double));
		for (i = 0; i < Width; i = i + 1)
		{
			Old[i] = 0;
		}
    New = (double*)malloc(Width * sizeof(double));
	/* VARS INIT END */

	/* MPI borders */
	int mypart = Height/numprocs;
	int upperb, downb;
	upperb = id * mypart;
	downb = (id + 1) * mypart - 1;
	/* MPI borders END */

	if (id == 0) // master
    {
        printf("Start!\n");
        time1 = MPI_Wtime();
    }
		for (t = upperb; t < downb; t = t + 1)
		{
			for (i = 0; i < Width; i = i + 1)
			{
				if(i == 0)  // ���� ��� ����� ����
				{
					New[i] = BL[t];
				}
				else if(i == Width-1)   // ���� ��� ����� ����
				{
					New[i] = BR[t];
				}
				else            // ���� ��� � ��������
				{
					New[i] = coeff * (Old[i - 1] - 2 *  Old[i] + Old[i + 1]); // ������� ���������/���������
				}
			}

			for (i = 0; i < Width; i = i + 1)
			{
				Old[i] = New[i];    // ���������� �������� ���������
			}
        }
    if (id == 0)
    {
        time2 = MPI_Wtime();
        double delta = time2 - time1;
        printf("time = %f\n", delta);
    }

    free(Old);
    free(New);
	free(BL);
	free(BR);
}
