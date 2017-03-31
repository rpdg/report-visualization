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
using System.Text;
using System.Collections.Generic;
using Converter.Model;
using System.Dynamic;
using System.Linq;

namespace Converter.Util
{
	class LightCellsDataHandlerVisitCells : LightCellsDataHandler
	{
		private int cellCount;
		private int formulaCount;
		private int stringCount;

		internal LightCellsDataHandlerVisitCells()
		{
			cellCount = 0;
			formulaCount = 0;
			stringCount = 0;
		}

		public int CellCount {
			get { return cellCount; }
		}

		public int FormulaCount {
			get { return formulaCount; }
		}

		public int StringCount {
			get { return stringCount; }
		}

		public bool StartSheet(Worksheet sheet)
		{
			Console.WriteLine("Processing sheet[" + sheet.Name + "]");
			return true;
		}

		public bool StartRow(int rowIndex)
		{
			return (rowIndex < 50000);
			//return true;
		}

		public bool ProcessRow(Row row)
		{
			var cell = row.FirstCell;
			Console.WriteLine(cell);
			return true;
		}

		public bool StartCell(int column)
		{
			return true;
		}

		public bool ProcessCell(Cell cell)
		{
//			cellCount++;
//			if (cell.IsFormula) {
//				formulaCount++;
//			} else if (cell.Type == CellValueType.IsString) {
//				stringCount++;
//			}
			return true;
		}
	}
	
	
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public static class Parser
	{
		public static string Excel2Json(string fileFullName)
		{
			
			LoadOptions opts = new LoadOptions();
			opts.MemorySetting = MemorySetting.MemoryPreference;
			//opts.LoadDataAndFormatting = true;
			//opts.LoadDataOnly = true;
			//opts.IgnoreNotPrinted = true;
			opts.LightCellsDataHandler = new LightCellsDataHandlerVisitCells();
			
			Workbook workbook = new Workbook(fileFullName , opts);
			
			
			var dic = new Dictionary<String, List<DailyReport>>();
			
			//for (int i = 0; i < workbook.Worksheets.Count; i++) {
			//Worksheet worksheet = workbook.Worksheets[i];
			
			foreach (Worksheet worksheet in workbook.Worksheets) {
				
				string sheetName = worksheet.Name;
				
				if (sheetName != "分析") {
					DataTable tb = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1, true);
					List<DailyReport> drps = DataTable2DailyReport(tb, sheetName);
					dic.Add(worksheet.Name, drps);
				}
				
			}
			
			return JsonHelper.Serialize(dic);
		}
		
		public static string Excel2Json(Stream stream)
		{
			using (stream) {
				LoadOptions opts = new LoadOptions();
				opts.MemorySetting = MemorySetting.MemoryPreference;
				//opts.LightCellsDataHandler = new LightCellsDataHandlerVisitCells();
				
				Workbook workbook = new Workbook(stream, opts);
				stream.Close();
				
				var ds = new DataSet();
				var dic = new Dictionary<String, List<DailyReport>>();
				
				for (int i = 0; i < workbook.Worksheets.Count; i++) {
					Worksheet worksheet = workbook.Worksheets[i];
					//foreach (Worksheet worksheet in workbook.Worksheets) {
					
					string sheetName = worksheet.Name;
					
					if (sheetName != "分析") {
						DataTable tb = worksheet.Cells.ExportDataTable(0, 0, worksheet.Cells.MaxRow + 1, worksheet.Cells.MaxColumn + 1, true);
						//ds.Tables.Add(tb);
						List<DailyReport> drps = DataTable2DailyReport(tb, sheetName);
						dic.Add(worksheet.Name, drps);
					}
					
				}

				
				return JsonHelper.Serialize(dic);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tbSrc"></param>
		/// <param name="tbName"></param>
		/// <returns></returns>
		public static List<DailyReport> DataTable2DailyReport(DataTable tbSrc, string tbName)
		{
			/*var rows = tb.AsEnumerable();
			
			var query = from q in rows
				group q by q.Field<DateTime>("BEGIN_TIME").ToString("yyyy/MM/dd") into r
				select new DailyReport{
					ProgramCount = r.Count() ,
					AverageProgramTimeLength = r.Average( e => e.Field<object>(2).TryToDecimal() ) ,
				};
			
			return query.ToList();*/
			
			DataView dv = tbSrc.DefaultView;
			dv.Sort = "BEGIN_TIME Asc";
			DataTable tb = dv.ToTable();
			
			var rptList = new List<DailyReport>();
			
			var dic = new Dictionary<string , List<ProduceItem>>();
			
			for (int j = 0; j < tb.Rows.Count; j++) {
				
				DataRow row = tb.Rows[j];
				
				
				var item = new ProduceItem {
					ProgramLength = row[2].TryToDecimal(),
					TaskDuration = row[6].TryToDecimal(),
				};
				
				var beginStr = row[4].TrimToString();
				
				if (!string.IsNullOrEmpty(beginStr)) {
					
					try {
						item.Begin = DateTime.Parse(beginStr);
						
						
						var endStr = row[5].TrimToString();
						if (!string.IsNullOrEmpty(endStr))
							item.End = DateTime.Parse(endStr);
						
						
						string key = item.Begin.ToString("yyyy-MM-dd");
						
						if (!dic.ContainsKey(key)) {
							dic.Add(key, new List<ProduceItem>());
						}
						
						dic[key].Add(item);
					} catch (Exception e) {
						throw new Exception(tbName + ", 第" + j + "行发生错误");
					}
				}
				
				
				
			}
			
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
					//平均时长
					rpt.AverageProgramTimeLength = Math.Round(totalLength / finished, 1);
					
					//平均耗时
					rpt.AverageTaskDuration = Math.Round(totalTaskDuration / finished, 1);
					
					//完成率(百分数)
					rpt.AccomplishmentRatio = Math.Round(finished / progCount * 100, 2);
					
					//效率
					//excel统计中，会将几秒钟的任务认为是0分钟，所以会出现执行时长为0的情况
					if (totalTaskDuration > 0)
						rpt.Efficiency = Math.Round(totalLength / totalTaskDuration, 2);
				}
				
				rptList.Add(rpt);
			}
			
			
			return rptList;
		}
		
	}
}
