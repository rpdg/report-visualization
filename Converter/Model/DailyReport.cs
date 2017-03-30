/*
 * Created by SharpDevelop.
 * User: lv.pengfei
 * Date: 2017/3/28
 * Time: 9:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Converter.Model
{
	/// <summary>
	/// Description of DailyReport.
	/// </summary>
	public class DailyReport
	{
		//日期
		public string BeginDate {get; set;}
		
		//节目数
		public int ProgramCount {get; set;}	
		
		//完成节目数
		public int AccomplishedProgramCount {get; set;}	
		
		//完成率(百分数)
		public decimal AccomplishmentRatio {get; set;}
		
		
		//节目总时长
		public decimal TotalProgramTimeLength {get; set;}
		
		//节目平均时长		
		public decimal AverageProgramTimeLength {get; set;}
		
		//总耗时
		public decimal TotalTaskDuration {get; set;}
		
		//平均耗时		
		public decimal AverageTaskDuration {get; set;}
		
		//生产效率
		public decimal Efficiency {get; set;}
	}
	
	
	public class ProduceItem
	{
		//节目时长(分)
		public decimal ProgramLength {get; set;}
		
		//BEGIN_TIME
		public DateTime Begin {get; set;}
		
		//END_TIME
		public DateTime? End {get; set;}
		
		//耗时（分）
		public decimal TaskDuration {get; set;}
	}
}
