using GameSparks.Core;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class GameSparksJsonSchema
	{
		private struct FieldValidation
		{
			public enum Mode
			{
				Type,
				ObjectSchema,
				ListSchema
			}

			public Mode FieldMode;

			public Type FieldType;

			public GameSparksJsonSchema FieldSchema;
		}

		private Dictionary<string, FieldValidation> _schema = new Dictionary<string, FieldValidation>();

		public GameSparksJsonSchema Field<T>(string path)
		{
			_schema[path] = new FieldValidation
			{
				FieldMode = FieldValidation.Mode.Type,
				FieldType = typeof(T),
				FieldSchema = null
			};
			return this;
		}

		public GameSparksJsonSchema Field(string path, GameSparksJsonSchema fieldSchema)
		{
			_schema[path] = new FieldValidation
			{
				FieldMode = FieldValidation.Mode.ObjectSchema,
				FieldType = typeof(GSData),
				FieldSchema = fieldSchema
			};
			return this;
		}

		public GameSparksJsonSchema List(string path, GameSparksJsonSchema itemSchema)
		{
			_schema[path] = new FieldValidation
			{
				FieldMode = FieldValidation.Mode.ListSchema,
				FieldType = typeof(List<GSData>),
				FieldSchema = itemSchema
			};
			return this;
		}

		public GameSparksJsonSchema List<T>(string path)
		{
			_schema[path] = new FieldValidation
			{
				FieldMode = FieldValidation.Mode.Type,
				FieldType = typeof(List<T>),
				FieldSchema = null
			};
			return this;
		}

		public string Validate(GSData data)
		{
			if (data == null)
			{
				return "data is null";
			}
			foreach (KeyValuePair<string, FieldValidation> item in _schema)
			{
				string text = CheckField(item.Key, item.Value, data);
				if (!string.IsNullOrEmpty(text))
				{
					return $"{{{text}}}";
				}
			}
			return "";
		}

		private string CheckField(string path, FieldValidation fieldType, GSData data)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "empty path";
			}
			string[] array = path.Split(new char[1]
			{
				'.'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 1)
			{
				if (!HasValue(path, fieldType.FieldType, data))
				{
					return $"{path}: not of type '{fieldType.FieldType}'";
				}
				if (fieldType.FieldMode == FieldValidation.Mode.ObjectSchema)
				{
					GSData gSData = data.GetGSData(path);
					string text = fieldType.FieldSchema.Validate(gSData);
					if (!string.IsNullOrEmpty(text))
					{
						return $"{path}: {{{text}}}";
					}
				}
				else if (fieldType.FieldMode == FieldValidation.Mode.ListSchema)
				{
					List<GSData> gSDataList = data.GetGSDataList(path);
					int i = 0;
					for (int count = gSDataList.Count; i < count; i++)
					{
						GSData data2 = gSDataList[i];
						string text2 = fieldType.FieldSchema.Validate(data2);
						if (!string.IsNullOrEmpty(text2))
						{
							return $"{path}[{i}]: {text2}";
						}
					}
				}
				return "";
			}
			string text3 = array[0];
			GSData gSData2 = data.GetGSData(text3);
			if (gSData2 == null)
			{
				return $"{text3}: not an object";
			}
			path = path.Substring(text3.Length + 1);
			string text4 = CheckField(path, fieldType, gSData2);
			if (!string.IsNullOrEmpty(text4))
			{
				return $"{text3}: {{{text4}}}";
			}
			return "";
		}

		private bool HasValue(string key, Type fieldType, GSData data)
		{
			if (fieldType == typeof(bool))
			{
				return data.GetBoolean(key).HasValue;
			}
			if (fieldType == typeof(DateTime))
			{
				return data.GetDate(key).HasValue;
			}
			if (fieldType == typeof(double))
			{
				return data.GetDouble(key).HasValue;
			}
			if (fieldType == typeof(List<double>))
			{
				return data.GetDoubleList(key) != null;
			}
			if (fieldType == typeof(float))
			{
				return data.GetFloat(key).HasValue;
			}
			if (fieldType == typeof(List<float>))
			{
				return data.GetFloatList(key) != null;
			}
			if (fieldType == typeof(int))
			{
				return data.GetInt(key).HasValue;
			}
			if (fieldType == typeof(List<int>))
			{
				return data.GetIntList(key) != null;
			}
			if (fieldType == typeof(long))
			{
				return data.GetLong(key).HasValue;
			}
			if (fieldType == typeof(List<long>))
			{
				return data.GetLongList(key) != null;
			}
			if (fieldType == typeof(List<string>))
			{
				return data.GetStringList(key) != null;
			}
			if (fieldType == typeof(string))
			{
				return data.GetString(key) != null;
			}
			if (fieldType == typeof(GSData))
			{
				return data.GetGSData(key) != null;
			}
			if (fieldType == typeof(List<GSData>))
			{
				return data.GetGSDataList(key) != null;
			}
			throw new ArgumentException($"Cannot get a value of type {fieldType} from a GSData instance: this is an unsupported type.");
		}
	}
}
