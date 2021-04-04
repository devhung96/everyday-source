using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Kafka
{
    public static class TopicDefine
    {
        public const string REGISTER_USER_REQUEST = "REGISTER_USER_REQUEST";
        public const string REGISTER_USER_RESPONSE = "REGISTER_USER_RESPONSE";


        public const string REGISTER_USER_MUTIL_REQUEST = "REGISTER_USER_MUTIL_REQUEST";
        public const string REGISTER_USER_MUTIL_RESPONSE = "REGISTER_USER_MUTIL_RESPONSE";

        public const string UN_REGISTER_USER_REQUEST = "UN_REGISTER_USER_REQUEST";
        public const string UN_REGISTER_USER_RESPONSE = "UN_REGISTER_USER_RESPONSE";

        public const string UN_REGISTER_USER_MUTIL_REQUEST = "UN_REGISTER_USER_MUTIL_REQUEST";
        public const string UN_REGISTER_USER_MUTIL_RESPONSE = "UN_REGISTER_USER_MUTIL_RESPONSE";


        public const string UN_REGISTER_USER_WITH_USERID_REQUEST = "UN_REGISTER_USER_WITH_USERID_REQUEST";
        public const string UN_REGISTER_USER_WITH_USERID_RESPONSE = "UN_REGISTER_USER_WITH_USERID_RESPONSE";


        public  const  string DETECT_FACE_RESPONSE = "DETECT_FACE_RESPONSE";
        public const string ADD_TAG = "ADD_TAG";
        public const string ADD_TAG1 = "ADD_TAG1";
        public const string REMOVE_TAG = "REMOVE_TAG";   
        public const string UPDATE_TAG = "UPDATE_TAG";
        public const string REMOVE_TAG_RESPONSE = "REMOVE_TAG_RESPONSE";
        public const string ADD_TAG_RESPONSE = "ADD_TAG_RESPONSE";
        public const string UPDATE_TAG_RESPONSE = "UPDATE_TAG_RESPONSE";
        public const string ADD_TICKET_TYPE = "ADD_TICKET_TYPE";
        public const string REMOVE_TICKET_TYPE = "REMOVE_TICKET_TYPE";
        public const string UPDATE_TICKET_TYPE = "UPDATE_TICKET_TYPE";
        public const string ADD_TICKET_RESPONSE = "ADD_TICKET_RESPONSE";
    }
}
