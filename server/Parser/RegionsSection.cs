namespace atlantis
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class RegionsSection : IReportSectionParser {
        public Task<bool> CanParseAsync(Cursor<TextParser> cursor) => IsRegion(cursor);

        private async Task<bool> IsRegion(Cursor<TextParser> cursor) {
            bool isRegion = false;
            if (await cursor.NextAsync()) {
                isRegion = cursor.Value.Match("-----");
                cursor.Value.Reset();
            }

            cursor.Back();

            return isRegion;
        }

        private bool IsUnit(TextParser p) {
            var isRegionContent = p.OneOf(
                x => x.Match("-"),
                x => x.Match("*")
            );
            p.Reset();

            return isRegionContent;
        }

        private bool IsStructure(TextParser p) {
            var isRegionContent = p.Match("+");
            p.Reset();

            return isRegionContent;
        }

        public async Task ParseAsync(Cursor<TextParser> cursor, JsonWriter writer) {
            await writer.WritePropertyNameAsync("regions");
            await writer.WriteStartArrayAsync();

            do {
                await writer.WriteStartObjectAsync();
                var h = cursor.Value.AsString();
                await AllParsers.RegionHeader.Parse(cursor.Value).Value.WriteJson(writer);

                await cursor.SkipEmptyLines();
                await writer.WritePropertyNameAsync("props");
                await writer.WriteValueAsync(cursor.Value.AsString());

                await cursor.SkipEmptyLines();
                await writer.WritePropertyNameAsync("exits");
                await writer.WriteValueAsync(cursor.Value.AsString());

                bool hasStructures = false;

                await writer.WritePropertyNameAsync("units");
                await writer.WriteStartArrayAsync();
                while (await cursor.NextAsync()) {
                    if (cursor.Value.EOF) continue;

                    if (IsUnit(cursor.Value)) {
                        var unit = AllParsers.Unit.Parse(cursor.Value);
                        if (unit) await unit.Value.WriteJson(writer);

                        continue;
                    }

                    if (IsStructure(cursor.Value)) {
                        if (!hasStructures) {
                            await writer.WriteEndArrayAsync();

                            hasStructures = true;
                            await writer.WritePropertyNameAsync("structures");
                            await writer.WriteStartArrayAsync();
                        }
                        else {
                            await writer.WriteEndArrayAsync();
                            await writer.WriteEndObjectAsync();
                        }

                        await writer.WriteStartObjectAsync();
                        await writer.WritePropertyNameAsync("structure");

                        var structure = AllParsers.Structure.Parse(cursor.Value);
                        if (structure) await structure.Value.WriteJson(writer);

                        await writer.WritePropertyNameAsync("units");
                        await writer.WriteStartArrayAsync();

                        continue;
                    }

                    cursor.Back();
                    break;
                }

                await writer.WriteEndArrayAsync();
                if (hasStructures) {
                    await writer.WriteEndObjectAsync();
                    await writer.WriteEndArrayAsync();
                }

                await writer.WriteEndObjectAsync();
            }
            while (await cursor.NextAsync() && await IsRegion(cursor));

            await writer.WriteEndArrayAsync();
            cursor.Back();
        }
    }
}
