using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PedroKatas
{
    public enum CATEGORY
    {
        ONES,
        TWOs,
        THREES,
        FOURS,
        FIVES,
        SIXES,
        PAIR,
        TWO_PAIRS,
        THREE_OF_A_KIND,
        FOUR_OF_A_KIND,
        SMALL_STRAIGHT,
        LARGE_STRAIGHT,
        FULL_HOUSE,
        YAHTZEE
    }

    public class DiceValue
    {
        private readonly int _value;

        public DiceValue(int value)
        {
            if (value < 1 || value > 6)
            {
                throw new ArgumentException("Dice value must be between 1 and 6");
            }
            _value = value;
        }

        public bool IsEqualTo(int target) => _value == target;
        public int Times(int multiplier) => _value * multiplier;
        public int AsInt() => _value;
    }

    public class Roll
    {
        private readonly List<DiceValue> _dice;

        public Roll(IEnumerable<int> dice)
        {
            _dice = dice.Select(d => new DiceValue(d)).ToList();
        }

        public int SumMatching(int value)
        {
            return _dice.Where(d => d.IsEqualTo(value)).Sum(d => d.AsInt());
        }

        public bool MatchesExact(params int[] expected)
        {
            return _dice.Select(d => d.AsInt()).ToHashSet().SetEquals(expected);
        }

        public List<int> FindGroupsWithSizeAtLeast(int size)
        {
            return _dice
                .GroupBy(d => d.AsInt())
                .Where(g => g.Count() >= size)
                .Select(g => g.Key)
                .OrderByDescending(k => k)
                .ToList();
        }

        public int Sum() => _dice.Sum(d => d.AsInt());

        public bool AllSame()
        {
            return _dice.All(d => d.AsInt() == _dice[0].AsInt());
        }

        public List<IGrouping<int, DiceValue>> Grouped()
        {
            return _dice.GroupBy(d => d.AsInt()).ToList();
        }
    }

    public class Score
    {
        private readonly int _value;

        public Score(int value)
        {
            _value = value;
        }

        public int AsInt() => _value;
    }

    public interface ICategory
    {
        CATEGORY Type();
        Score ScoreFor(Roll roll);
    }

    public class NumberCategory : ICategory
    {
        private readonly CATEGORY _type;
        private readonly int _target;

        public NumberCategory(CATEGORY type, int target)
        {
            _type = type;
            _target = target;
        }

        public CATEGORY Type() => _type;

        public Score ScoreFor(Roll roll)
        {
            return new Score(roll.SumMatching(_target));
        }
    }

    public class PairCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.PAIR;

        public Score ScoreFor(Roll roll)
        {
            var pairs = roll.FindGroupsWithSizeAtLeast(2);
            return new Score(pairs.Any() ? pairs.First() * 2 : 0);
        }
    }

    public class TwoPairsCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.TWO_PAIRS;

        public Score ScoreFor(Roll roll)
        {
            var pairs = roll.FindGroupsWithSizeAtLeast(2);
            if (pairs.Count < 2)
            {
                return new Score(0);
            }
            return new Score(pairs[0] * 2 + pairs[1] * 2);
        }
    }

    public class ThreeOfAKindCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.THREE_OF_A_KIND;

        public Score ScoreFor(Roll roll)
        {
            var triples = roll.FindGroupsWithSizeAtLeast(3);
            return new Score(triples.Any() ? triples.First() * 3 : 0);
        }
    }

    public class FourOfAKindCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.FOUR_OF_A_KIND;

        public Score ScoreFor(Roll roll)
        {
            var quads = roll.FindGroupsWithSizeAtLeast(4);
            return new Score(quads.Any() ? quads.First() * 4 : 0);
        }
    }

    public class SmallStraightCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.SMALL_STRAIGHT;

        public Score ScoreFor(Roll roll)
        {
            return new Score(roll.MatchesExact(1, 2, 3, 4, 5) ? 15 : 0);
        }
    }

    public class LargeStraightCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.LARGE_STRAIGHT;

        public Score ScoreFor(Roll roll)
        {
            return new Score(roll.MatchesExact(2, 3, 4, 5, 6) ? 20 : 0);
        }
    }

    public class FullHouseCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.FULL_HOUSE;

        public Score ScoreFor(Roll roll)
        {
            var groups = roll.Grouped();
            var hasThree = groups.Any(g => g.Count() == 3);
            var hasTwo = groups.Any(g => g.Count() == 2);

            if (hasThree && hasTwo)
            {
                return new Score(roll.Sum());
            }
            return new Score(0);
        }
    }

    public class YahtzeeCategory : ICategory
    {
        public CATEGORY Type() => CATEGORY.YAHTZEE;

        public Score ScoreFor(Roll roll)
        {
            return new Score(roll.AllSame() ? 50 : 0);
        }
    }

    public class ScoreCard
    {
        private readonly Dictionary<CATEGORY, Score> _scores = new();

        public void Assign(ICategory category, Roll roll)
        {
            var type = category.Type();
            if (_scores.ContainsKey(type))
            {
                throw new InvalidOperationException("Category already assigned");
            }

            _scores[type] = category.ScoreFor(roll);
        }

        public Score Get(CATEGORY category)
        {
            return _scores.TryGetValue(category, out var score)
                ? score
                : new Score(0);
        }

        public int Total() => _scores.Values.Sum(s => s.AsInt());
    }

    public static class CategoryFactory
    {
        public static ICategory Create(CATEGORY category)
        {
            return category switch
            {
                CATEGORY.ONES => new NumberCategory(category, 1),
                CATEGORY.TWOs => new NumberCategory(category, 2),
                CATEGORY.THREES => new NumberCategory(category, 3),
                CATEGORY.FOURS => new NumberCategory(category, 4),
                CATEGORY.FIVES => new NumberCategory(category, 5),
                CATEGORY.SIXES => new NumberCategory(category, 6),
                CATEGORY.PAIR => new PairCategory(),
                CATEGORY.TWO_PAIRS => new TwoPairsCategory(),
                CATEGORY.THREE_OF_A_KIND => new ThreeOfAKindCategory(),
                CATEGORY.FOUR_OF_A_KIND => new FourOfAKindCategory(),
                CATEGORY.SMALL_STRAIGHT => new SmallStraightCategory(),
                CATEGORY.LARGE_STRAIGHT => new LargeStraightCategory(),
                CATEGORY.FULL_HOUSE => new FullHouseCategory(),
                CATEGORY.YAHTZEE => new YahtzeeCategory()
            };
        }
    }

    public class PlayGame
    {
        private readonly Dictionary<string, ScoreCard> _players = new();

        public void RegisterPlayer(string name)
        {
            if (!_players.ContainsKey(name))
                _players[name] = new ScoreCard();
        }

        public ScoreCard GetScoreCard(string player) => _players[player];

        public void AssignCategory(string player, ICategory category, Roll roll)
        {
            _players[player].Assign(category, roll);
        }

        public int GetScore(string player)
        {
            return _players[player].Total();
        }
    }
    
    public class YahtzeeShould
    {
        [Fact]
        public void Let_Honza_Use_All_Categories_While_Satish_Uses_Only_Some()
        {
            var game = new PlayGame();
            game.RegisterPlayer("Honza");
            game.RegisterPlayer("Satish");

            var allCategories = Enum.GetValues(typeof(CATEGORY)).Cast<CATEGORY>().ToList();
            var fullRoll = new Roll(new[] { 6, 6, 6, 6, 6 });

            foreach (var category in allCategories)
            {
                game.AssignCategory("Honza", CategoryFactory.Create(category), fullRoll);
            }

            game.AssignCategory("Satish", CategoryFactory.Create(CATEGORY.ONES), new Roll(new[] { 1, 1, 1, 1, 1 }));
            game.AssignCategory("Satish", CategoryFactory.Create(CATEGORY.TWOs), new Roll(new[] { 2, 2, 3, 3, 4 }));

            game.GetScore("Honza").Should().BeGreaterThan(0);
            game.GetScoreCard("Honza").Get(CATEGORY.YAHTZEE).AsInt().Should().Be(50);

            game.GetScore("Satish").Should().BeGreaterThan(0);
            game.GetScoreCard("Satish").Get(CATEGORY.THREES).AsInt().Should().Be(0);
        }

        [Fact]
        public void Let_Satish_Complete_All_Categories_And_Honza_Only_Few()
        {
            var game = new PlayGame();
            game.RegisterPlayer("Honza");
            game.RegisterPlayer("Satish");

            var allCategories = Enum.GetValues(typeof(CATEGORY)).Cast<CATEGORY>().ToList();
            var rollSatish = new Roll(new[] { 5, 5, 5, 5, 5 });
            var rollHonza = new Roll(new[] { 3, 3, 3, 1, 1 });

            foreach (var category in allCategories)
            {
                game.AssignCategory("Satish", CategoryFactory.Create(category), rollSatish);
            }

            game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.THREES), rollHonza);
            game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.ONES), rollHonza);
            game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.FULL_HOUSE), rollHonza);

            game.GetScore("Satish").Should().BeGreaterThan(0);
            game.GetScoreCard("Satish").Get(CATEGORY.FIVES).AsInt().Should().Be(25);
            game.GetScore("Honza").Should().BeGreaterThan(0);
            game.GetScoreCard("Honza").Get(CATEGORY.FOURS).AsInt().Should().Be(0);
        }

        [Fact]
        public void Test_If_Players_Cannot_Reuse_Categories()
        {
            var game = new PlayGame();
            game.RegisterPlayer("Honza");
            game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.YAHTZEE), new Roll(new[] { 6, 6, 6, 6, 6 }));
            Action assignAgain = () => game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.YAHTZEE), new Roll(new[] { 6, 6, 6, 6, 6 }));
            assignAgain.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Test_If_Players_Each_Player_Has_Independent_ScoreCard()
        {
            var game = new PlayGame();
            game.RegisterPlayer("Honza");
            game.RegisterPlayer("Satish");

            game.AssignCategory("Honza", CategoryFactory.Create(CATEGORY.THREES), new Roll(new[] { 3, 3, 3, 3, 3 }));
            game.AssignCategory("Satish", CategoryFactory.Create(CATEGORY.THREES), new Roll(new[] { 1, 1, 1, 1, 1 }));

            game.GetScore("Honza").Should().Be(15);
            game.GetScore("Satish").Should().Be(0);
        }

        [Fact]
        public void Test_If_Players_FullGame_BothPlayers_AssignAllCategories()
        {
            var game = new PlayGame();
            game.RegisterPlayer("Honza");
            game.RegisterPlayer("Satish");

            var categories = Enum.GetValues(typeof(CATEGORY)).Cast<CATEGORY>().ToList();
            var fixedRoll = new Roll(new[] { 6, 6, 6, 6, 6 });

            foreach (var category in categories)
            {
                game.AssignCategory("Honza", CategoryFactory.Create(category), fixedRoll);
                game.AssignCategory("Satish", CategoryFactory.Create(category), fixedRoll);
            }

            game.GetScore("Honza").Should().BeGreaterThan(0);
            game.GetScore("Satish").Should().BeGreaterThan(0);
        }
        
        [Theory]
        [InlineData(new[] { 1, 1, 1, 4, 5 }, CATEGORY.ONES, 3)]
        [InlineData(new[] { 2, 2, 2, 2, 2 }, CATEGORY.TWOs, 10)]
        [InlineData(new[] { 3, 3, 3, 1, 2 }, CATEGORY.THREES, 9)]
        [InlineData(new[] { 4, 4, 1, 2, 5 }, CATEGORY.FOURS, 8)]
        [InlineData(new[] { 5, 5, 5, 5, 5 }, CATEGORY.FIVES, 25)]
        [InlineData(new[] { 6, 1, 2, 6, 6 }, CATEGORY.SIXES, 18)]
        public void Should_Calculate_Number_Categories_Correctly(int[] dice, CATEGORY category, int expected)
        {
            var roll = new Roll(dice);
            var score = CategoryFactory.Create(category).ScoreFor(roll);
            score.AsInt().Should().Be(expected);
        }

        [Fact]
        public void Should_Score_Pair()
        {
            var roll = new Roll(new[] { 3, 3, 1, 5, 6 });
            var score = CategoryFactory.Create(CATEGORY.PAIR).ScoreFor(roll);
            score.AsInt().Should().Be(6);
        }

        [Fact]
        public void Should_Score_Two_Pairs()
        {
            var roll = new Roll(new[] { 2, 2, 5, 5, 6 });
            var score = CategoryFactory.Create(CATEGORY.TWO_PAIRS).ScoreFor(roll);
            score.AsInt().Should().Be(14);
        }

        [Fact]
        public void Should_Score_Three_Of_A_Kind()
        {
            var roll = new Roll(new[] { 4, 4, 4, 2, 5 });
            var score = CategoryFactory.Create(CATEGORY.THREE_OF_A_KIND).ScoreFor(roll);
            score.AsInt().Should().Be(12);
        }

        [Fact]
        public void Should_Score_Four_Of_A_Kind()
        {
            var roll = new Roll(new[] { 5, 5, 5, 5, 2 });
            var score = CategoryFactory.Create(CATEGORY.FOUR_OF_A_KIND).ScoreFor(roll);
            score.AsInt().Should().Be(20);
        }

        [Fact]
        public void Should_Score_Small_Straight()
        {
            var roll = new Roll(new[] { 1, 2, 3, 4, 5 });
            var score = CategoryFactory.Create(CATEGORY.SMALL_STRAIGHT).ScoreFor(roll);
            score.AsInt().Should().Be(15);
        }

        [Fact]
        public void Should_Score_Large_Straight()
        {
            var roll = new Roll(new[] { 2, 3, 4, 5, 6 });
            var score = CategoryFactory.Create(CATEGORY.LARGE_STRAIGHT).ScoreFor(roll);
            score.AsInt().Should().Be(20);
        }

        [Fact]
        public void Should_Score_Full_House()
        {
            var roll = new Roll(new[] { 3, 3, 5, 5, 5 });
            var score = CategoryFactory.Create(CATEGORY.FULL_HOUSE).ScoreFor(roll);
            score.AsInt().Should().Be(21);
        }

        [Fact]
        public void Should_Score_Yahtzee()
        {
            var roll = new Roll(new[] { 6, 6, 6, 6, 6 });
            var score = CategoryFactory.Create(CATEGORY.YAHTZEE).ScoreFor(roll);
            score.AsInt().Should().Be(50);
        }

        [Fact]
        public void Should_Return_Zero_For_Invalid_Pair()
        {
            var roll = new Roll(new[] { 1, 2, 3, 4, 5 });
            var score = CategoryFactory.Create(CATEGORY.PAIR).ScoreFor(roll);
            score.AsInt().Should().Be(0);
        }

        [Fact]
        public void Should_Prevent_Reusing_Category()
        {
            var roll = new Roll(new[] { 2, 2, 2, 2, 2 });
            var scoreCard = new ScoreCard();
            var category = CategoryFactory.Create(CATEGORY.TWOs);

            scoreCard.Assign(category, roll);

            Action act = () => scoreCard.Assign(category, roll);

            act.Should().Throw<InvalidOperationException>().WithMessage("*already assigned*");
        }
    }
}
