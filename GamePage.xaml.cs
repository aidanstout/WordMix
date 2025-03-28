using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Microsoft.Maui.Media;

namespace BE_App;

public partial class GamePage : ContentPage
{
    private CancellationTokenSource _cts;
    string word;
    int score;
    // Expanded word list with categorization by syllable count
    private readonly Dictionary<int, List<string>> wordsBySyllable = new Dictionary<int, List<string>>
    {
        { 1, new List<string> { "axe", "bed", "cup", "door", "town", "sink", "lamp", "box", "frog", "ship" } },
        { 2, new List<string> { "apple", "window", "pencil", "turtle", "basket", "planet", "helmet", "rocket" } },
        { 3, new List<string> { "elephant", "dinosaur", "butterfly", "umbrella", "hospital", "banana" } }
    };

    private int currentDifficulty = 1; 

    Random rand = new Random();
    //instance of word variant generator
    private readonly DyslexiaWordVariantGenerator variantGenerator = new DyslexiaWordVariantGenerator();

    public GamePage()
    {
        InitializeComponent();

        // Start with a random word from the 1-syllable list
        SelectRandomWord();
        askQuestion(word);
    }

    private void SelectRandomWord()
    {
        // Choose which syllable group to use
        int syllableCount = rand.Next(1, 4); 

        if (!wordsBySyllable.ContainsKey(syllableCount) || wordsBySyllable[syllableCount].Count == 0)
            syllableCount = 1; // Default to 1-syllable words

        // Select a random word from the chosen syllable group
        int randnum = rand.Next(wordsBySyllable[syllableCount].Count);
        word = wordsBySyllable[syllableCount][randnum];
    }

    private string[] Word_Jumble(string word)
    {
        //generator to create word variants based on dyslexic patterns
        List<string> variants = variantGenerator.GenerateWordVariants(word, 3);
        return variants.ToArray();
    }

    private void checkAnswer()
    {
        score += 100;
        score_keeper.Text = "Score: " + score;

        // TODO adjusting difficulty based on score or streak of correct answers
        AdjustDifficulty();

        // Select a new word
        SelectRandomWord();
        askQuestion(word);
    }

    private void AdjustDifficulty()
    {

        if (score >= 500 && currentDifficulty < 3)
            currentDifficulty = 3;
        else if (score >= 300 && currentDifficulty < 2)
            currentDifficulty = 2;
    }

    private void askQuestion(string correct_word)
    {

        if (_cts != null && !_cts.Token.IsCancellationRequested)
        {
            _cts.Cancel(); // Cancel the previous utterance
            _cts.Dispose(); // Dispose the old token source
        }

        _cts = new CancellationTokenSource(); // Create a new token source
        
        TextToSpeech.Default.SpeakAsync("Find the correct spelling of the word " + word, cancelToken: _cts.Token);

        string[] jumbled = Word_Jumble(correct_word);

        // Randomize the position of answers
        int[] positions = { 0, 1, 2, 3 };
        ShuffleArray(positions);

        // Assign words to buttons
        button1.Text = jumbled[positions[0]];
        button2.Text = jumbled[positions[1]];
        button3.Text = jumbled[positions[2]];
        button4.Text = jumbled[positions[3]];
    }

    // Fisher-Yates shuffle algorithm for randomizing button positions
    private void ShuffleArray<T>(T[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + rand.Next(n - i);
            T temp = array[r];
            array[r] = array[i];
            array[i] = temp;
        }
    }

    private void Button4_Clicked(object sender, EventArgs e)
    {
        if (button4.Text == word)
        {
            checkAnswer();
        }
        
    }

    private void Button3_Clicked(object sender, EventArgs e)
    {
        if (button3.Text == word)
        {
            checkAnswer();
        }
        
    }

    private void Button2_Clicked(object sender, EventArgs e)
    {
        if (button2.Text == word)
        {
            checkAnswer();
        }
        
    }

    private void Button1_Clicked(object sender, EventArgs e)
    {
        if (button1.Text == word)
        {
            checkAnswer();
        }
        
    }

    // hint button text to speech
    private async void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        if (_cts != null && !_cts.Token.IsCancellationRequested)
        {
            _cts.Cancel(); // Cancel the previous utterance
            _cts.Dispose(); // Dispose the old token source
        }

        _cts = new CancellationTokenSource(); // Create a new token source

        var speechOptions = new SpeechOptions
        {
            Pitch = 0.3f
            
        };
        await TextToSpeech.Default.SpeakAsync(word, speechOptions, cancelToken: _cts.Token);
    }
}


// Class for generating dyslexia-informed word variants
public class DyslexiaWordVariantGenerator
{
    private static readonly Random _random = new Random();

    // Common dyslexic error patterns
    private static readonly Dictionary<char, char[]> VisuallyConfusedLetters = new Dictionary<char, char[]>
    {
        { 'b', new[] { 'd', 'p', 'q' } },
        { 'd', new[] { 'b', 'p', 'q' } },
        { 'p', new[] { 'b', 'd', 'q' } },
        { 'q', new[] { 'b', 'd', 'p' } },
        { 'm', new[] { 'n', 'w' } },
        { 'n', new[] { 'm', 'u' } },
        { 'u', new[] { 'n', 'v' } },
        { 'v', new[] { 'u', 'w' } },
        { 'w', new[] { 'v', 'm' } },
        { 'g', new[] { 'q', 'j' } },
        { 'i', new[] { 'j', 'l' } },
        { 'j', new[] { 'i', 'g' } },
        { 'l', new[] { 'i', 't' } },
        { 't', new[] { 'l', 'f' } },
        { 'f', new[] { 't' } }
    };

    // Common letter sequence reversals
    private static readonly Dictionary<string, string> ReversalPatterns = new Dictionary<string, string>
    {
        { "on", "no" },
        { "oa", "ao" },
        { "th", "ht" },
        { "ch", "hc" },
        { "sh", "hs" },
        { "wh", "hw" },
        { "br", "rb" },
        { "cr", "rc" },
        { "dr", "rd" },
        { "fr", "rf" },
        { "gr", "rg" },
        { "pr", "rp" },
        { "tr", "rt" },
        { "bl", "lb" },
        { "cl", "lc" },
        { "fl", "lf" },
        { "gl", "lg" },
        { "pl", "lp" },
        { "sl", "ls" }
    };

    // Common phonetic confusions
    private static readonly Dictionary<string, string[]> PhoneticConfusions = new Dictionary<string, string[]>
    {
        { "a", new[] { "e", "o", "u" } },
        { "e", new[] { "a", "i" } },
        { "i", new[] { "e", "y" } },
        { "o", new[] { "a", "u" } },
        { "u", new[] { "o", "a" } },
        { "c", new[] { "k", "s" } },
        { "k", new[] { "c", "q" } },
        { "s", new[] { "c", "z" } },
        { "z", new[] { "s" } },
        { "f", new[] { "ph", "v" } },
        { "ph", new[] { "f" } },
        { "v", new[] { "f" } },
        { "th", new[] { "f", "v" } },
        { "ch", new[] { "tch", "sh" } },
        { "tch", new[] { "ch" } },
        { "sh", new[] { "ch" } },
        { "wh", new[] { "w" } },
        { "w", new[] { "wh" } },
        { "tion", new[] { "sion", "shun" } },
        { "sion", new[] { "tion" } }
    };


    /// <summary>
    /// Generates variant spellings for a word based on common dyslexic patterns
    /// </summary>
    /// <param name="word">The correctly spelled word</param>
    /// <param name="numVariants">Number of variant spellings to generate (default 3)</param>
    /// <returns>A list with the correct word as the first element, followed by variant spellings</returns>
    public List<string> GenerateWordVariants(string word, int numVariants = 3)
    {
        if (string.IsNullOrWhiteSpace(word))
            throw new ArgumentException("Word cannot be empty or null", nameof(word));

        // Store original word and variants
        var result = new List<string> { word.ToLower() };
        var variants = new HashSet<string>();

        // Generate more variants than needed to ensure we have enough unique ones
        int attemptsMultiplier = 3;
        int maxAttempts = numVariants * attemptsMultiplier;
        int attempts = 0;

        while (variants.Count < numVariants && attempts < maxAttempts)
        {
            attempts++;
            // Choose a random error pattern based on word length and complexity
            int errorType = _random.Next(4);
            string variant = "";

            switch (errorType)
            {
                case 0:
                    variant = ApplyVisualConfusion(word);
                    break;
                case 1:
                    variant = ApplyLetterReversal(word);
                    break;
                case 2:
                    variant = ApplyPhoneticConfusion(word);
                    break;
                case 3:
                    variant = ApplyVowelConfusion(word);
                    break;
            }

            // Only add if it's a unique variant and not the original word
            if (!string.IsNullOrEmpty(variant) && variant != word && !variants.Contains(variant))
            {
                variants.Add(variant);
            }
        }

        // Add unique variants to result
        result.AddRange(variants.Take(numVariants));

        // If we still don't have enough variants, add some simpler ones
        while (result.Count < numVariants + 1)
        {
            string backupVariant = ApplySimpleSubstitution(word);
            if (!result.Contains(backupVariant))
            {
                result.Add(backupVariant);
            }
        }

        return result;
    }

    private string ApplyVisualConfusion(string word)
    {
        char[] letters = word.ToCharArray();

        // Find positions of letters that can be visually confused
        var eligiblePositions = new List<int>();
        for (int i = 0; i < letters.Length; i++)
        {
            if (VisuallyConfusedLetters.ContainsKey(letters[i]))
            {
                eligiblePositions.Add(i);
            }
        }

        if (eligiblePositions.Count == 0)
            return ApplySimpleSubstitution(word); // Fallback if no visually confused letters

        // Randomly select a position to change
        int pos = eligiblePositions[_random.Next(eligiblePositions.Count)];

        // Replace with a visually similar letter
        char[] alternatives = VisuallyConfusedLetters[letters[pos]];
        letters[pos] = alternatives[_random.Next(alternatives.Length)];

        return new string(letters);
    }

    private string ApplyLetterReversal(string word)
    {
        if (word.Length < 2)
            return word;

        StringBuilder result = new StringBuilder(word);

        // Find reversible patterns
        var reversalCandidates = new List<(int position, string pattern, string replacement)>();

        foreach (var pattern in ReversalPatterns.Keys)
        {
            int pos = word.IndexOf(pattern);
            if (pos >= 0)
            {
                reversalCandidates.Add((pos, pattern, ReversalPatterns[pattern]));
            }
        }

        // If no specific patterns found, try adjacent letter reversal
        if (reversalCandidates.Count == 0)
        {
            int pos = _random.Next(word.Length - 1);
            char temp = result[pos];
            result[pos] = result[pos + 1];
            result[pos + 1] = temp;
        }
        else
        {
            // Apply a specific reversal pattern
            var selected = reversalCandidates[_random.Next(reversalCandidates.Count)];
            result.Remove(selected.position, selected.pattern.Length);
            result.Insert(selected.position, selected.replacement);
        }

        return result.ToString();
    }

    private string ApplyPhoneticConfusion(string word)
    {
        StringBuilder result = new StringBuilder(word);
        var phonemePositions = new List<(int position, string original, string replacement)>();

        // Find positions of phonemes that can be confused
        foreach (var phoneme in PhoneticConfusions.Keys)
        {
            int pos = 0;
            while ((pos = word.IndexOf(phoneme, pos)) >= 0)
            {
                string[] alternatives = PhoneticConfusions[phoneme];
                string replacement = alternatives[_random.Next(alternatives.Length)];
                phonemePositions.Add((pos, phoneme, replacement));
                pos += phoneme.Length;
            }
        }

        if (phonemePositions.Count == 0)
            return ApplySimpleSubstitution(word); // Fallback

        // Apply one phonetic confusion
        var selected = phonemePositions[_random.Next(phonemePositions.Count)];
        result.Remove(selected.position, selected.original.Length);
        result.Insert(selected.position, selected.replacement);

        return result.ToString();
    }


    private string ApplyVowelConfusion(string word)
    {
        StringBuilder result = new StringBuilder(word);

        // Find positions of vowels
        var vowelPositions = new List<int>();
        for (int i = 0; i < word.Length; i++)
        {
            if ("aeiou".Contains(word[i]))
            {
                vowelPositions.Add(i);
            }
        }

        if (vowelPositions.Count == 0)
            return ApplySimpleSubstitution(word); // Fallback

        // Replace a vowel with another vowel
        int pos = vowelPositions[_random.Next(vowelPositions.Count)];
        char[] vowels = "aeiou".ToCharArray();
        char originalVowel = word[pos];
        char newVowel;

        do
        {
            newVowel = vowels[_random.Next(vowels.Length)];
        } while (newVowel == originalVowel);

        result[pos] = newVowel;

        return result.ToString();
    }

    private string ApplySimpleSubstitution(string word)
    {
        // Simple letter substitution as a fallback method
        char[] letters = word.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            if (VisuallyConfusedLetters.ContainsKey(letters[i]))
            {
                char[] alternatives = VisuallyConfusedLetters[letters[i]];
                letters[i] = alternatives[_random.Next(alternatives.Length)];
                break;
            }
        }
        return new string(letters);
    }

}