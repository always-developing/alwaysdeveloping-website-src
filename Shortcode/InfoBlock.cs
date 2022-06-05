using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysDeveloping.Shortcode
{
    public class InfoBlock : IShortcode
    {
        public async Task<IEnumerable<ShortcodeResult>> ExecuteAsync(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
        {
            var infoBlock = new ShortcodeResult($@"
<blockquote class=""info-block"">
    <div>
        <div style=""width: 5%; float: left; vertical-align: middle; padding-right: 60px; "">
            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-info-circle"" width=""44"" height=""44"" viewBox=""0 0 24 24"" stroke-width=""1.5"" stroke=""#00abfb"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                <path stroke=""none"" d=""M0 0h24v24H0z"" fill=""none"" />
                <circle cx=""12"" cy=""12"" r=""9"" />
                <line x1=""12"" y1=""8"" x2=""12.01"" y2=""8"" />
                <polyline points=""11 12 12 12 12 16 13 16"" />
            </svg >
        </div >
        <div>
            <p>{content}</p>
        </div>
    </div>
</blockquote >
");

            return await Task.FromResult(new List<ShortcodeResult> { infoBlock });
        }
    }
}
