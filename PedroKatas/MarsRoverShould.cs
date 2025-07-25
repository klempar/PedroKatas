using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace PedroKatas;

public class MarsRoverShould
{
    
    // Unit tests
    [Fact]
    public void DeconstructTheInputInstructions()
    {
        var rover = new MarsRover();
        var instructions = "5 5\n1 2 N\nLMLMLMLMM";
        rover.deconstructTheInstructions(instructions);
    
        var expectedInitialPosition = new PositionWithDirection(1, 2, 'N');
    
        rover.gridSize.Should().NotBeNull();
        rover.gridSize.Should().BeEquivalentTo(new int[] { 5, 5 });
        rover.roverPosition.Should().NotBeNull();
        rover.roverPosition.Should().BeEquivalentTo(expectedInitialPosition);
        
    }
    
    // if I pass no commands, the rover should not move
    // if the rover goes beyond the grid
    // find some other small tests.
    
    
    
    //
    // // acceptance tests
    // [Fact]
    // public void StartAndGoThroughSetInstructionsCaseOne()
    // {
    //
    //     var inputInstructions = "5 5\n1 2 N\nLMLMLMLMM";
    //     
    //     var rover = new MarsRover();
    //     rover.deconstructTheInstructions(inputInstructions);
    //
    //     var response = rover.DoInstructions(inputInstructions);
    //
    //     response.Should().NotBe(null);
    //     var expectedOutput = "1 3 N";
    //     response.Should().Be(expectedOutput);
    //
    // }
    //
    // [Fact]
    // public void StartAndGoThroughSetInstructionsCaseTwo()
    // {
    //
    //     var inputInstructions = "5 5\n3 3 E\nMMRMMRMRRM";
    //
    //     var rover = new MarsRover();
    //     
    //     rover.deconstructTheInstructions(inputInstructions);
    //     var response = rover.DoInstructions(inputInstructions);
    //
    //     response.Should().NotBe(null);
    //     var expectedOutput = "1 5 N";
    //     response.Should().Be(expectedOutput);
    //
    // }

}

public enum Direction
{
    N, E, S, W
}

public static class DirectionExtensions
{
    public static Direction TurnLeft(this Direction direction)
    {
        return direction switch
        {
            Direction.N => Direction.W,
            Direction.E => Direction.N,
            Direction.S => Direction.E,
            Direction.W => Direction.S,
            _ => throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static Direction TurnRight(this Direction direction)
    {
        return direction switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new System.ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    
    public static Direction FromChar(char c)
    {
        return c switch
        {
            'N' => Direction.N,
            'E' => Direction.E,
            'S' => Direction.S,
            'W' => Direction.W,
            _ => throw new ArgumentException($"Invalid direction character: {c}")
        };
    }
}

public class MarsRover
{
    
    
    public int[] gridSize { get; set; }
    public PositionWithDirection roverPosition { get; set; }
    
    public string DoInstructions(string moveSequence)
    {
        foreach (var instruction in moveSequence.Split('\n')[2])
        {
            switch (instruction)
            {
                case 'L':
                    this.roverPosition.Direction = roverPosition.Direction.TurnLeft();
                    break;
                case 'R':
                    roverPosition.Direction = roverPosition.Direction.TurnRight();
                    break;
                case 'M':
                    roverPosition.MoveForward();
                    break;
                default:
                    throw new System.ArgumentException($"Invalid instruction: {instruction}");
            }
        }
        
        return $"{roverPosition.X} {roverPosition.Y} {roverPosition.Direction}";
    }

    public void deconstructTheInstructions(string instructions)
    {
        // update the gridSize and roverPosition properties
        var gridSize = instructions.Split('\n')[0];
        var gridWidth = gridSize.Split(' ')[0];
        var gridHeight = gridSize.Split(' ')[1];
        
        this.roverPosition = new PositionWithDirection(
            int.Parse(instructions.Split('\n')[1].Split(' ')[0]),
            int.Parse(instructions.Split('\n')[1].Split(' ')[1]),
            char.Parse(instructions.Split('\n')[1].Split(' ')[2])
        );

        this.gridSize = new int[] { int.Parse(gridWidth), int.Parse(gridHeight) };
    }
}

public class PositionWithDirection
{
    int _x, _y;
    private Direction _direction;

    public int X
    {
        get => _x;
        set => _x = value;
    }

    public int Y
    {
        get => _y;
        set => _y = value;
    }

    public Direction Direction { get; set; }

    public PositionWithDirection(int x, int y, char direction)
    {
        this._x = x;
        this._y = y;
        this._direction = DirectionExtensions.FromChar(direction);
    }

    public void MoveForward()
    {
        switch (Direction)
        {
            case Direction.N:
                this.Y++ ;
                break;
            case Direction.E:
                this.X--;
                break;
            case Direction.S:
                this.Y--;
                break;
            case Direction.W:
                this.X++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}