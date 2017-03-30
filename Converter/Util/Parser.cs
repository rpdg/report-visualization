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
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public static class Parser
	{
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string Excel2Json(Stream stream)
		{
			using (stream) {
				Workbook workbook = new Workbook(stream);
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
			
			var dic = new Dictionary<string , int>();
			
			
			string previousDateKey = "";
			decimal currentDateStarted = 0;
			decimal currentDateFinished = 0;
			decimal currentDateTotalLength = 0;
			decimal currentDateTotalTaskDuration = 0;
			
			int l = tb.Rows.Count ;
			for (int j = 0; j < l; j++) {
				
				DataRow row = tb.Rows[j];
				
				
				var item = new ProduceItem {
					ProgramLength = row[2].TryToDecimal(),
					TaskDuration = row[6].TryToDecimal(),
				};
				
				var beginDateString = row[4].TrimToString();
				
				if (!string.IsNullOrEmpty(beginDateString)) {
					
					try {
						item.Begin = DateTime.Parse(beginDateString);
					
					
						var endDateString = row[5].TrimToString();
						if (!string.IsNullOrEmpty(endDateString))
							item.End = DateTime.Parse(endDateString);
						
						
						string currentDateKey = item.Begin.ToString("yyyy-MM-dd");
						
						if (previousDateKey != currentDateKey ) {
							
							if (!string.IsNullOrEmpty(previousDateKey)) {
								var dailyReport = new DailyReport() {
									BeginDate = previousDateKey,
									ProgramCount = (int)currentDateStarted,
									AccomplishedProgramCount = (int)currentDateFinished,
									TotalProgramTimeLength = currentDateTotalLength,
									TotalTaskDuration = currentDateTotalTaskDuration,
								};
								if (currentDateFinished > 0) {
									//平均时长
									dailyReport.AverageProgramTimeLength = Math.Round(currentDateTotalLength / currentDateFinished, 1);
					
									//平均耗时
									dailyReport.AverageTaskDuration = Math.Round(currentDateTotalTaskDuration / currentDateFinished, 1);
				
									//完成率(百分数)
									dailyReport.AccomplishmentRatio = Math.Round(currentDateFinished / currentDateStarted * 100, 2);
					
									//效率
									//excel统计中，会将几秒钟的任务认为是0分钟，所以会出现执行时长为0的情况
									if (currentDateTotalTaskDuration > 0)
										dailyReport.Efficiency = Math.Round(currentDateTotalLength / currentDateTotalTaskDuration, 2);
								}
							
								rptList.Add(dailyReport);
							
							}
							
							previousDateKey = currentDateKey;
							currentDateStarted = 0;
							currentDateFinished = 0;
							currentDateTotalLength = 0;
							currentDateTotalTaskDuration = 0;
							
							
							dic.Add(currentDateKey, 1);
						}
						
						currentDateStarted++;
						currentDateTotalLength += item.ProgramLength;
						if (item.End != null) {
							currentDateFinished++;
							currentDateTotalTaskDuration += item.TaskDuration;
						}
						
						
						//dic[currentDateKey].Add(item);
						
					} catch (Exception e) {
						throw new Exception(tbName + ", 第" + j + "行发生错误");
					}
				}
					
				
				
			}
			
			/*
			foreach (string dateKey in dic.Keys) {
				var list = dic[dateKey];
				
				decimal finished = 0;
				decimal totalLength = 0;
				decimal totalTaskDuration = 0;
				

				for (int i = 0; i < list.Count; i++) {
					var item = list[i];
					if (item.End != null) {
					
						finished++;
						
						totalLength += item.Length;
						
						totalTaskDuration += item.TaskDuration;
					}
				}
				
				
				var rpt = new DailyReport() {
					BeginDate = dateKey,
					ProgramCount = list.Count,
					AccomplishedProgramCount = (int)finished,
					TotalProgramTimeLength = totalLength,
					TotalTaskDuration = totalTaskDuration,
				};
				
				
				if (finished > 0) {
					//平均时长
					rpt.AverageProgramTimeLength = Math.Round(totalLength / rpt.AccomplishedProgramCount, 1);
					
					//平均耗时
					rpt.AverageTaskDuration = Math.Round(totalTaskDuration / rpt.AccomplishedProgramCount, 1);
				
					//完成率(百分数)
					rpt.AccomplishmentRatio = Math.Round(finished / rpt.ProgramCount * 100, 2);
					
					//效率
					//excel统计中，会将几秒钟的任务认为是0分钟，所以会出现执行时长为0的情况
					if (totalTaskDuration > 0)
						rpt.Efficiency = Math.Round(totalLength / totalTaskDuration, 2);
				}
				rptList.Add(rpt);
			}
			*/
			
			return rptList;
		}
		
	}
}
