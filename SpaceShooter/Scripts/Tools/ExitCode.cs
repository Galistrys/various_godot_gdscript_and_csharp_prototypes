public enum ExitCode
{
    TestError = 0,
    Ok,
    SystemRequest,
    AutoloadDuplication,
    SingletonDuplication,
    FailedToInitialize,
    MapNotExists,
    DirectoryCouldNotBeOpened,
    DirectoryListFailed
}