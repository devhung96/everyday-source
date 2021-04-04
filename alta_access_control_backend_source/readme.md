# 🌤️ Sample Project .Net Core 🌥️

## Kafka
```
1. Register user detect (Client --> Server)
Topic:REGISTER_USER_REQUEST
Value(String) = {
      "rgDectectUserId": "1",
      "rgDectectKey": "1",
      "modeId": "1",
      "TagCode": "ae8fa172-844e-4f72-b058-0787867e607a",
      "TicketTypeId": "d05ec2dc-cc7d-4962-bde3-b9e1ec751275",
      "registerDettectDetailRequests": [
        {
          "rgDectectDetailDateBegin": "2020-12-25 00:00:00",
          "rgDectectDetailDateEnd": "2020-12-26 23:00:00",
          "rgDectectDetailTimeBegin": "00:00:00",
          "rgDectectDetailTimeEnd": "23:00:00",
          "rgDectectDetailRepeat": 1,
          "rgDectectDetailRepeatValueData": [
        
          ]
        }
      ],
      "rgDectectExtension": ""
}



2. UnRegister user detect (Client --> Server)
Topic:UN_REGISTER_USER_REQUEST
Value(String) = {
                  "rgDectectUserId": "string",
                  "rgDectectKey": "string",
                  "modeId": "string",
                  "tagId": "string",
                  "ticketTypeId": "string",
                  "transactionId": "string"
                }




3. Register user detect response (Server --> Client)
Topic:REGISTER_USER_RESPONSE
Value(String) = {
                    DataResult = "" hoặc string
                    DataRequest = string (dữ liệu truyền lên)
                    IsSuccess = true,
                    Message = message,
                    TransactionId: string
                }

4. UnRegister user detect response (Server --> Client)
Topic:UN_REGISTER_USER_RESPONSE
Value(String) = {
                    DataResult = "" hoặc string
                    DataRequest = string (dữ liệu truyền lên)
                    IsSuccess = true,
                    Message = message,
                    TransactionId: string
                }

5. Unregister mutil user detect request (Client --> Server)
Topic:UN_REGISTER_USER_MUTIL_REQUEST

Key = (string)
Value(String) = {
                    UnRegisterUserDetects = [
                        {
                              "rgDectectUserId": "string",
                              "rgDectectKey": "string",
                              "modeId": "string",
                              "TagCode": "string",
                              "ticketTypeId": "string"
                        }
                    ]
                } 


6. Unregister mutil user detect response (Server --> Client)
Topic:UN_REGISTER_USER_MUTIL_RESPONSE
Key = (string)
Value(String) = {
                    DataResult = "" hoặc string
                    DataRequest = string (dữ liệu truyền lên)
                    IsSuccess = true,
                    Message = message
                }

7. register mutil user detect request (Client --> Server)
Topic:REGISTER_USER_MUTIL_REQUEST

Key = (string)
Value(String) = {
                    RegisterDetectDetails = [
                        {
                               "rgDectectUserId": "1",
                              "rgDectectKey": "1",
                              "modeId": "1",
                              "TagCode": "ae8fa172-844e-4f72-b058-0787867e607a",
                              "TicketTypeId": "d05ec2dc-cc7d-4962-bde3-b9e1ec751275",
                              "registerDettectDetailRequests": [
                                {
                                  "rgDectectDetailDateBegin": "2020-12-25 00:00:00",
                                  "rgDectectDetailDateEnd": "2020-12-26 23:00:00",
                                  "rgDectectDetailTimeBegin": "00:00:00",
                                  "rgDectectDetailTimeEnd": "23:00:00",
                                  "rgDectectDetailRepeat": 1,
                                  "rgDectectDetailRepeatValueData": [
        
                                  ]
                                }
                              ],
                              "rgDectectExtension": ""
                                                }
                    ]
                } 
8. register mutil user detect response (Server --> Client)
Topic:REGISTER_USER_MUTIL_RESPONSE
Key = (string)
Value(String) = {
                    DataResult = "" hoặc string
                    DataRequest = string (dữ liệu truyền lên)
                    IsSuccess = true,
                    Message = message
                }

9. Unregister user detect with user id request (Client --> Server)
Topic:UN_REGISTER_USER_WITH_USERID_REQUEST

Key = (string)
Value(String) = userId


10. Unregister user detect with user id reponse (Server --> Client)
Topic:UN_REGISTER_USER_WITH_USERID_RESPONSE

Key = (string)
Value(String) = {
                    DataResult = "" hoặc string
                    DataRequest = string (dữ liệu truyền lên)
                    IsSuccess = true,
                    Message = message
                }

```
