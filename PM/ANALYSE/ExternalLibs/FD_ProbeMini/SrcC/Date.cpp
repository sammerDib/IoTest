/*
 * $Id: Date.cpp 5752 2007-10-04 09:36:25Z n-combe $
 */


#include "..\stdafx.h"

int dateWriteISODate(char *buf, time_t t)
{
	tm * timeptr = localtime(&t); /* Il ne faut pas libérer ce pointeur car il est partagé, libéré par le système */

	if (timeptr == NULL)
	{
		return FALSE;
	}

	return sprintf(buf, "%.4d-%.2d-%.2dT%.2d:%.2d:%.2d",
        1900 + timeptr->tm_year, 
		timeptr->tm_mon + 1,
		timeptr->tm_mday, 
		timeptr->tm_hour,
		timeptr->tm_min,
		timeptr->tm_sec
	);
}

int dateWriteISODate(char *buf)
{
	time_t now;
	time(&now);
	return dateWriteISODate(buf, now);
}

