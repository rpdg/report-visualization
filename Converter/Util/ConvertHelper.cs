/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/28
 * Time: 12:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;

namespace Converter.Util
{
	/// <summary>
	/// Description of Convert.
	/// </summary>
	public static class ConvertHelper
	{
		public static decimal TryToDecimal(this object obj, decimal def = 0m)
		{
			if (obj == null || Convert.IsDBNull(obj)) {
				return def;
			}
			decimal val = def;
			var b = decimal.TryParse(obj.ToString(), out val);
			return val;
		}
        
        
		public static int TryToInt(this object obj, int def = 0)
		{
			decimal val = obj.TryToDecimal((decimal)def);
			return (int)val;
		}
        
		public static long TryToLong(this object obj, long def = 0L)
		{
			decimal val = obj.TryToDecimal((decimal)def);
			return (long)val;
		}



		/// <summary>
		/// 进行TOSTRING后清空左右空格 【扩展方法】
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="defaultVal"></param>
		/// <returns></returns>
		public static string TrimToString(this object obj, string defaultVal = "")
		{
			if (obj == null || Convert.IsDBNull(obj)) {
				return defaultVal;
			}
			return obj.ToString().Trim();
		}
		
		public static string TrimToString<T>(this T str, string defaultVal = "")
		{
			if (str == null || Convert.IsDBNull(str)) {
				return defaultVal;
			}
			return str.ToString().Trim();
		}

	}
}
