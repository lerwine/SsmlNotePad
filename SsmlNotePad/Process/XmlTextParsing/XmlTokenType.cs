namespace Erwine.Leonard.T.SsmlNotePad.Process.XmlTextParsing
{
    public enum XmlTokenType
    {
        None,
        InvalidToken,
        CharacterEntity,
        Comment,
        ProcessingInstruction,
        DocType,
        CDataSection,
        Element,
        NewLine,
        CharacterData
    }
}