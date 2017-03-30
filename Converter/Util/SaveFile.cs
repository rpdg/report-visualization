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
using NReco.ImageGenerator;

namespace Converter.Util
{
	/// <summary>
	/// Description of SaveHTML.
	/// </summary>
	public static class SaveFile
	{
		public static string ToHTML(string json, string fileName)
		{
			string outFile = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase)) + ".html";
			// Write the string to a file.
			StreamWriter fileStream = new System.IO.StreamWriter(outFile);
			
			using (fileStream) {
				string dir = System.Environment.CurrentDirectory;
				
				string h1 = File.ReadAllText(dir + "\\HTML\\head.html");
				string h2 = File.ReadAllText(dir + "\\HTML\\foot.html");
				
				fileStream.WriteLine("<title>" + outFile + "</title>");
				fileStream.WriteLine(h1);
				fileStream.WriteLine(json);
				fileStream.WriteLine(h2);
				
				fileStream.Close();
			}
			
			return outFile;
			
		}
		
		public static string ToPng(string json, string fileName)
		{
			string outFile = fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.CurrentCultureIgnoreCase)) + ".png";
			
			string dir = System.Environment.CurrentDirectory;
			
			string h1 = File.ReadAllText(dir + "\\HTML\\head.html");
			string h2 = File.ReadAllText(dir + "\\HTML\\foot.html");
			
			
			var imageCons = new HtmlToImageConverter();
			var bytes = imageCons.GenerateImage(h1 + json + h2, ImageFormat.Png);
			

			using (StreamWriter sw = new System.IO.StreamWriter(outFile)) {
				sw.BaseStream.Write(bytes, 0, bytes.Length - 1);
				sw.Close();
			}
			
			return outFile;
		}
	}

}
