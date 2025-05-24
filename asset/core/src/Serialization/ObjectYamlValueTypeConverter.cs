using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlPrompt.Core.Serialization;

public class ObjectYamlValueTypeConverter : IYamlTypeConverter
{
	public bool Accepts(Type type)
	{
		return type == typeof(object);
	}

	public object? ReadYaml(IParser parser, Type type)
	{
		object? value;
		if (parser.Current is Scalar scalar)
		{
			value = scalar.IsKey ? scalar.Value : ParseScalar(scalar);
		}
		else
		{
			throw new InvalidOperationException(parser.Current?.ToString());
		}

		parser.MoveNext();
		return value;
	}

	private static object? ParseScalar(Scalar scalar)
	{
		if (scalar.Value == null)
			return null;

		if (scalar.IsQuotedImplicit)
			return scalar.Value;

		if (string.IsNullOrEmpty(scalar.Value))
			return null;

		if (bool.TryParse(scalar.Value, out var booleanValue))
			return booleanValue;

		if (int.TryParse(scalar.Value,
			NumberStyles.Integer,
			CultureInfo.InvariantCulture,
			out var intValue))
			return intValue;

		if (double.TryParse(scalar.Value,
			NumberStyles.Float | NumberStyles.AllowThousands,
			CultureInfo.InvariantCulture,
			out var doubleValue))
			return doubleValue;

		if (DateTime.TryParse(scalar.Value,
			CultureInfo.InvariantCulture,
			DateTimeStyles.AdjustToUniversal,
			out var dateTimeValue))
			return dateTimeValue;
		
		if (DateTimeOffset.TryParse(scalar.Value,
			CultureInfo.InvariantCulture,
			DateTimeStyles.AdjustToUniversal,
			out var dateTimeOffsetValue))
			return dateTimeOffsetValue;
			
		if (TimeSpan.TryParse(scalar.Value, out var timeSpanValue))
			return timeSpanValue;
			
		if (scalar.Tag == "!!guid" && Guid.TryParse(scalar.Value, out var guidValue))
    		return guidValue;
		
		return scalar.Value;
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type)
	{
		if (value == null)
		{
			emitter.Emit(new Scalar("tag:yaml.org,2002:null", ""));
			return;
		}

		if (value is object[] array)
		{
			emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Block));
			foreach (var element in array)
			{
				WriteYaml(emitter, element, type);
			}

			emitter.Emit(new SequenceEnd());
			return;
		}

		switch (value)
		{
			case null:
				emitter.Emit(new Scalar("tag:yaml.org,2002:null", ""));
				break;
			case byte byteValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					byteValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case sbyte sbyteValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					sbyteValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case short shortValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					shortValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case ushort ushortValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					ushortValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case int intValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					intValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case uint uintValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					uintValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case long longValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					longValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case ulong ulongValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:int",
					ulongValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case float floatValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:float",
					floatValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case double doubleValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:float",
					doubleValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case decimal decimalValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:float",
					decimalValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case DateTime dateTimeValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:timestamp",
					dateTimeValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case DateTimeOffset dateTimeOffsetValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:timestamp",
					dateTimeOffsetValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case TimeSpan timeSpanValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:timestamp",
					timeSpanValue.ToString(), ScalarStyle.Any, true, false));
				break;
			case Guid guidValue:
				emitter.Emit(new Scalar("!!guid", guidValue.ToString()));
				break;
			case bool booleanValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:bool",
					booleanValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
				break;
			case char charValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:str",
					charValue.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, false, true));
				break;
			case string stringValue:
				emitter.Emit(new Scalar(null, "tag:yaml.org,2002:str", stringValue, ScalarStyle.Any, false, true));
				break;
			default:
				throw new InvalidOperationException(value.ToString());
		}
	}
}