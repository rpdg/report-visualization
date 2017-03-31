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
		
		BackgroundWorker m_oWorker;
		
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			//Control.CheckForIllegalCrossThreadCalls = false;
			
			
			
			m_oWorker = new BackgroundWorker();
			
			// Create a background worker thread that ReportsProgress &
			// SupportsCancellation
			// Hook up the appropriate events.
			m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
			//m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
			m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
			//m_oWorker.WorkerReportsProgress = true;
			//m_oWorker.WorkerSupportsCancellation = true;
		}
		
		void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			sheetJson = Parser.Excel2Json(sourceFileName);
			//Console.Write(sheetJson);
		}
		
		void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			AppendTxtMethod(safeFileName + " 加载完成.");
			button1.Enabled = true;
			button2.Enabled = true;
			button3.Enabled = true;
		}
		
		
		void UpdateProgress(){
			AppendTxtMethod("开始载入文件: " + safeFileName);
		}
		
		void DeleteLineMethod(int a_line)
		{
			int start_index = richTextBox1.GetFirstCharIndexFromLine(a_line);
			if(start_index >-1){
				int count = richTextBox1.Lines[a_line].Length;

				// Eat new line chars
				if (a_line < richTextBox1.Lines.Length - 1)
				{
					count += richTextBox1.GetFirstCharIndexFromLine(a_line + 1) -
						((start_index + count - 1) + 1);
				}

				richTextBox1.Text = richTextBox1.Text.Remove(start_index, count);
				
				Console.Write(richTextBox1.Text);
			}
		}
		
		void AppendTxtMethod(string msg)
		{
			richTextBox1.AppendText(msg + "\r\n");
			richTextBox1.ScrollToCaret();
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
				
				AppendTxtMethod("开始载入文件: " + safeFileName);
				
				string directoryPath = Path.GetDirectoryName(sourceFileName);
				openFileDialog1.InitialDirectory = directoryPath;
				
				textBox1.Text = sourceFileName;
				button1.Enabled = false;
				button2.Enabled = false;
				button3.Enabled = false;
				
				m_oWorker.RunWorkerAsync();
				
				/*Stream stream;
				try {
					if ((stream = openFileDialog1.OpenFile()) != null) {
						using (stream) {
							sheetJson = Parser.Excel2Json(stream);
							stream.Close();
						}

					}
				} catch (Exception ex) {
					//MessageBox.Show("Error:  " + ex.Message);
					UpdateTxtMethod("Error:  " + ex.Message);
				}*/
			}
		}
		
		
		
		
		void Button2Click(object sender, EventArgs e)
		{
			
			string fileName = SaveFile.ToHTML(sheetJson, sourceFileName);
			AppendTxtMethod(fileName + " 已保存");
			
		}
		
		
		void Button3Click(object sender, EventArgs e)
		{
			string fileName = SaveFile.ToPng(sheetJson, sourceFileName);
			AppendTxtMethod(fileName + " 已保存");
		}
	}
}
