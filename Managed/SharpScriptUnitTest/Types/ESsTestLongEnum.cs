namespace SharpScriptUnitTest.Types;

[Flags]
public enum ESsTestLongEnum
{
	One,
	Two = 1 << 0,
	Three = 1 << 1,
	Four = 1 << 2,
}
