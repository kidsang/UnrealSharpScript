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

bool USsTestScriptMethod::Equals(const FSsTestNumericStruct& Lhs, const FSsTestNumericStruct& Rhs)
{
	return Lhs.X == Rhs.X && Lhs.Y == Rhs.Y;
}

FSsTestNumericStruct USsTestScriptMethod::AddInt(const FSsTestNumericStruct& Lhs, int Rhs)
{
	return FSsTestNumericStruct(Lhs.X + Rhs, Lhs.Y + Rhs);
}

FSsTestNumericStruct USsTestScriptMethod::SubtractInt(const FSsTestNumericStruct& Lhs, int Rhs)
{
	return FSsTestNumericStruct(Lhs.X - Rhs, Lhs.Y - Rhs);
}

FSsTestNumericStruct USsTestScriptMethod::MultiplyInt(const FSsTestNumericStruct& Lhs, int Rhs)
{
	return FSsTestNumericStruct(Lhs.X * Rhs, Lhs.Y * Rhs);
}

FSsTestNumericStruct USsTestScriptMethod::DivideInt(const FSsTestNumericStruct& Lhs, int Rhs)
{
	return FSsTestNumericStruct(Lhs.X / Rhs, Lhs.Y / Rhs);
}
