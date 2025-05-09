﻿using EpicGames.Core;
using EpicGames.UHT.Types;
using SharpScriptBindingGenerator.PropertyTranslators;

namespace SharpScriptBindingGenerator.Utilities;

public static class StructUtilities
{
	public static bool IsBlittable(this UhtScriptStruct structObj)
	{
		if (structObj.ScriptStructFlags.HasAnyFlags(EStructFlags.Immutable))
		{
			return true;
		}

		if (PropertyTranslatorManager.BlittableStructs.Contains(structObj.EngineName))
		{
			return true;
		}

		// Any struct we haven't manually exported is not blittable, yet.
		// The fix for this is to add a header parser to check for non-UPROPERTY properties in the struct.
		// Because a struct can be recognized as blittable by the reflection data,
		// but have a non-UPROPERTY property that is not picked up by UHT, that makes it not blittable causing a mismatch in memory layout.
		// This is a temporary solution until we can get that working.
		return false;
	}
}
