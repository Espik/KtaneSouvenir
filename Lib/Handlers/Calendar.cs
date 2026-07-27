using System.Collections.Generic;
using Souvenir;

using static Souvenir.AnswerLayout;

public enum SCalendar
{
    [Question("What was the holiday in {0}?", OneColumn4Answers, "April Fools’", "Australia Day", "Bastille Day", "Christmas Eve", "Cinco de Mayo", "Day of German Unity", "Day of the Dead", "Earth Day", "Epiphany", "Golden Week", "Groundhog Day", "Guy Fawkes Night", "Kwanzaa", "Republic Day", "Saint Patrick’s Day", "Valentine’s Day", "Veterans Day", "World Braille Day", TranslateAnswers = true)]
    Holiday
}

public partial class SouvenirModule
{
    [Handler("calendar", "Calendar", typeof(SCalendar), "Timwi")]
    [ManualQuestion("What was the holiday?")]
    private IEnumerator<SouvenirInstruction> ProcessCalendar(ModuleData module)
    {
        var comp = GetComponent(module, "calendar");
        var allHolidays = SCalendar.Holiday.GetAnswers(); // We do not get the names directly from the module due to typos, wrong apostrophes, and various inconsistencies from the manual

        yield return WaitForSolve;

        var holiday = GetIntField(comp, "holiday").Get(min: 0, max: 17);

        // Don’t ask a question if the holiday is in the submitted month
        var correctMonth = GetIntField(comp, "correctMonthIndex").Get(min: 0, max: 11);
        if (holiday switch
        {
            0 => correctMonth is 3,        // April Fools’ — April
            1 => correctMonth is 0,        // Australia Day — January
            2 => correctMonth is 6,        // Bastille Day — July
            3 => correctMonth is 11,       // Christmas Eve — December
            4 => correctMonth is 4,        // Cinco de Mayo — May
            5 => correctMonth is 9,        // Day of German Unity — October
            6 => correctMonth is 9,        // Day of the Dead — October
            7 => correctMonth is 3,        // Earth Day — April
            8 => correctMonth is 0,        // Epiphany — January
            9 => correctMonth is 3 or 4,   // Golden Week — April & May
            10 => correctMonth is 1,       // Groundhog Day — Feburary
            11 => correctMonth is 10,      // Guy Fawkes Night — November
            12 => correctMonth is 0 or 11, // Kwanzaa — December & January
            13 => correctMonth is 5,       // Republic Day — June
            14 => correctMonth is 2,       // Saint Patrick’s Day — March
            15 => correctMonth is 1,       // Valentine’s Day — Feburary
            16 => correctMonth is 10,      // Veterans Day — November
            _ => correctMonth is 0         // World Braille Day — January
        })
            yield return legitimatelyNoQuestion(module, "The holiday is present in the submitted month.");

        yield return question(SCalendar.Holiday).Answers(allHolidays[holiday]);
    }
}
