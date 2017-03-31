/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/31
 * Time: 15:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Converter.Util
{
	/// <summary>
	/// Description of Logger.
	/// </summary>
	public static class Logger
	{
		static RichTextBox richTextBox;
		
		public static void Init(RichTextBox richTextBox1)
		{
			richTextBox = richTextBox1;
		}
		
		/// <summary>
		/// Thread safe richtextbox event logging method
		/// </summary>
		/// <param name="eventText"></param>
		/// <param name="textColor"></param>
		public static void Write(string eventText, Color textColor)
		{
			if (richTextBox.InvokeRequired) {
				richTextBox.BeginInvoke(new Action(delegate {
					Write( eventText, textColor);
				}));
				return;
			}

			string nDateTime = DateTime.Now.ToString("[HH:mm:ss]") + " - ";

			// color text.
			richTextBox.SelectionStart = richTextBox.Text.Length;
			richTextBox.SelectionColor = textColor;

			// newline if first line, append if else.
			if (richTextBox.Lines.Length == 0) {
				richTextBox.AppendText(nDateTime + eventText);
				richTextBox.ScrollToCaret();
				richTextBox.AppendText(System.Environment.NewLine);
			} else {
				richTextBox.AppendText(nDateTime + eventText + System.Environment.NewLine);
				richTextBox.ScrollToCaret();
			}
		}
		
		
		
		public static void DeleteLineMethod(RichTextBox richTextBox, int lineIndex)
		{
			int start_index = richTextBox.GetFirstCharIndexFromLine(lineIndex);
			if (start_index > -1) {
				int count = richTextBox.Lines[lineIndex].Length;

				// Eat new line chars
				if (lineIndex < richTextBox.Lines.Length - 1) {
					count += richTextBox.GetFirstCharIndexFromLine(lineIndex + 1) -
					((start_index + count - 1) + 1);
				}

				richTextBox.Text = richTextBox.Text.Remove(start_index, count);
				
				Console.Write(richTextBox.Text);
			}
		}
	}
}
