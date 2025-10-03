# Hash Validation Bypass

This modification disables hash validation checks in the Wabbajack installer process. 

## Changes Made

1. Modified `ThrowOnNonMatchingHash` methods to log warnings instead of throwing exceptions
2. Changed download validation to warn about hash mismatches but continue processing
3. Prevented file deletion when hashes don't match

## Purpose

Allows installation to continue even when files don't exactly match the expected hashes. This can be useful when:

- Installing older modlists with updated mods
- Working with modified game files
- Testing modlist installations with known modifications

## Note

This is a convenience modification and bypasses safety checks that ensure file integrity. Use with caution as it may result in incomplete or broken installations if the file differences are significant.