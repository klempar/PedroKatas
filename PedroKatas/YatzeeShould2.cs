// using System;
// using System.Collections.Generic;
// using System.Linq;
//
// namespace PedroKatas
// {
//     // === Enums ===
//     public enum CATEGORY
//     {
//         ONES,
//         TWOs,
//         THREES,
//         FOURS,
//         FIVES,
//         SIXES,
//         PAIR,
//         TWO_PAIRS,
//         THREE_OF_A_KIND,
//         FOUR_OF_A_KIND,
//         SMALL_STRAIGHT,
//         LARGE_STRAIGHT,
//         FULL_HOUSE,
//         YAHTZEE
//     }
//
//     // === Value Object ===
//     public class DiceValue
//     {
//         public int Value { get; }
//
//         public DiceValue(int value)
//         {
//             if (value < 1 || value > 6)
//                 throw new ArgumentException("Die value must be between 1 and 6");
//             Value = value;
//         }
//     }
//
//     public class Roll
//     {
//         public List<DiceValue> Dice { get; }
//
//         public Roll(IEnumerable<int> dice)
//         {
//             if (dice.Count() != 5)
//                 throw new ArgumentException("A roll must contain exactly 5 dice");
//
//             Dice = dice.Select(d => new DiceValue(d)).ToList();
//         }
//     }
//
//     // === Score wrapper ===
//     public class Score
//     {
//         public int Value { get; }
//
//         public Score(int value)
//         {
//             Value = value;
//         }
//     }
//
//     // === Scoring interface ===
//     public interface ICategory
//     {
//         CATEGORY Type { get; }
//         int CalculateScore(Roll roll);
//     }
//
//     // === Number Categories (Ones to Sixes) ===
//     public class NumberCategory : ICategory
//     {
//         public CATEGORY Type { get; }
//         private readonly int _targetValue;
//
//         public NumberCategory(CATEGORY type, int targetValue)
//         {
//             Type = type;
//             _targetValue = targetValue;
//         }
//
//         public int CalculateScore(Roll roll)
//         {
//             return roll.Dice.Where(d => d.Value == _targetValue).Sum(d => d.Value);
//         }
//     }
//
//     public class PairCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.PAIR;
//
//         public int CalculateScore(Roll roll)
//         {
//             return roll.Dice
//                 .GroupBy(d => d.Value)
//                 .Where(g => g.Count() >= 2)
//                 .OrderByDescending(g => g.Key)
//                 .Select(g => g.Key * 2)
//                 .FirstOrDefault();
//         }
//     }
//
//     public class TwoPairsCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.TWO_PAIRS;
//
//         public int CalculateScore(Roll roll)
//         {
//             var pairs = roll.Dice
//                 .GroupBy(d => d.Value)
//                 .Where(g => g.Count() >= 2)
//                 .OrderByDescending(g => g.Key)
//                 .Take(2)
//                 .ToList();
//
//             return pairs.Count == 2 ? pairs.Sum(p => p.Key * 2) : 0;
//         }
//     }
//
//     public class ThreeOfAKindCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.THREE_OF_A_KIND;
//
//         public int CalculateScore(Roll roll)
//         {
//             return roll.Dice
//                 .GroupBy(d => d.Value)
//                 .Where(g => g.Count() >= 3)
//                 .Select(g => g.Key * 3)
//                 .FirstOrDefault();
//         }
//     }
//
//     public class FourOfAKindCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.FOUR_OF_A_KIND;
//
//         public int CalculateScore(Roll roll)
//         {
//             return roll.Dice
//                 .GroupBy(d => d.Value)
//                 .Where(g => g.Count() >= 4)
//                 .Select(g => g.Key * 4)
//                 .FirstOrDefault();
//         }
//     }
//
//     public class SmallStraightCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.SMALL_STRAIGHT;
//
//         public int CalculateScore(Roll roll)
//         {
//             var expected = new HashSet<int> { 1, 2, 3, 4, 5 };
//             var actual = roll.Dice.Select(d => d.Value).ToHashSet();
//             return actual.SetEquals(expected) ? 15 : 0;
//         }
//     }
//
//     public class LargeStraightCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.LARGE_STRAIGHT;
//
//         public int CalculateScore(Roll roll)
//         {
//             var expected = new HashSet<int> { 2, 3, 4, 5, 6 };
//             var actual = roll.Dice.Select(d => d.Value).ToHashSet();
//             return actual.SetEquals(expected) ? 20 : 0;
//         }
//     }
//
//     public class FullHouseCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.FULL_HOUSE;
//
//         public int CalculateScore(Roll roll)
//         {
//             var groups = roll.Dice.GroupBy(d => d.Value).ToList();
//             var hasThree = groups.Any(g => g.Count() == 3);
//             var hasTwo = groups.Any(g => g.Count() == 2);
//
//             return hasThree && hasTwo ? roll.Dice.Sum(d => d.Value) : 0;
//         }
//     }
//
//     public class YahtzeeCategory : ICategory
//     {
//         public CATEGORY Type => CATEGORY.YAHTZEE;
//
//         public int CalculateScore(Roll roll)
//         {
//             return roll.Dice.All(d => d.Value == roll.Dice.First().Value) ? 50 : 0;
//         }
//     }
//
//     // === ScoreCard ===
//     public class ScoreCard
//     {
//         private readonly Dictionary<CATEGORY, Score> _scores = new();
//
//         public void Assign(ICategory category, Roll roll)
//         {
//             if (_scores.ContainsKey(category.Type))
//                 throw new InvalidOperationException("Category already assigned");
//
//             int score = category.CalculateScore(roll);
//             _scores[category.Type] = new Score(score);
//         }
//
//         public Score GetScore(CATEGORY category)
//         {
//             return _scores.TryGetValue(category, out var score)
//                 ? score
//                 : new Score(0);
//         }
//
//         public int TotalScore => _scores.Values.Sum(s => s.Value);
//     }
//
//     // === Factory ===
//     public static class CategoryFactory
//     {
//         public static ICategory Create(CATEGORY category)
//         {
//             return category switch
//             {
//                 CATEGORY.ONES => new NumberCategory(category, 1),
//                 CATEGORY.TWOs => new NumberCategory(category, 2),
//                 CATEGORY.THREES => new NumberCategory(category, 3),
//                 CATEGORY.FOURS => new NumberCategory(category, 4),
//                 CATEGORY.FIVES => new NumberCategory(category, 5),
//                 CATEGORY.SIXES => new NumberCategory(category, 6),
//                 CATEGORY.PAIR => new PairCategory(),
//                 CATEGORY.TWO_PAIRS => new TwoPairsCategory(),
//                 CATEGORY.THREE_OF_A_KIND => new ThreeOfAKindCategory(),
//                 CATEGORY.FOUR_OF_A_KIND => new FourOfAKindCategory(),
//                 CATEGORY.SMALL_STRAIGHT => new SmallStraightCategory(),
//                 CATEGORY.LARGE_STRAIGHT => new LargeStraightCategory(),
//                 CATEGORY.FULL_HOUSE => new FullHouseCategory(),
//                 CATEGORY.YAHTZEE => new YahtzeeCategory(),
//                 _ => throw new ArgumentException("Unsupported category")
//             };
//         }
//     }
//
//     // === Usage Example ===
//     public class GameExample
//     {
//         public static void Main()
//         {
//             var roll = new Roll(new List<int> { 2, 2, 3, 3, 3 });
//             var scoreCard = new ScoreCard();
//
//             var category = CategoryFactory.Create(CATEGORY.FULL_HOUSE);
//             scoreCard.Assign(category, roll);
//
//             Console.WriteLine($"Score for FULL_HOUSE: {scoreCard.GetScore(CATEGORY.FULL_HOUSE).Value}");
//             Console.WriteLine($"Total Score: {scoreCard.TotalScore}");
//         }
//     }
// }
//
//
// // using Xunit;
// // using FluentAssertions;
// // using System;
// // using System.Collections.Generic;
// // using System.IO;
// //
// // namespace PedroKatas;
// //
// // public enum CATEGORY
// // {
// //     FOURs,
// //     TWOs,
// //     THREES,
// //     FIVES,
// //     SIXES,
// //     ONES,
// //     PAIR
// // }
// //
// // public class YahtzeeShould
// // {
// //     [Theory]
// //     [MemberData(nameof(TestData))]
// //     public void UpdateScordcard(string name, List<int> rolls, CATEGORY category, Dictionary<CATEGORY, int> expected)
// //     {
// //         var scoringSystem = new YatzeeScoringSystem();
// //         var result = scoringSystem.UpdateScoreCard(name, rolls, category);
// //
// //         result.Should().BeEquivalentTo(expected);
// //     }
// //
// //     public static IEnumerable<object[]> TestData =>
// //         new List<object[]>
// //         {
// //             new object[]
// //             {
// //                 "Satish",
// //                 new List<int> { 2, 2 },
// //                 CATEGORY.TWOs,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 4 },
// //                     { CATEGORY.FOURs, 0 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "Honza",
// //                 new List<int> { 4, 4, 1 },
// //                 CATEGORY.FOURs,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.FOURs, 8 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "Honza2",
// //                 new List<int> { 3, 3, 3, 3, 1 },
// //                 CATEGORY.THREES,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.THREES, 12 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "Satish2",
// //                 new List<int> { 5, 5, 5, 5, 5 },
// //                 CATEGORY.FIVES,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.FIVES, 25 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "Honza3",
// //                 new List<int> { 1, 2, 3, 4, 6 },
// //                 CATEGORY.SIXES,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.SIXES, 6 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "Satish3",
// //                 new List<int> { 1, 2, 3, 4, 5 },
// //                 CATEGORY.ONES,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.ONES, 1 }
// //                 }
// //             },
// //             new object[]
// //             {
// //                 "HonzaPair",
// //                 new List<int> {4, 4},
// //                 CATEGORY.PAIR,
// //                 new Dictionary<CATEGORY, int>
// //                 {
// //                     { CATEGORY.TWOs, 0 },
// //                     { CATEGORY.PAIR, 8 }
// //                 }
// //             }
// //         };
// // }
// //
// // public class YatzeeScoringSystem
// // {
// //     public Dictionary<string, Dictionary<CATEGORY, int>> scordCard = new()
// //     {
// //         {
// //             "Satish", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.FOURs, 0 }
// //             }
// //         },
// //         {
// //             "Honza", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.FOURs, 0 }
// //             }
// //             
// //         },
// //         {
// //             "Honza2", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.THREES, 0 }
// //             }
// //         },
// //         {
// //             "Satish2", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.FIVES, 0 }
// //             }
// //         },
// //         {
// //             "Honza3", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.SIXES, 0 }
// //             }
// //         },
// //         {
// //             "Satish3", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.ONES, 0 }
// //             }
// //         },
// //         {
// //             "HonzaPair", new Dictionary<CATEGORY, int>
// //             {
// //                 { CATEGORY.TWOs, 0 },
// //                 { CATEGORY.PAIR, 0 }
// //             }
// //         }
// //     };
// //
// //     public Dictionary<CATEGORY, int> UpdateScoreCard(string name, List<int> chosenRolls, CATEGORY expectedCATEGORY)
// //     {
// //         int score = 0;
// //
// //         // combinations
// //         if (CATEGORY.PAIR == expectedCATEGORY)
// //         {
// //             // var groupedRolls = chosenRolls.GroupBy(x => x)
// //             //     .Where(g => g.Count() >= 2)
// //             //     .Select(g => g.Key)
// //             //     .ToList();
// //
// //             int numberOfAppearences = 0;
// //             int chosenPairDie = 0;
// //
// //             for (var dieIndex = 0; dieIndex < chosenRolls.Count; dieIndex++)
// //             {
// //                 for (var dieIndexInner = 0; dieIndexInner < chosenRolls.Count; dieIndexInner++)
// //                 {
// //                     if (dieIndex == dieIndexInner)
// //                     {
// //                         continue;
// //                     }
// //                     if (chosenRolls[dieIndex] == chosenRolls[dieIndexInner])
// //                     {
// //                         numberOfAppearences++;
// //                         chosenPairDie = chosenRolls[dieIndex];
// //                     }
// //                 }
// //             }
// //             if (numberOfAppearences == 2)
// //             {
// //                 score = chosenPairDie * 2;
// //             }
// //             scordCard[name][expectedCATEGORY] = score;
// //             return scordCard[name]; 
// //         }
// //         
// //         // singles
// //         foreach (var die in chosenRolls)
// //         {
// //             if ((expectedCATEGORY == CATEGORY.TWOs && die == 2) ||
// //                 (expectedCATEGORY == CATEGORY.FOURs && die == 4) ||
// //                 (expectedCATEGORY == CATEGORY.THREES && die == 3) ||
// //                 (expectedCATEGORY == CATEGORY.FIVES && die == 5) ||
// //                 (expectedCATEGORY == CATEGORY.SIXES && die == 6) ||
// //                 (expectedCATEGORY == CATEGORY.ONES && die == 1))
// //             {
// //                 score += die;
// //             }
// //         }
// //
// //         scordCard[name][expectedCATEGORY] = score;
// //
// //         return scordCard[name];
// //     }
// // }