
#include "pch.h"
#include <iostream>

//A NativeAOT .Net DLL is accessed here.

//#define EXP32 extern "C" __declspec(dllexport)
#define IMP32 extern "C" __declspec(dllimport)

#pragma comment(lib, "NativeLibrary.lib")


//#ifdef _INTELLISENSE_ 
////So motzt Intellisense nicht
//IMP32 int32_t rng_crypto_int_32() { return 0; } 
//IMP32 int64_t rng_crypto_int_64() { return 0; } 
//#endif


//A NativeAOT .Net DLL is accessed here.
IMP32 int32_t rng_crypto_int_32();
IMP32 int64_t rng_crypto_int_64();

int main()
{
  printf("NativeAOT: Start C/C++ Console %s", "\n");
  printf("Crypto Random Int32: %d\n", rng_crypto_int_32());
  printf("Crypto Random Int64: %lld\n", rng_crypto_int_64()); 

  printf("\n\nFinish C/C++ Console %s\n","");
  
  //char result = std::getchar();

  return 0;
}



////FOR LINUX, something like this
//#include <dlfcn.h>
//
//typedef int (*add_func)(int, int);
//
//int main() {
//  void* lib = dlopen("./libNativeLibrary.so", RTLD_NOW);
//  add_func add = (add_func)dlsym(lib, "add");
//
//  int result = add(3, 4);
//  return 0;
//}
