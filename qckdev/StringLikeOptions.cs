
namespace qckdev
{

    /// <summary>
    /// Provides options for LIKE operation.
    /// </summary>
    public sealed class StringLikeOptions
    {

        /// <summary>
        /// Gets or sets a character used to substitute zero or more characters in a <see cref="System.String"/>.
        /// </summary>
        public char WildCard { get; set; } = '*';
        /// <summary>
        /// Gets or sets a character used to substitute a single character in a <see cref="System.String"/>.
        /// </summary>
        public char CharWildCard { get; set; } = '?';

        /// <summary>
        /// Gets or sets true to ignore case during the comparison; otherwise, false.
        /// </summary>
        public bool IgnoreCase { get; set; } = true;
    
    }
}
