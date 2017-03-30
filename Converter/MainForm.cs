/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/27
 * Time: 14:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data;
using Converter.Util;

using System.Diagnostics;  
using System.Runtime.InteropServices;  
using System.Threading;  

namespace Converter
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//Control.CheckForIllegalCrossThreadCalls = false;  
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		//Thread drawThread = null;              
        //delegate void drawDelegate(int i); 
        
        
		void Button1Click(object sender, EventArgs e)
		{
			richTextBox1.Text = "";
			Stream myStream = null;
			
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			//openFileDialog1.InitialDirectory = "c:\\";
			openFileDialog1.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = false;

			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				string sourceFileName = openFileDialog1.FileName;
				string directoryPath = Path.GetDirectoryName(sourceFileName);
				openFileDialog1.InitialDirectory = directoryPath ;
				
				textBox1.Text = sourceFileName;
				
				try {
					if ((myStream = openFileDialog1.OpenFile()) != null) {
						
						string sheetsJson = Parser.Excel2Json(myStream);
						richTextBox1.Text = sheetsJson;
						SaveFile.IntoHTML(sheetsJson , sourceFileName);
					}
				} catch (Exception ex) {
					//MessageBox.Show("Error:  " + ex.Message);
					richTextBox1.Text = "Error:  " + ex.Message;
				}
			}
		}
		void Label1Click(object sender, EventArgs e)
		{
	
		}
	}
}
