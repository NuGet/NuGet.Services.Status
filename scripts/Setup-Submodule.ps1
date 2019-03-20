Function Invoke-Git {
    [CmdletBinding()]
    Param(
        [string[]] $Arguments
    )

    # We are invoking git through cmd here because otherwise the redirection does not process until after git has completed, leaving errors in the stream.
    Write-Host "git $Arguments"
    & cmd /c "git $Arguments 2>&1"
}

Write-Host 'Resetting submodules'
Invoke-Git -Arguments ('submodule', 'deinit', '--all', '-f')

Write-Host "Updating submodules"
Invoke-Git -Arguments ('submodule', 'update', '--init', '--')