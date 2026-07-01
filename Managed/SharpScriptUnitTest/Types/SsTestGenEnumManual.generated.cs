#nullable enable
using SharpScript.Subclassing;
using UnrealEngine.Intrinsic;

namespace SharpScriptUnitTest.Types;

public static class ESsTestGenEnumManualNativeRef
{
	public static readonly IntPtr NativeType;

	static ESsTestGenEnumManualNativeRef()
	{
		EnumValueDef[] _valueDefs =
		[
			new()
			{
				Name = "One",
				Value = (long)ESsTestGenEnumManual.One,
			},
			new()
			{
				Name = "Two",
				Value = (long)ESsTestGenEnumManual.Two,
			},
			new()
			{
				Name = "Three",
				Value = (long)ESsTestGenEnumManual.Three,
			},
			new()
			{
				Name = "Four",
				Value = (long)ESsTestGenEnumManual.Four,
			},
		];

		unsafe
		{
			fixed (EnumValueDef* _valueDefsPtr = _valueDefs)
			{
				NativeType = SubclassingUtils.GenerateEnum(
					"SsTestGenEnumManual",
					(IntPtr)_valueDefsPtr, _valueDefs.Length, 1);
			}
		}
	}
}
