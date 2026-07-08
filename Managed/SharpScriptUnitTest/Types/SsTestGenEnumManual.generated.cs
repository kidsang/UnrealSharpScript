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
				MetaDataEntry[] _metaEntries =
				[
				];

				fixed (MetaDataEntry* _metaEntriesPtr = _metaEntries)
				{
					EnumDef _enumDef = new()
					{
						EnumName = "SsTestGenEnumManual",
						ValueDefines = (IntPtr)_valueDefsPtr,
						ValueCount = _valueDefs.Length,
						IsFlags = 1,
						Specifiers = (ulong)(EnumSpecs.BlueprintType),
						MetaEntries = (IntPtr)_metaEntriesPtr,
						MetaCount = _metaEntries.Length,
					};
					NativeType = SubclassingUtils.GenerateEnum((IntPtr)(&_enumDef));
				}
			}
		}
	}
}
