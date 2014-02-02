
#ifndef MYFUNCTIONS_H
#define MYFUNCTIONS_H

#include <cmath>

const double pi = 3.14159265;
const double e =  2.71828182;
const double accuracy = 1000000; // 10000%

typedef double (*func)(double);

struct Data
{
	double radius;
	func function;
	double volume;
};


double function1(double x);

double function2(double x);

double function3(double x);

double calcSquare(double r, func f);

double rightSquare(double r);

#endif /* MYFUNCTIONS_H */
