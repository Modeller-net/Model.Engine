using System.Text.RegularExpressions;

namespace Names;

/// <summary>
/// A container for exceptions to simple pluralization/singularization rules.
/// Vocabularies.Default contains an extensive list of rules for US English.
/// At this time, multiple vocabularies and removing existing rules are not supported.
/// </summary>
public class Vocabulary
{
    internal Vocabulary()
    { }

    private readonly List<Rule> _plurals = [];
    private readonly List<Rule> _singulars = [];
    private readonly List<string> _uncountables = [];

    /// <summary>
    /// Adds a word to the vocabulary which cannot easily be pluralized/singularized by RegEx, e.g. "person" and "people".
    /// </summary>
    /// <param name="singular">The singular form of the irregular word, e.g. "person".</param>
    /// <param name="plural">The plural form of the irregular word, e.g. "people".</param>
    /// <param name="matchEnding">True to match these words on their own as well as at the end of longer words. False, otherwise.</param>
    public void AddIrregular(string singular, string plural, bool matchEnding = true)
    {
        if (matchEnding)
        {
            AddPlural("(" + singular[0] + ")" + singular[1..] + "$", string.Concat("$1", plural.AsSpan(1)));
            AddSingular("(" + plural[0] + ")" + plural[1..] + "$", string.Concat("$1", singular.AsSpan(1)));
        }
        else
        {
            AddPlural($"^{singular}$", plural);
            AddSingular($"^{plural}$", singular);
        }
    }

    /// <summary>
    /// Adds an uncountable word to the vocabulary, e.g. "fish".  Will be ignored when plurality is changed.
    /// </summary>
    /// <param name="word">Word to be added to the list of uncountables.</param>
    public void AddUncountable(string word)
    {
        _uncountables.Add(word.ToLower());
    }

    /// <summary>
    /// Adds a rule to the vocabulary that does not follow trivial rules for pluralization, e.g. "bus" -> "buses"
    /// </summary>
    /// <param name="rule">RegEx to be matched, case insensitive, e.g. "(bus)es$"</param>
    /// <param name="replacement">RegEx replacement  e.g. "$1"</param>
    public void AddPlural(string rule, string replacement)
    {
        _plurals.Add(new(rule, replacement));
    }

    /// <summary>
    /// Adds a rule to the vocabulary that does not follow trivial rules for singularization, e.g. "vertices/indices -> "vertex/index"
    /// </summary>
    /// <param name="rule">RegEx to be matched, case insensitive, e.g. ""(vert|ind)ices$""</param>
    /// <param name="replacement">RegEx replacement  e.g. "$1ex"</param>
    public void AddSingular(string rule, string replacement)
    {
        _singulars.Add(new(rule, replacement));
    }

    /// <summary>
    /// Pluralizes the provided input considering irregular words
    /// </summary>
    /// <param name="word">Word to be pluralized</param>
    /// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
    /// <returns></returns>
    public string Pluralize(string word, bool inputIsKnownToBeSingular = true)
    {
        var result = ApplyRules(_plurals, word, false);

        if (inputIsKnownToBeSingular)
        {
            return result ?? word;
        }

        var asSingular = ApplyRules(_singulars, word, false);
        var asSingularAsPlural = ApplyRules(_plurals, asSingular, false);
        if (asSingular != null && asSingular != word && asSingular + "s" != word && asSingularAsPlural == word && result != word)
        {
            return word;
        }

        return result ?? word;
    }

    /// <summary>
    /// Singularizes the provided input considering irregular words
    /// </summary>
    /// <param name="word">Word to be singularized</param>
    /// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
    /// <param name="skipSimpleWords">Skip singularizing single words that have an 's' on the end</param>
    /// <returns></returns>
    public string Singularize(string word, bool inputIsKnownToBePlural = true, bool skipSimpleWords = false)
    {
        var result = ApplyRules(_singulars, word, skipSimpleWords);

        if (inputIsKnownToBePlural)
        {
            return result ?? word;
        }

        // the Plurality is unknown so we should check all possibilities
        var asPlural = ApplyRules(_plurals, word, false);
        var asPluralAsSingular = ApplyRules(_singulars, asPlural, false);
        if (asPlural != word && word + "s" != asPlural && asPluralAsSingular == word && result != word)
        {
            return word;
        }

        return result ?? word;
    }

    private string? ApplyRules(IList<Rule> rules, string? word, bool skipFirstRule)
    {
        if (word is null)
        {
            return null;
        }

        if (word.Length < 1)
        {
            return word;
        }

        if (IsUncountable(word))
        {
            return word;
        }

        var result = word;
        var end = skipFirstRule ? 1 : 0;
        for (var i = rules.Count - 1; i >= end; i--)
        {
            if ((result = rules[i].Apply(word)) != null)
            {
                break;
            }
        }
        return result != null ? MatchUpperCase(word, result) : result;
    }

    private bool IsUncountable(string word)
    {
        return _uncountables.Contains(word.ToLower());
    }

    private static string MatchUpperCase(string word, string replacement)
    {
        return char.IsUpper(word[0]) && char.IsLower(replacement[0]) ? char.ToUpper(replacement[0]) + replacement.Substring(1) : replacement;
    }

    private class Rule(string pattern, string replacement)
    {
        private readonly Regex _regex = new(pattern, RegexOptions.IgnoreCase | RegexOptionsUtil.Compiled);

        public string? Apply(string word) => !_regex.IsMatch(word) ? null : _regex.Replace(word, replacement);
    }
}