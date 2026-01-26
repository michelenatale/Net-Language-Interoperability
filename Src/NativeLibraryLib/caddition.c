#include <stdio.h> 
#include <stdlib.h> 

__declspec(dllexport) int addition(int a, int b)
{
  return a + b; 
}