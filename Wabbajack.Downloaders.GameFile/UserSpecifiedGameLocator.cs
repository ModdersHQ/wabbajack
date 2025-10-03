using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Wabbajack.DTOs;
using Wabbajack.Paths;
using Wabbajack.Paths.IO;

namespace Wabbajack.Downloaders.GameFile;

/// <summary>
/// Implementation of IGameLocator that uses a user-specified folder instead of auto-detecting game location.
/// </summary>
public class UserSpecifiedGameLocator : IGameLocator
{
    private readonly ILogger<UserSpecifiedGameLocator> _logger;
    private AbsolutePath _userSpecifiedLocation;
    private readonly Dictionary<Game, AbsolutePath> _gameLocations = new();
    
    public UserSpecifiedGameLocator(ILogger<UserSpecifiedGameLocator> logger)
    {
        _logger = logger;
        _userSpecifiedLocation = default;
    }

    /// <summary>
    /// Sets the game location to be used for all games.
    /// </summary>
    public void SetGameLocation(AbsolutePath location)
    {
        _userSpecifiedLocation = location;
        _logger.LogInformation("User specified game location set to: {location}", location);
    }

    /// <summary>
    /// Sets the game location for a specific game.
    /// </summary>
    public void SetGameLocation(Game game, AbsolutePath location)
    {
        if (location != default && location.DirectoryExists())
        {
            _gameLocations[game] = location;
            _logger.LogInformation("User specified {game} location set to: {location}", game, location);
        }
    }

    public AbsolutePath GameLocation(Game game)
    {
        // Check if we have a specific location for this game
        if (_gameLocations.TryGetValue(game, out var specificLocation))
            return specificLocation;

        if (_userSpecifiedLocation == default)
            throw new Exception($"Game folder not specified for {game}");

        return _userSpecifiedLocation;
    }

    public bool IsInstalled(Game game)
    {
        return _gameLocations.ContainsKey(game) || _userSpecifiedLocation != default;
    }

    public bool TryFindLocation(Game game, out AbsolutePath path)
    {
        if (_gameLocations.TryGetValue(game, out path))
            return true;

        path = _userSpecifiedLocation;
        return _userSpecifiedLocation != default;
    }
}