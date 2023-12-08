namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// Inserts a new tooltip line one index under the specified line's <see cref="TooltipLine.Name"/> index. 
        /// </summary>
        /// <param name="lineToInsertAt"></param>
        /// <param name="customTooltipLineName"></param>
        /// <param name="customTooltipLineDescription"></param>
        public static void InsertNewTooltipLine(this Item item, List<TooltipLine> tooltipLines, string lineToInsertAt, string customTooltipLineName, string customTooltipLineDescription)
        {
            // Do nothing if the item is in social slots.
            if (item.social)
                return;

            // Find the index of the line which we want to insert at.
            int insertionIndex = -1;
            for (int i = 0; i < tooltipLines.Count; i++)
            {
                if (tooltipLines[i].Name == lineToInsertAt)
                {
                    insertionIndex = i;
                    break;
                }
            }

            // Insert the new line.
            tooltipLines.Insert(insertionIndex + 1, new(Cascade.Instance, customTooltipLineName, customTooltipLineDescription));
        }

        /// <summary>
        /// Removes a specific tooltip line of an item based on the given identifier name.
        /// </summary>
        /// <param name="identifierName">the name of the category of line which should be removed. As an example, if I wanted to
        /// remove the line with an item's name, I would set this as <c>"ItemName"</c></param>
        public static void RemoveSpecificTooltipLine(this List<TooltipLine> tooltipLines, string identifierName)
        {
            tooltipLines.RemoveAll(line => line.Name.StartsWith(identifierName));
        }
    }
}
