/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/28
 * Time: 16:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;

namespace Converter.Util
{
	/// <summary>
	/// Description of SaveHTML.
	/// </summary>
	public static class Save
	{
		public static void IntoHTML(string json , string fileName)
		{
			string outFile = fileName.Substring(0 , fileName.LastIndexOf("." , StringComparison.CurrentCultureIgnoreCase)) ;
			// Write the string to a file.
			StreamWriter file = new System.IO.StreamWriter(outFile + ".html");
			
			string dir = System.Environment.CurrentDirectory;
			
			string h1 = File.ReadAllText(dir+"\\HTML\\head.html");
			string h2 = File.ReadAllText(dir+"\\HTML\\foot.html");
			file.WriteLine(h1);
			file.WriteLine(json);
			file.WriteLine(h2);
			
			file.Close();
		}
	}
}
