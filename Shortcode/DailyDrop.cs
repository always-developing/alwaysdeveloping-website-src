using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysDeveloping.Shortcode
{
    public class DailyDrop : IShortcode
    {
        public async Task<IEnumerable<ShortcodeResult>> ExecuteAsync(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
        {
            var dailyDrop = new ShortcodeResult($@"
<blockquote class=""daily-drop"">
    <div>
        <div style=""width: 5%; float: left; vertical-align: middle; padding-right: 60px; "">
            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-bulb"" width=""50"" height=""50"" viewBox=""0 0 24 24"" stroke-width=""1.5"" stroke=""#ffec00"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
              <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none""/>
              <path d=""M3 12h1m8 -9v1m8 8h1m-15.4 -6.4l.7 .7m12.1 -.7l-.7 .7"" />
              <path d=""M9 16a5 5 0 1 1 6 0a3.5 3.5 0 0 0 -1 3a2 2 0 0 1 -4 0a3.5 3.5 0 0 0 -1 -3"" />
              <line x1=""9.7"" y1=""17"" x2=""14.3"" y2=""17"" />
            </svg>
        </div>
        <div class=""drop-header"">
            Daily Drop {content}
        </div>
        <div>
            <br>At the start of 2022 I set myself the goal of learning one new coding related piece of knowledge a day.<br> It could be anything - some.NET / C# functionality I wasn't aware of, a design practice, a cool new coding technique, or just something I find interesting. It could be something I knew at one point but had forgotten, or something completely new, which I may or may never actually use.<br><br>
            The Daily Drop is a record of these pieces of knowledge - writing about and summarizing them helps re-enforce the information for myself, as well as potentially helps others learn something new as well.
        <div>
    </div>
</blockquote>
");

            return await Task.FromResult(new List<ShortcodeResult> { dailyDrop });
        }
    }
}
