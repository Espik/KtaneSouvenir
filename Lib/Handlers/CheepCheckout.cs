using System.Collections.Generic;
using System.Linq;
using Souvenir;
using UnityEngine;

using static Souvenir.AnswerLayout;

public enum SCheepCheckout
{
    [Question("Which of these bird sounds could be heard in {0}?", TwoColumns4Answers, AnswerType = InfoType.Audio, ForeignAudioID = Sounds.Generated)]
    Birds
}

public partial class SouvenirModule
{
    private Dictionary<string, AudioClip> _cheepCheckoutAudio = null;

    [Handler("cheepCheckout", "Cheep Checkout", typeof(SCheepCheckout), "Quinn Wuest")]
    [ManualQuestion("Which bird sounds could be heard?")]
    private IEnumerator<SouvenirInstruction> ProcessCheepCheckout(ModuleData module)
    {
        var comp = GetComponent(module, "cheepCheckoutScript");
        var fldUnicorn = GetField<bool>(comp, "unicorn");
        var birdPitches = GetField<string>(comp, "SouvenirBirdsPitches");

        yield return WaitForSolve;

        if (_cheepCheckoutAudio is null)
        {
            _cheepCheckoutAudio = [];
            var lmh = new[] { "low", "med", "high" };
            var chirpClips = lmh.Select(prefix => Enumerable.Range(1, 3).Select(suffix => Sounds.GetForeignClip("cheepCheckout", prefix + suffix)).ToArray()).ToArray();

            string data = birdPitches.Get();
            foreach (var str in data.Split(';'))
            {
                if (str.Split('=') is { Length: 2 } parts && parts[0] is { } name && parts[1] is { } chirps)
                    _cheepCheckoutAudio[name] = Sounds.Combine($"cheepCheckout_{name}",
                        (0.0f, chirpClips["LMH".IndexOf(chirps[0])][0]),
                        (0.68f, chirpClips["LMH".IndexOf(chirps[1])][1]),
                        (1.36f, chirpClips["LMH".IndexOf(chirps[2])][2]));
                yield return null;
            }
        }

        if (fldUnicorn.Get())
            yield return legitimatelyNoQuestion(module, "The unicorn applied.");

        var birdNames = GetArrayField<string>(comp, "ruleseededBirdNames").Get(expectedLength: 27, validator: v => v != "[Unicorn Bastard]" && !_cheepCheckoutAudio.ContainsKey(v) ? "Invalid bird name" : null);
        var shuffledList = GetArrayField<int>(comp, "numberArray").Get(expectedLength: 27);
        var birdsPresent = shuffledList.Take(5).Where(ix => ix < 26).Select(ix => _cheepCheckoutAudio[birdNames[ix]]).ToArray();

        yield return question(SCheepCheckout.Birds).Answers(birdsPresent, all: _cheepCheckoutAudio.Values.ToArray());
    }
}
