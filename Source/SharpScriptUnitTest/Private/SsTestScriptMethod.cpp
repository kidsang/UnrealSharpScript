#include "SsTestScriptMethod.h"

FString USsTestScriptMethod::NumericStructToString(const FSsTestNumericStruct& InStruct)
{
    return FString::Printf(TEXT("X=%d Y=%d"), InStruct.X, InStruct.Y);
}
