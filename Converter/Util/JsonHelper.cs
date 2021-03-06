﻿using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Linq;
using System.Data;

namespace Converter.Util
{

	/// <summary>
	/// Json处理类
	/// </summary>
	public static class JsonHelper
	{
		/// <summary>
		/// 序列化数据为Json数据格式.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static string Serialize(object value)
		{
			string type = value.GetType().ToString();
 
			JsonSerializer json = new JsonSerializer();
 
			/**/
			json.NullValueHandling = NullValueHandling.Ignore;
			json.ObjectCreationHandling = ObjectCreationHandling.Replace;
			json.MissingMemberHandling = MissingMemberHandling.Ignore;
			json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
 
			switch (type) {
				case "System.Data.DataRow":
					{
						json.Converters.Add(new DataRowConverter());
						break;
					}
				case "System.Data.DataTable":
					{
						json.Converters.Add(new DataTableConverter());
						break;
					}
				case "System.Data.DataSet":
					{
						json.Converters.Add(new DataSetConverter());
						break;
					}
				case "System.DateTime":
					{
						var dateConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" };
						return JsonConvert.SerializeObject(value, dateConverter);
					}
				default:
					{
						//return JsonConvert.SerializeObject(value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore , DateFormatString="yyyy-MM-dd HH:mm:ss"});
						return JsonConvert.SerializeObject(value, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
					}
			}
			/*if (type == typeof(DataRow))
				json.Converters.Add(new DataRowConverter());
			else if (type == typeof(DataTable))
				json.Converters.Add(new DataTableConverter());
			else if (type == typeof(DataSet))
				json.Converters.Add(new DataSetConverter());
			else if (type == typeof(DateTime)) {
				var dateConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" };
				return JsonConvert.SerializeObject(value, dateConverter);
			} 
			else
				return JsonConvert.SerializeObject(value);*/
 
			using (StringWriter sw = new StringWriter()) {
				Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
				//writer.Formatting = Formatting.Indented;
				writer.Formatting = Formatting.None;
				writer.QuoteChar = '"';
				json.Serialize(writer, value);
 
				string output = sw.ToString();
				writer.Close();
				sw.Close();
 
				return output;
			}
			
		}
 
		/// <summary>
		/// 反序列化Json数据格式.
		/// </summary>
		/// <param name="jsonText">The json text.</param>
		/// <param name="valueType">Type of the value.</param>
		/// <returns></returns>
		public static object Deserialize(string jsonText, Type valueType)
		{
			Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
 
			json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
			json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
			json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
			json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
 
			StringReader sr = new StringReader(jsonText);
			Newtonsoft.Json.JsonTextReader reader = new JsonTextReader(sr);
			object result = json.Deserialize(reader, valueType);
			reader.Close();
 
			return result;
		}
		
	}
 
	/// <summary>
	/// Converts a <see cref="DataRow"/> object to and from JSON.
	/// </summary>
	public class DataRowConverter : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="dataRow">The data row.</param>
		/// <param name="serializer">The JsonSerializer 对象.</param>
		public override void WriteJson(JsonWriter writer, object dataRow, JsonSerializer serializer)
		{
			DataRow row = dataRow as DataRow;
 
			writer.WriteStartObject();
			foreach (DataColumn column in row.Table.Columns) {
				writer.WritePropertyName(column.ColumnName);
				serializer.Serialize(writer, row[column]);
			}
			writer.WriteEndObject();
		}
 
		/// <summary>
		/// Determines whether this instance can convert the specified value type.
		/// </summary>
		/// <param name="objectType">Type of the value.</param>
		/// <returns>
		///     <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return typeof(DataRow).IsAssignableFrom(objectType);
		}
 
		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue"></param>
		/// <param name="serializer"></param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
 
	/// <summary>
	/// Converts a DataTable to JSON. Note no support for deserialization
	/// </summary>
	public class DataTableConverter : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="dataTable">The data table.</param>
		/// <param name="ser">The JsonSerializer Object.</param>
		public override void WriteJson(JsonWriter writer, object dataTable, JsonSerializer ser)
		{
			DataTable table = dataTable as DataTable;
			DataRowConverter converter = new DataRowConverter();
 
			//writer.WriteStartObject();
			//writer.WritePropertyName("data");
 			
			writer.WriteStartArray();
 
			foreach (DataRow row in table.Rows) {
				converter.WriteJson(writer, row, ser);
			}
 
			writer.WriteEndArray();
			
			//writer.WriteEndObject();
		}
 
		/// <summary>
		/// Determines whether this instance can convert the specified value type.
		/// </summary>
		/// <param name="valueType">Type of the value.</param>
		/// <returns>
		///     <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type valueType)
		{
			return typeof(DataTable).IsAssignableFrom(valueType);
		}
 
		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue"></param>
		/// <param name="ser"></param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer ser)
		{
			throw new NotImplementedException();
		}
	}
 
	/// <summary>
	/// Converts a <see cref="DataSet"/> object to JSON. No support for reading.
	/// </summary>
	public class DataSetConverter : JsonConverter
	{
		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
		/// <param name="dataset">The dataset.</param>
		/// <param name="ser">The JsonSerializer Object.</param>
		public override void WriteJson(JsonWriter writer, object dataset, JsonSerializer ser)
		{
			DataSet dataSet = dataset as DataSet;
 
			DataTableConverter converter = new DataTableConverter();
 
			//writer.WriteStartObject();
			//writer.WritePropertyName("data");
 			
			writer.WriteStartArray();
 
			foreach (DataTable table in dataSet.Tables) {
				converter.WriteJson(writer, table, ser);
			}
			writer.WriteEndArray();
			
			//writer.WriteEndObject();
		}
 
		/// <summary>
		/// Determines whether this instance can convert the specified value type.
		/// </summary>
		/// <param name="valueType">Type of the value.</param>
		/// <returns>
		///     <c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type valueType)
		{
			return typeof(DataSet).IsAssignableFrom(valueType);
		}
 
		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue"></param>
		/// <param name="ser"></param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer ser)
		{
			throw new NotImplementedException();
		}
	}

	
	
	

}