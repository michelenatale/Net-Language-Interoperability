
#include <time.h> 
#include <stdlib.h> 

__declspec(dllexport) void fill_rng_bytes(
  unsigned char* buffer, int length)
{ 
  srand((unsigned)time(NULL)); 
  for (int i = 0; i < length; i++) 
    buffer[i] = rand() % 256; 
}