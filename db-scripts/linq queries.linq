<Query Kind="SQL">
  <Connection>
    <ID>0672da94-c4c9-44b8-908c-5c96d5176638</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>Webhooks-Delivery</Database>
    <DisplayName>webhooks</DisplayName>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

insert into WebhookEPs(Name,Url,Secret)
VALUES ('Test Endpoint', 'https://webhook.site/test', 'my-secret-key-123');

--check
Select * from WebhookEPs;

--insert test events
insert into WebhookEvents(EventpointID,EventType, Payload)
Values(1,'order.created','{"orderid":123,"amount": 124.99, "quanitity": 5}');

--check
select * from WebhookEvents

--simulate event 
update WebhookEvents
set ProcessingLockUntil = Dateadd(MINUTE,5,getutcdate())
Output inserted.*
where id = 1
and Status = 0
and (ProcessingLockUntil is null or ProcessingLockUntil < getutcdate());

--simulate delivery failure
update WebHookEvents
set
AttemptCount = AttemptCount + 1,
LastAttemptAt = getutcdate(),
LastResponseCode = 500,
LastErrorMessage = 'INternal server error',
NextAttemptAt = DateAdd(second,power(2,AttemptCount +1),getutcdate()),
ProcessingLockUntil = null
where id = 1;

--check retry time 
SELECT Id, AttemptCount, NextAttemptAt, LastResponseCode FROM WebhookEvents;

--simluate successful delivery
update WebhookEvents
set Status =1, DeliveredAt = getutcdate(), ProcessingLockUntil = null
where id =1;

INSERT INTO WebhookEvents (EventPointId, EventType, Payload)
VALUES (1, 'test.event', '{"message": "hello"}');
--check
select * from WebhookEvents where id = 9;

SELECT Id, Name, ApiKey FROM WebhookEPs;

SELECT * FROM WebhookEPs ORDER BY Id DESC;


SELECT Id, Name, Url, IsActive, ApiKey FROM WebhookEPs WHERE Id = 7;
