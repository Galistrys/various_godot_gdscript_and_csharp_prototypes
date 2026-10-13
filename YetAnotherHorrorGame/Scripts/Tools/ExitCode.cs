public enum ExitCode
{
    TestError = 0,
    Ok,
    SystemRequest,
    AutoloadDuplication,
    LoadDataFailed,
    InteractableWithoutId,
    ItemIdNotFound,
    NoteIdNotFound,
    DiaryIdNotFound,
    FailedToInitialize,
    PositionNotFound,
    LevelDoNotExists,
    IndexOutOfBounds,
    FileNotFound,
    WrongDataType
}