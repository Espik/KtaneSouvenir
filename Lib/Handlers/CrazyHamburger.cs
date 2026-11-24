using System.Collections.Generic;
using System.Linq;
using Souvenir;
using UnityEngine;
using static Souvenir.AnswerLayout;

public enum SCrazyHamburger
{
    [Question("What was the {1} ingredient shown in {0}?", ThreeColumns6Answers, AnswerType = InfoType.Sprites, Arguments = [QandA.Ordinal], ArgumentGroupSize = 1)]
    Ingredient
}

public partial class SouvenirModule
{
    [Handler("GSCrazyHamburger", "Crazy Hamburger", typeof(SCrazyHamburger), "noting3548")]
    [ManualQuestion("What were the displayed ingredients?")]
    private IEnumerator<SouvenirInstruction> ProcessCrazyHamburger(ModuleData module)
    {
        var comp = GetComponent(module, "CrazyHamburgerScript");
        var fldIngredients = GetField<string>(comp, "Ingredients");

        var spriteRenderers = GetArrayField<SpriteRenderer>(comp, "Sprites", isPublic: true).Get(expectedLength: 7).Take(6).ToArray();
        var spriteScales = new[] { 2750, 2000, 6000, 1250, 1750, 2500 };

        var ingredientSprites = new Sprite[spriteRenderers.Length];
        for (var i = 0; i < spriteRenderers.Length; i++)
            ingredientSprites[i] = spriteRenderers[i].sprite.TranslateSprite(spriteScales[i]);

        yield return WaitForSolve;

        var dic = new Dictionary<char, Sprite>()
        {
            ['B'] = ingredientSprites[0],
            ['C'] = ingredientSprites[1],
            ['G'] = ingredientSprites[2],
            ['H'] = ingredientSprites[3],
            ['O'] = ingredientSprites[4],
            ['R'] = ingredientSprites[5]
        };

        var ingredients = fldIngredients.Get(v => v.Any(ch => !dic.ContainsKey(ch)) ? $"expected only characters {dic.Keys.JoinString()}" : null);

        for (var i = 0; i < ingredients.Length; i++)
            yield return question(SCrazyHamburger.Ingredient, args: [Ordinal(i + 1)]).Answers(dic[ingredients[i]], all: ingredientSprites);
    }
}
