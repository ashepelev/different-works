

#include "MyFunctions.h"
#include <iostream>


// r[i+1] = {r[i]+pi}
double function1(double x)
{
	double intpart;
	return modf((x+pi),&intpart);
}

// r[i+1] = {e^(pi+r[i])}
double function2(double x)
{
	double intpart;
	return modf((pow(e,pi+x)),&intpart);
}

// r[i+1] = {(pi-2+r[i])^2}
double function3(double x)
{
	double intpart;
	return modf((pow(pi-2+x,2)),&intpart);
}

double calcSquare(double r, func f)
{
	double square = 0;
	long rightPoints = 0;
	long totalPoints = (long)8*r*r*r*(accuracy/100.0);
	double prevx = 0.15263611;
	double prevy = 0.35857256;
	double prevz = 0.58275923;
	double intpart;
	for (long i = 0; i < totalPoints; ++i)
	{
		double x = prevx = f(prevx);
		double y = prevy = f(prevy);
		double z = prevz = f(prevz);
		//std::cerr << prevx << " ";
		x-=(1/2.0); x*=(2*r);
		y-=(1/2.0); y*=(2*r);
		z-=(1/2.0); z*=(2*r);
		if (sqrt(x*x+y*y+z*z) <= r)
			++rightPoints;
	}
	square = ((double) rightPoints/(totalPoints*(1.0))*(8*r*r*r));
	return square;
}



double rightSquare(double r)
{
	 return (4/3.0)*pi*(r*r*r);
}
