//
//  test.c
//  test
//
//  Created by Daniel Lewis on 7/12/12.
//  Copyright (c) 2012 __MyCompanyName__. All rights reserved.
//

#include <iostream>
#include <string>
//#include <cstdlib.h>
#include <dlfcn.h>
#include "fizzbuzzlib.h"
using namespace std;

int main(int argc, const char * argv[])
{
    cout << "Printf\n";
    fizzBuzzPrintf(1, 100, "a", "b", "c");
    //cout << "Streams\n";
    //ostream cout;
    //DIDNT WORK fizzBuzzStreams(cout);
    cout << "Simple\n";
    for (int i = 0; i <= 100; ++i)
    {
    	fizzBuzzString(i, "a", "b", "c");
    }
    return 0;
}

