# Game File Validation Fix

## Issue

When removing game file validation, an error occurred:

```
System.ArgumentNullException: Value cannot be null. (Parameter 'key')
at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior)
at System.Collections.Generic.Dictionary`2.Add(TKey key, TValue value)
at System.Linq.Enumerable.ToDictionary[TSource,TKey](IEnumerable`1 source, Func`2 keySelector, IEqualityComparer`1 comparer)
at Wabbajack.Installer.AInstaller`1.InstallArchives(CancellationToken token)
```

## Root Cause

In `AInstaller.cs`, the `InstallArchives` method was trying to create a dictionary using `VirtualFile` objects as keys. However, the `FileForArchiveHashPath` method can return `null` for certain hash paths (particularly for game files that weren't being validated). When a null value was encountered, it caused the `ToDictionary` operation to fail since dictionaries can't have null keys.

## Fix

Added a `.Where(a => a.VF != null)` filter before the `.GroupBy` operation to exclude any null `VirtualFile` references from being used as dictionary keys.

This allows the installation to proceed even when some archive hash paths don't resolve to actual files in the VFS.