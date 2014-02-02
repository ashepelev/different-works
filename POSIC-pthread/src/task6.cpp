//============================================================================
// Name        : task6.cpp
// Author      : 
// Version     :
// Copyright   : 
// Description : Hello World in C++, Ansi-style
//============================================================================

#include <iostream>
#include <pthread.h>
#include "MyFunctions.h"

using namespace std;

void* MyThreadProc(void* pvData)
		{
			Data * d;
			d = (Data*)pvData;
			d->volume = calcSquare(d->radius,d->function);
			pvData = d;
			return 0;
		}

int main() {
	cout << "Type in radius: ";
	double radius;
	cin >> radius;
	cout << endl;

	pthread_t hThread1,hThread2,hThread3;

	Data arrdata[3];
	for (int i = 0; i < 3; i++)
	{
		arrdata[i].radius = radius;
		arrdata[i].volume = 0;
	}
	arrdata[0].function = function1;
	arrdata[1].function = function2;
	arrdata[2].function = function2;

	pthread_create(&hThread1,NULL,MyThreadProc,(void*)&arrdata[0]);
	pthread_create(&hThread2,NULL,MyThreadProc,(void*)&arrdata[1]);
	pthread_create(&hThread3,NULL,MyThreadProc,(void*)&arrdata[2]);

	pthread_join(hThread1,NULL);
	pthread_join(hThread2,NULL);
	pthread_join(hThread3,NULL);

	for (int i = 0; i < 3; i++)
	{
		cout << "Calculated square" << (i+1) << ": " << arrdata[i].volume << endl;
	}
	cout << "Right square: " << rightSquare(radius) << endl;

	return 0;
}
