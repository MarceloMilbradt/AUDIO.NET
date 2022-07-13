namespace SmartLedKit.Core.Network.Responses
{

    public struct LocalResponse
    {
        public Command Command { get; }
        public int ReturnCode { get; }
        public string? JSON { get; }

        internal LocalResponse(Command command, int returnCode, string? json)
        {
            Command = command;
            ReturnCode = returnCode;
            JSON = json;
        }

        public override string ToString() => $"{Command}: {JSON} (return code = {ReturnCode})";
    }
}
