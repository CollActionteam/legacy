// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Too many exceptions to this rule, also seems a little buggy")]
[assembly: SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "We're not internationalized currently")]
[assembly: SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "We're mostly not normalizing strings when we're doing ToLower")]
[assembly: SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Property setters needed for EF core and for options objects")]
[assembly: SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "We're not a library")]
[assembly: SuppressMessage("Security", "CA5394: Do not use insecure randomness", Justification = "We're not using randomness for security")]