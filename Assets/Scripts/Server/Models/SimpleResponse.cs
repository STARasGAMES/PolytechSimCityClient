using System;

namespace Models.Response
{
    [Serializable]
    public class SimpleResponse
    {
        public bool IsError { get { return !string.IsNullOrEmpty(error) || errorCode != 0; } }
        public string error;
        public long errorCode;
        public string success;
        public string responseText;

        public override string ToString()
        {
            return base.ToString() + "\nIsError="+ IsError+" error="+ error+ " errorCode="+errorCode+ " success=" +success;
        }
    }
}