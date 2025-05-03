#include "SsTestScriptMethod.h"

FString USsTestScriptMethod::NumericStructToString(const FSsTestNumericStruct& InStruct)
{
    return FString::Printf(TEXT("X=%d Y=%d"), InStruct.X, InStruct.Y);
}

void USsTestScriptMethod::TestDefaultValue(FSsTestNumericStruct& InStruct, const FVector& InVector, const FVector& InVector2)
{
	InStruct.X = (int)InVector.X;
	InStruct.Y = (int)InVector.Y;
}

FIntPoint USsTestScriptMethod::NumericStructToIntPoint(const FSsTestNumericStruct& InStruct)
{
	return FIntPoint(InStruct.X, InStruct.Y);
}
