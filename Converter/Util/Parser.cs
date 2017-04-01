/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/27
 * Time: 15:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using Aspose.Cells;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using Converter.Model;
using System.Dynamic;
using System.Linq;
using Converter.Util;


namespace Converter.Util
{
	class LightCellsDataHandlerVisitCells : LightCellsDataHandler
	{

		bool readNewRow;
		bool firstCellHasValue;
		
		internal LightCellsDataHandlerVisitCells()
		{
			//readNewRow = true;
			//firstCellHasValue = true;
		}


		public bool StartSheet(Worksheet sheet)
		{
			if (sheet.Name.Equals("分析", StringComparison.CurrentCultureIgnoreCase))
				return false;
			
			readNewRow = true;
			firstCellHasValue = true;
			Logger.Write("读取表 [" + sheet.Name + "]", Color.Navy);
			return true;
		}

		public bool StartRow(int rowIndex)
		{
			readNewRow = true;
			
			return firstCellHasValue;
		}

		public bool ProcessRow(Row row)
		{
			//var cell = row.GetCellOrNull(1);
			//Console.WriteLine(cell);
			
			if (row.Index > 3) {
				Console.WriteLine(row);
			}
			
			return true;
		}

		public bool StartCell(int column)
		{
			return true;
		}

		public bool ProcessCell(Cell cell)
		{
			if (readNewRow) {
				
				readNewRow = false;
				
				if (cell.Column != 0)
					firstCellHasValue = false;
			}
			
			
			
			return firstCellHasValue;
		}
	}
	
	
	/// <summary>
	/// parse the excel to json
	/// </summary>
	public static class Parser
	{
		
		/// <summary>
		/// parse the excel to json
		/// </summary>
		/// <param name="fileFullName">file name with full path</param>
		/// <returns>json string</returns>
		public static string Excel2Json(string fileFullName)
		{
			
			LoadOptions opts = new LoadOptions();
			opts.MemorySetting = MemorySetting.MemoryPreference;
			//opts.LoadDataAndFormatting = true;
			//opts.LoadDataOnly = true;
			opts.IgnoreNotPrinted = true;
			opts.LightCellsDataHandler = new LightCellsDataHandlerVisitCells();
			
			Workbook workbook = new Workbook(fileFullName, opts);
			
			
			var dic = new Dictionary<String, List<DailyReport>>();
			
			//for (int i = 0; i < workbook.Worksheets.Count; i++) {
			//Worksheet worksheet = workbook.Worksheets[i];
			
			foreach (Worksheet worksheet in workbook.Worksheets) {
				
				if (worksheet.Cells.MaxRow > -1) {
					
					string sheetName = worksheet.Name;
					int processedLines;
					
					DataTable tb = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1, true);
					List<DailyReport> drps = DataTable2DailyReport(tb, sheetName, out processedLines);
					dic.Add(worksheet.Name, drps);
					//tb.Dispose();

					Logger.Write("处理表 [" + worksheet.Name + "]，共" + worksheet.Cells.MaxRow + "行，有效数据" + processedLines + "行", Color.Red);
				}
				
				
			}
			
			return JsonHelper.Serialize(dic);
		}
		
		
		/// <summary>
		/// Convert DataTable to daily report list
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="tbName"></param>
		/// <param name="processedLines"></param>
		/// <returns></returns>
		public static List<DailyReport> DataTable2DailyReport(DataTable tb, string tbName, out int processedLines)
		{
			/*var rows = tb.AsEnumerable();
			
			var query = from q in rows
				group q by q.Field<DateTime>("BEGIN_TIME").ToString("yyyy/MM/dd") into r
				select new DailyReport{
					ProgramCount = r.Count() ,
					AverageProgramTimeLength = r.Average( e => e.Field<object>(2).TryToDecimal() ) ,
				};
			
			return query.ToList();*/
			
			
			/*DataView dv = tbSrc.DefaultView;
			dv.Sort = "BEGIN_TIME Asc";
			DataTable tb = dv.ToTable();*/
			tb.DefaultView.Sort = "BEGIN_TIME Asc";
			tb = tb.DefaultView.ToTable();
			
			processedLines = 0;
			
			var rptList = new List<DailyReport>();
			
			var dic = new Dictionary<string , List<ProduceItem>>();
			
			for (int j = 0; j < tb.Rows.Count; j++) {
				
				DataRow row = tb.Rows[j];
								
				
				var beginStr = row[4].TrimToString();
				
				if (!string.IsNullOrEmpty(beginStr)) {
					
					processedLines++;
					
					var item = new ProduceItem {
						ProgramLength = row[2].TryToDecimal(),
						TaskDuration = row[6].TryToDecimal(),
						Begin = DateTime.Parse(beginStr),
					};
						
						
					var endStr = row[5].TrimToString();
					if (!string.IsNullOrEmpty(endStr))
						item.End = DateTime.Parse(endStr);
						
						
					string key = item.Begin.ToString("yyyy-MM-dd");
						
					if (!dic.ContainsKey(key)) {
						dic.Add(key, new List<ProduceItem>());
					}
						
					dic[key].Add(item);
						
				}
				
				
				
			}
			//tb.Dispose();
			
			foreach (string dateKey in dic.Keys) {
				var list = dic[dateKey];
				
				decimal finished = 0;
				decimal totalLength = 0;
				decimal totalTaskDuration = 0;
				

				int progCount = list.Count;
				for (int i = 0; i < progCount; i++) {
					var item = list[i];
					if (item.End != null) {
						
						finished++;
						
						totalLength += item.ProgramLength;
						
						totalTaskDuration += item.TaskDuration;
					}
				}
				
				
				var rpt = new DailyReport() {
					BeginDate = dateKey,
					ProgramCount = progCount,
					AccomplishedProgramCount = (int)finished,
					TotalProgramTimeLength = totalLength,
					TotalTaskDuration = totalTaskDuration,
				};
				
				if (finished > 0) {
					//平均时长(分)
					//rpt.AverageProgramTimeLength = Math.Round(totalLength / finished, 1);
					
					//平均耗时(分)
					//rpt.AverageTaskDuration = Math.Round(totalTaskDuration / finished, 1);
					
					//完成率(百分数)
					rpt.AccomplishmentRatio = Math.Round(finished / progCount * 100, 2);
					
					//效率
					//excel原始表格中，会将几秒钟的任务认为是0分钟，所以会出现执行时长为0的情况
					if (totalLength>0 && totalTaskDuration > 0)
						rpt.Efficiency = Math.Round(totalLength / totalTaskDuration, 5);
				}
				
				rptList.Add(rpt);
			}
			
			
			return rptList;
		}
		
	}
}
