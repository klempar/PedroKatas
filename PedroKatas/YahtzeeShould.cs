using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;

namespace PedroKatas;

public enum CATEGORY
{
    FOURs,
    TWOs,
    THREES,
    FIVES,
    SIXES,
    ONES,
    PAIR
}

public class YahtzeeShould
{
    [Theory]
    [MemberData(nameof(TestData))]
    public void UpdateScordcard(string name, List<int> rolls, CATEGORY category, Dictionary<CATEGORY, int> expected)
    {
        var scoringSystem = new YatzeeScoringSystem();
        var result = scoringSystem.UpdateScoreCard(name, rolls, category);

        result.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestData =>
        new List<object[]>
        {
            new object[]
            {
                "Satish",
                new List<int> { 2, 2 },
                CATEGORY.TWOs,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 4 },
                    { CATEGORY.FOURs, 0 }
                }
            },
            new object[]
            {
                "Honza",
                new List<int> { 4, 4, 1 },
                CATEGORY.FOURs,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.FOURs, 8 }
                }
            },
            new object[]
            {
                "Honza2",
                new List<int> { 3, 3, 3, 3, 1 },
                CATEGORY.THREES,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.THREES, 12 }
                }
            },
            new object[]
            {
                "Satish2",
                new List<int> { 5, 5, 5, 5, 5 },
                CATEGORY.FIVES,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.FIVES, 25 }
                }
            },
            new object[]
            {
                "Honza3",
                new List<int> { 1, 2, 3, 4, 6 },
                CATEGORY.SIXES,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.SIXES, 6 }
                }
            },
            new object[]
            {
                "Satish3",
                new List<int> { 1, 2, 3, 4, 5 },
                CATEGORY.ONES,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.ONES, 1 }
                }
            },
            new object[]
            {
                "HonzaPair",
                new List<int> {4, 4},
                CATEGORY.PAIR,
                new Dictionary<CATEGORY, int>
                {
                    { CATEGORY.TWOs, 0 },
                    { CATEGORY.PAIR, 8 }
                }
            }
        };
}

public class YatzeeScoringSystem
{
    public Dictionary<string, Dictionary<CATEGORY, int>> scordCard = new()
    {
        {
            "Satish", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.FOURs, 0 }
            }
        },
        {
            "Honza", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.FOURs, 0 }
            }
            
        },
        {
            "Honza2", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.THREES, 0 }
            }
        },
        {
            "Satish2", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.FIVES, 0 }
            }
        },
        {
            "Honza3", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.SIXES, 0 }
            }
        },
        {
            "Satish3", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.ONES, 0 }
            }
        },
        {
            "HonzaPair", new Dictionary<CATEGORY, int>
            {
                { CATEGORY.TWOs, 0 },
                { CATEGORY.PAIR, 0 }
            }
        }
    };

    public Dictionary<CATEGORY, int> UpdateScoreCard(string name, List<int> chosenRolls, CATEGORY expectedCATEGORY)
    {
        int score = 0;

        // combinations
        if (CATEGORY.PAIR == expectedCATEGORY)
        {
            // var groupedRolls = chosenRolls.GroupBy(x => x)
            //     .Where(g => g.Count() >= 2)
            //     .Select(g => g.Key)
            //     .ToList();

            int numberOfAppearences = 0;
            int chosenPairDie = 0;

            for (var dieIndex = 0; dieIndex < chosenRolls.Count; dieIndex++)
            {
                for (var dieIndexInner = 0; dieIndexInner < chosenRolls.Count; dieIndexInner++)
                {
                    if (dieIndex == dieIndexInner)
                    {
                        continue;
                    }
                    if (chosenRolls[dieIndex] == chosenRolls[dieIndexInner])
                    {
                        numberOfAppearences++;
                        chosenPairDie = chosenRolls[dieIndex];
                    }
                }
            }
            if (numberOfAppearences == 2)
            {
                score = chosenPairDie * 2;
            }
            scordCard[name][expectedCATEGORY] = score;
            return scordCard[name]; 
        }
        
        // singles
        foreach (var die in chosenRolls)
        {
            if ((expectedCATEGORY == CATEGORY.TWOs && die == 2) ||
                (expectedCATEGORY == CATEGORY.FOURs && die == 4) ||
                (expectedCATEGORY == CATEGORY.THREES && die == 3) ||
                (expectedCATEGORY == CATEGORY.FIVES && die == 5) ||
                (expectedCATEGORY == CATEGORY.SIXES && die == 6) ||
                (expectedCATEGORY == CATEGORY.ONES && die == 1))
            {
                score += die;
            }
        }

        scordCard[name][expectedCATEGORY] = score;

        return scordCard[name];
    }
}