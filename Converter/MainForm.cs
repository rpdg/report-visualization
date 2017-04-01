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
using System.ComponentModel;

namespace Converter
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		string sheetJson;
		string sourceFileName;
		string safeFileName;
		
		BackgroundWorker parseWorker;
		
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//Control.CheckForIllegalCrossThreadCalls = false;
			
			Logger.Init(richTextBox1);
			
			parseWorker = new BackgroundWorker();
			
			// Create a background worker thread that ReportsProgress &
			// SupportsCancellation
			// Hook up the appropriate events.
			parseWorker.DoWork += new DoWorkEventHandler(StartParse);
			//m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
			parseWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FinishParse);
			//m_oWorker.WorkerReportsProgress = true;
			//m_oWorker.WorkerSupportsCancellation = true;
		}
		
		void StartParse(object sender, DoWorkEventArgs e)
		{
			sheetJson = Parser.Excel2Json(sourceFileName);
			//Console.Write(sheetJson);
		}
		
		void FinishParse(object sender, RunWorkerCompletedEventArgs e)
		{
			Logger.Write(safeFileName + " 处理完成，可选择转为 HTML 或 JPEG" , Color.Black);
			button1.Enabled = true;
			button2.Enabled = true;
			button3.Enabled = true;
			
			
			if(checkBox2.Checked){
				GenerateImage();
			}
		}
		
		
		
		void Button1Click(object sender, EventArgs e)
		{
			
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			//openFileDialog1.InitialDirectory = "c:\\";
			openFileDialog1.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.RestoreDirectory = false;

			if (openFileDialog1.ShowDialog() == DialogResult.OK) {
				
				safeFileName = openFileDialog1.SafeFileName ;
				sourceFileName = openFileDialog1.FileName;
				
				Logger.Write("载入文件: " + safeFileName , Color.Brown);
				
				string directoryPath = Path.GetDirectoryName(sourceFileName);
				openFileDialog1.InitialDirectory = directoryPath;
				
				textBox1.Text = sourceFileName;
				button1.Enabled = false;
				button2.Enabled = false;
				button3.Enabled = false;
				
				parseWorker.RunWorkerAsync();
				
			}
		}
		
		
		void GenerateImage()
		{
			string fileName = SaveFile.ToPng(sheetJson, sourceFileName);
			Logger.Write(fileName + " 已保存" , Color.Green);
		}
		
		
		void Button2Click(object sender, EventArgs e)
		{
			
			string fileName = SaveFile.ToHTML(sheetJson, sourceFileName);
			Logger.Write( fileName + " 已保存" , Color.Green);
			
		}
		
		
		void Button3Click(object sender, EventArgs e)
		{
			GenerateImage();
		}
	}
}
