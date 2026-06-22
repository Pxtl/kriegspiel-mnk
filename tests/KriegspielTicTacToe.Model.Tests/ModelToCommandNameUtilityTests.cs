namespace KriegspielTicTacToe.Model.Tests;
using Xunit;

public class ModelToCommandNameUtilityTests
{
    
    #region BuildPlayerToCommandNameMap
    [Fact]
    public void BuildPlayerToCommandNameMap_SingleTypeableMark() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { new Player("A") }
        );
        
        map.ContainsKey(new Player("A")).Should().BeTrue();
        map.ContainsKey(new Player("B")).Should().BeFalse();
    }
    
    [Fact]
    public void BuildPlayerToCommandNameMap_WithOnlyTypeableMarks_ReturnsIdentity()
    {
        var marks = new[] { "X", "O", "9", "5" };
        var players = new List<Player>();
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }

        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        Assert.Equal(4, result.Count);
        foreach (var player in players) {
            Assert.Equal(player.Mark, result[player]);
        }
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_LowercaseLettersAreIdentity()
    {
        var marks = new[] { "x", "a", "z", "b" };
        var players = new List<Player>();
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }

        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        foreach (var player in players) {
            Assert.Equal(player.Mark, result[player]);
        }
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_AlphabeticLettersAreTypeable()
    {
        var marks = new[] { "X", "O", "A", "Z", "B" };
        var players = new List<Player>();
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }

        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        foreach (var player in players) {
            Assert.Equal(player.Mark, result[player]);
        }
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_PunctuationMarksAreNonTypeable()
    {
        var marks = new[] { ".", "!", "?", ":", ";" };
        var players = new List<Player>();
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }

        var expected = new Dictionary<Player, string>{
            [new Player(".")] = "1",
            [new Player("!")] = "2",
            [new Player("?")] = "3",
            [new Player(":")] = "4",
            [new Player(";")] = "5"
        };
        var actual = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_ExcludesUsedMarksFromAlternates()
    {
        var marks = new[] { "A", ".", "!", "1" };
        var players = new List<Player> { };
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }
        
        var expected = new Dictionary<Player, string>{
            [new Player("A")] = "A",
            [new Player(".")] = "2",
            [new Player("!")] = "3",
            [new Player("1")] = "1"
        };
        var actual = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_MultiCharMarkGetsAlternateKey()
    {
        // Multi-char marks cannot be used directly ( ArgumentException), 
        // but we can test they are not typeable by using Player.FromString with null
        var typeableMark = "A";
        var typeablePlayer = new Player(typeableMark);
        
        var players = new List<Player> {
            new Player(typeablePlayer.Mark),
        };
        
        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);
        
        // Single-char typeable mark returns identity
        Assert.Equal(typeableMark, result[typeablePlayer]);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_PreservesPlayerReferencesInDictionary()
    {
        var players = new List<Player> {
            new Player("X"),
            new Player("O")
        };

        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        var originalX = players.First(p => p.Mark == "X");
        Assert.True(result.ContainsKey(originalX));
        Assert.Equal("X", result[originalX]);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_ReturnsCorrectCountForEmptyInput()
    {
        var emptyPlayers = new List<Player>();
        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(emptyPlayers);
        Assert.Empty(result);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_DigitsAreTypeable()
    {
        var marks = new[] { "0", "1", "8", "9" };
        var players = new List<Player>();
        foreach (var mark in marks) {
            players.Add(new Player(mark));
        }

        var result = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        foreach (var player in players) {
            Assert.Equal(player.Mark, result[player]);
        }
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_MixedTypeableAndNonTypeableWorksCorrectly()
    {
        var typeable = new[] { "X", "O" };
        var nonTypeable = new[] { ".", "!" };
        var players = new List<Player>();
        foreach (var t in typeable) {
            players.Add(new Player(t));
        }
        foreach (var nt in nonTypeable) {
            players.Add(new Player(nt));
        }

        var expected = new Dictionary<Player, string>{
            [new Player("X")] = "X",
            [new Player("O")] = "O",
            [new Player(".")] = "1",
            [new Player("!")] = "2"
        };
        var actual = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(players);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_TwoSpecialChars_ReturnMap1and2() {
        var player1 = new Player("☃"); //unicode snowman
        var player2 = new Player("☂"); //unicode umbrella
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(
            new[] { player1, player2 }
        );
        
        map[player1].Should().Be("1");
        map[player2].Should().Be("2");
    }
    #endregion

    #region BuildPlayerToCommandNameMap Edge Cases
    
    [Fact]
    public void BuildPlayerToCommandNameMap_EmptyArray() {
        var map = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(Array.Empty<Player>());
        
        map.Count.Should().Be(0);
    }

    [Fact]
    public void BuildPlayerToCommandNameMap_NullValueThrows() {
        var action = () => {
            _ = ModelToCommandNameUtility.BuildPlayerToCommandNameMap(null!);
        };
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion
}

