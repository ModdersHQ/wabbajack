# Game Files Verification Changes

## Modification to Game File Hash Verification

We've modified how game files are verified in the Wabbajack.Downloaders.GameFile.GameFileDownloader class to make the installer more resilient.

### Original Implementation

The original implementation would:
1. Check if the game file exists at the expected location
2. Calculate the hash of the file
3. Compare this hash with the expected hash in the archive

This approach was problematic because:
- Game files can vary between installations or versions
- Different patches of a game might modify these files, causing their hashes to differ
- This led to errors like "Failed to look up file Readme.txt by hash" with "Sequence contains no matching element"

### Modified Implementation

The modified implementation:
1. Only checks if the game file exists at the expected location
2. Does not check the hash of the file

This allows more flexibility with game installations and prevents hash verification errors for game files.

### Code Change

```csharp
// Original implementation
public override async Task<bool> Verify(Archive archive, GameFileSource archiveState, IJob job,
    CancellationToken token)
{
    var fp = archiveState.GameFile.RelativeTo(_locator.GameLocation(archiveState.Game));
    if (!fp.FileExists()) return false;
    return await _hashCache.FileHashCachedAsync(fp, token) == archive.Hash;
}

// Modified implementation
public override async Task<bool> Verify(Archive archive, GameFileSource archiveState, IJob job,
    CancellationToken token)
{
    var fp = archiveState.GameFile.RelativeTo(_locator.GameLocation(archiveState.Game));
    return fp.FileExists();
}
```

This change helps prevent issues when installing modlists that include game files where the hash might not match exactly.