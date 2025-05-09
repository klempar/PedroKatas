using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;

namespace PedroKatas;

public enum CATEGORY
{
    FOURs,
    TWOs
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
        }
    };

    public Dictionary<CATEGORY, int> UpdateScoreCard(string name, List<int> chosenRolls, CATEGORY expectedCATEGORY)
    {
        int score = 0;

        foreach (var die in chosenRolls)
        {
            if ((expectedCATEGORY == CATEGORY.TWOs && die == 2) ||
                (expectedCATEGORY == CATEGORY.FOURs && die == 4))
            {
                score += die;
            }
        }

        scordCard[name][expectedCATEGORY] = score;

        return scordCard[name];
    }
}