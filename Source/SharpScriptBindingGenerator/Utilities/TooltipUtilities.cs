using System;
using System.Collections.Generic;
using System.Linq;
using EpicGames.UHT.Types;

namespace SharpScriptBindingGenerator.Utilities;

public class ParsedTooltip
{
	public class TokenString
	{
		public string SimpleValue = string.Empty;
		public string ComplexValue = string.Empty;
		public string Value => !string.IsNullOrEmpty(SimpleValue) ? SimpleValue : ComplexValue;

		public bool Equals(TokenString other)
		{
			return Value == other.Value;
		}

		public bool NotEquals(TokenString other)
		{
			return Value != other.Value;
		}

		public void SetValue(string value)
		{
			SimpleValue = value;
			ComplexValue = string.Empty;
		}

		public void SetValue(ref string value)
		{
			SimpleValue = string.Empty;
			ComplexValue = value;
		}
	}

	public class MiscToken
	{
		public TokenString TokenName = new();
		public TokenString TokenValue = new();
	}

	public class ParamToken
	{
		public TokenString ParamName = new();
		public TokenString ParamType = new();
		public TokenString ParamComment = new();
	}

	public string BasicTooltipText = string.Empty;
	public readonly List<MiscToken> MiscTokens = new(4);
	public readonly List<ParamToken> ParamTokens = new(8);
	public readonly ParamToken ReturnToken = new();
}

public static class TooltipUtilities
{
	private static void ParseTooltip(string inTooltip, ref ParsedTooltip outParsedTooltip)
	{
		string sourceTooltip = inTooltip;
		int sourceTooltipParseIndex = 0;

		void SkipToNextToken()
		{
			while (sourceTooltipParseIndex < sourceTooltip.Length &&
					(char.IsWhiteSpace(sourceTooltip[sourceTooltipParseIndex]) ||
					sourceTooltip[sourceTooltipParseIndex] == '-'))
			{
				++sourceTooltipParseIndex;
			}
		}

		var tokenStartIndex = sourceTooltipParseIndex;

		void ParseSimpleToken(ref ParsedTooltip.TokenString outToken)
		{
			while (sourceTooltipParseIndex < sourceTooltip.Length && !char.IsWhiteSpace(sourceTooltip[sourceTooltipParseIndex]))
			{
				++sourceTooltipParseIndex;
			}

			outToken.SimpleValue = sourceTooltip.Substring(tokenStartIndex, sourceTooltipParseIndex - tokenStartIndex);
		}

		void ParseComplexToken(ref ParsedTooltip.TokenString outToken)
		{
			int startIndex = sourceTooltipParseIndex;
			while (sourceTooltipParseIndex < sourceTooltip.Length && sourceTooltip[sourceTooltipParseIndex] != '@')
			{
				if (char.IsWhiteSpace(sourceTooltip[sourceTooltipParseIndex]))
				{
					if (startIndex != -1)
					{
						outToken.ComplexValue = sourceTooltip.Substring(startIndex, sourceTooltipParseIndex - startIndex);
						startIndex = -1;
					}

					while (sourceTooltipParseIndex < sourceTooltip.Length && char.IsWhiteSpace(sourceTooltip[sourceTooltipParseIndex]))
					{
						++sourceTooltipParseIndex;
					}

					while (sourceTooltipParseIndex < sourceTooltip.Length && char.IsWhiteSpace(sourceTooltip[sourceTooltipParseIndex]))
					{
						++sourceTooltipParseIndex;
					}

					outToken.ComplexValue += ' ';
				}

				if (sourceTooltipParseIndex < sourceTooltip.Length && sourceTooltip[sourceTooltipParseIndex] != '@')
				{
					if (startIndex == -1)
					{
						outToken.ComplexValue += sourceTooltip[sourceTooltipParseIndex];
					}

					++sourceTooltipParseIndex;
				}
			}

			if (startIndex == -1)
			{
				outToken.ComplexValue = outToken.ComplexValue.TrimEnd();
			}
			else
			{
				outToken.SimpleValue =
					sourceTooltip.Substring(startIndex, sourceTooltipParseIndex - startIndex);
				outToken.SimpleValue = outToken.SimpleValue.TrimEnd();
			}
		}

		while (sourceTooltipParseIndex < sourceTooltip.Length)
		{
			if (sourceTooltip[sourceTooltipParseIndex] == '@')
			{
				++sourceTooltipParseIndex;
				if (sourceTooltipParseIndex < sourceTooltip.Length && sourceTooltip[sourceTooltipParseIndex] == '@')
				{
					outParsedTooltip.BasicTooltipText += '@';
					++sourceTooltipParseIndex;
					continue;
				}

				var tokenName = new ParsedTooltip.TokenString();
				SkipToNextToken();
				ParseSimpleToken(ref tokenName);

				if (tokenName.Value == "param")
				{
					var paramToken = new ParsedTooltip.ParamToken();

					SkipToNextToken();
					ParseSimpleToken(ref paramToken.ParamName);

					SkipToNextToken();
					ParseComplexToken(ref paramToken.ParamComment);

					outParsedTooltip.ParamTokens.Add(paramToken);
				}
				else if (tokenName.Value is "return" or "returns")
				{
					SkipToNextToken();
					ParseComplexToken(ref outParsedTooltip.ReturnToken.ParamComment);
				}
				else
				{
					var miscToken = new ParsedTooltip.MiscToken
					{
						TokenName = tokenName
					};

					SkipToNextToken();
					ParseComplexToken(ref miscToken.TokenValue);

					outParsedTooltip.MiscTokens.Add(miscToken);
				}
			}
			else
			{
				outParsedTooltip.BasicTooltipText += sourceTooltip[sourceTooltipParseIndex++];
			}
		}
	}

	public static void AppendTooltip(this CodeBuilder builder, string toolTip)
	{
		if (string.IsNullOrEmpty(toolTip))
		{
			return;
		}

		ParsedTooltip parsedTooltip = new();
		ParseTooltip(toolTip, ref parsedTooltip);

		if (!string.IsNullOrEmpty(parsedTooltip.BasicTooltipText))
		{
			string[] lines = parsedTooltip.BasicTooltipText.Split(new[] { '\n' }, StringSplitOptions.None);

			builder.AppendLine("/// <summary>");
			foreach (var line in lines)
			{
				builder.AppendLine($"/// {line}");
			}

			builder.AppendLine("/// </summary>");
		}

		if (parsedTooltip.ParamTokens.Any())
		{
			foreach (var miscToken in parsedTooltip.ParamTokens)
			{
				builder.AppendLine($"/// <param name=\"{miscToken.ParamName.Value}\">{miscToken.ParamComment.Value}</param>");
			}
		}

		var returnToolTip = parsedTooltip.ReturnToken.ParamComment.Value;
		if (string.IsNullOrEmpty(returnToolTip))
		{
			return;
		}

		builder.AppendLine("/// <returns>");
		builder.Append(returnToolTip);
		builder.Append("</returns>");
	}

	public static void AppendTooltip(this CodeBuilder builder, UhtType type)
	{
		string toolTip = type.GetToolTipText();

		// Skip if the tooltip is just the same as the source name. UHT automatically generates these.
		if (toolTip == type.SourceName)
		{
			return;
		}

		AppendTooltip(builder, toolTip);
	}
}
