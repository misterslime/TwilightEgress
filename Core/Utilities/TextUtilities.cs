namespace Cascade
{
    public static partial class Utilities
    {
        /// <summary>
        /// Adds a new tooltip line to an already existing item's description by inserting at a specified index point.
        /// </summary>
        /// <param name="indexToInsertAt">The index in the tooltip where this should be inserted at.</param>
        /// <param name="tooltipLineName">The name of your new tooltip line.</param>
        /// <param name="tooltipLineDescription">the description of your new tooltip line.</param>
        public static void InsertNewTooltipLine(this List<TooltipLine> tooltipLines, int indexToInsertAt, string tooltipLineName, string tooltipLineDescription)
        {
            tooltipLines.Insert(indexToInsertAt, new(Cascade.Instance, tooltipLineName, tooltipLineDescription));
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
