using Statiq.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlwaysDeveloping.Shortcode
{
    public class TickBlock : IShortcode
    {
        public async Task<IEnumerable<ShortcodeResult>> ExecuteAsync(KeyValuePair<string, string>[] args, string content, IDocument document, IExecutionContext context)
        {
            var infoBlock = new ShortcodeResult($@"
<blockquote class=""info-block"">
    <div>
        <div style=""width: 5%; float: left; vertical-align: middle; padding-right: 60px; "">
            <svg xmlns=""http://www.w3.org/2000/svg"" class=""icon icon-tabler icon-tabler-circle-check"" width=""44"" height=""44"" viewBox=""0 0 24 24"" stroke-width=""2"" stroke=""#7bc62d"" fill=""none"" stroke-linecap=""round"" stroke-linejoin=""round"">
                <path stroke =""none"" d=""M0 0h24v24H0z"" fill=""none"" />
                <circle cx=""12"" cy=""12"" r=""9"" />
                <path d=""M9 12l2 2l4 -4"" />
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
