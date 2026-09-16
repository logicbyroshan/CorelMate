using System;

namespace CorelMate.Curves;

public interface IConvertTextToCurvesService
{
    void ConvertSelection();
}

public sealed class ConvertTextToCurvesService : IConvertTextToCurvesService
{
    public void ConvertSelection() => throw new NotSupportedException("CorelDRAW object-model execution is not implemented in the foundation milestone.");
}