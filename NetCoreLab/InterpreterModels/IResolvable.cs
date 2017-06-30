namespace NetCoreLab.InterpreterModels
{
    /// <summary>
    /// Represents a resolvable template element.
    /// </summary>
    internal interface IResolvable
    {
        /// <summary>
        /// Resolves the element using the data context.
        /// </summary>
        /// <param name="target">Data context.</param>
        /// <returns>Formatted string.</returns>
        string ResolveTemplate(object target);

        /// <summary>
        /// Concrete form of the element without trimming or fomatting.
        /// </summary>
        string ConcreteForm { get; set; }
    }
}
