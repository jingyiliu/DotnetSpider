﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DotnetSpider.DataFlow;
using DotnetSpider.DataFlow.Parser;
using DotnetSpider.Downloader;
using DotnetSpider.Scheduler;

namespace DotnetSpider.Sample.samples
{
	public class DatabaseSpider : Spider
	{
		protected override async Task Initialize()
		{
			NewGuidId();
			Scheduler = new QueueDistinctBfsScheduler();
			Speed = 1;
			Depth = 3;
			AddDataFlow(new DatabaseSpiderDataParser()).AddDataFlow(GetDefaultStorage());
			await AddRequests(
				new Request("https://news.cnblogs.com/n/page/1/", new Dictionary<string, string> {{"网站", "博客园"}}),
				new Request("https://news.cnblogs.com/n/page/2/", new Dictionary<string, string> {{"网站", "博客园"}}));
		}

		class DatabaseSpiderDataParser : DataParser
		{
			//public DatabaseSpiderDataParser()
			//{
			//	CanParse = DataParserHelper.CanParseByRegex("cnblogs\\.com");
			//	QueryFollowRequests = DataParserHelper.QueryFollowRequestsByXPath(".");
			//}

			protected override Task<DataFlowResult> Parse(DataFlowContext context)
			{
				context.AddData("URL", context.Response.Request.Url);
				context.AddData("Title", context.Selectable.XPath(".//title").GetValue());

				#region add mysql database

				var typeName = typeof(EntitySpider.CnblogsEntry).FullName;
				var entity = new EntitySpider.CnblogsEntry();
				context.Add(typeName, entity.GetTableMetadata());
				var items = new ParseResult<EntitySpider.CnblogsEntry>();
				entity.WebSite = context.Response.Request.Url;
				entity.Url = context.Response.Request.Url;
				entity.Title = context.Selectable.XPath(".//title").GetValue();
				items.Add(entity);
				context.AddParseData(typeName, items);

				#endregion

				return Task.FromResult(DataFlowResult.Success);
			}
		}

		public DatabaseSpider(SpiderParameters parameters) : base(parameters)
		{
		}
	}
}
