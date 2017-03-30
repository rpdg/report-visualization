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
		}
		void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			UpdateTxtMethod("excel 文档加载完成.");
			button2.Enabled = true;
			button3.Enabled = true;
		}
		
		
		
		public void UpdateTxtMethod(string msg)
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
				
				richTextBox1.Text = "开始载入文件";
				sourceFileName = openFileDialog1.FileName;
				
				string directoryPath = Path.GetDirectoryName(sourceFileName);
				openFileDialog1.InitialDirectory = directoryPath;
				
				textBox1.Text = sourceFileName;
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
			UpdateTxtMethod(fileName + " 已保存");
			
		}
		
		
		void Button3Click(object sender, EventArgs e)
		{
			string fileName = SaveFile.ToPng(sheetJson, sourceFileName);
			UpdateTxtMethod(fileName + " 已保存");
		}
	}
}
