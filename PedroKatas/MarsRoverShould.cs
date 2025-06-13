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

        var expectedInitialPosition = new PositionWithDirection();

        rover.gridSize.Should().NotBeNull();
        rover.gridSize.Should().BeEquivalentTo(new int[] { 5, 5 });
        rover.initialPosition.Should().NotBeNull();
        rover.initialPosition.Should().BeEquivalentTo(expectedInitialPosition);
        
        
    }
    
    
    

    // acceptance tests
    [Fact]
    public void StartAndGoThroughSetInstructionsCaseOne()
    {

        var inputInstructions = "5 5\n1 2 N\nLMLMLMLMM";

        var rover = new MarsRover();
        var response = rover.doInstructions(inputInstructions);

        response.Should().NotBe(null);
        var expectedOutput = "1 3 N";
        response.Should().Be(expectedOutput);

    }

    [Fact]
    public void StartAndGoThroughSetInstructionsCaseTwo()
    {

        var inputInstructions = "5 5\n3 3 E\nMMRMMRMRRM";

        var rover = new MarsRover();
        var response = rover.doInstructions(inputInstructions);

        response.Should().NotBe(null);
        var expectedOutput = "5 1 E";
        response.Should().Be(expectedOutput);

    }

}

public class MarsRover
{
    public int[] gridSize { get; private set; }
    public PositionWithDirection initialPosition { get; private set; }
    
    public string doInstructions(string inputInstructions)
    {
        throw new System.NotImplementedException();
    }

    public void deconstructTheInstructions(string instructions)
    {
        // update the gridSize and initialPosition properties
        var gridSize = instructions.Split('\n')[0];
        var gridWidth = gridSize.Split(' ')[0];
        var gridHeight = gridSize.Split(' ')[1];

        
        
        // setup the initial position
    }
}

public class PositionWithDirection
{
    int x, y;
    char direction;
}